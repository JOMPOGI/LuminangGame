using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using Luminang.Database;

namespace Luminang.UI.Minigames
{
    public class WordRushManager : MonoBehaviour
    {
        [Header("Debug & Testing")]
        public bool autoStartForTesting = true;
        public int testLanguageId = 1;
        public string testCategoryId = "";

        [Header("Global Settings (Dynamic)")]
        public static int CurrentLanguageId = 1;
        public static string CurrentCategoryId = ""; 

        [Header("Game Settings")]
        public int promptsPerGame = 15;
        public int maxLives = 3;
        public float passThreshold = 75f;

        [Header("UI References - Game")]
        public Image imageChallenges;
        public TextMeshProUGUI imageTextPrompt;
        public TextMeshProUGUI transcriptionText;
        public TextMeshProUGUI accuracyScoreText; // Live score
        public Button micButton;
        public Sprite micIdleSprite;
        public Sprite micRecordingSprite;
        public GameObject[] voiceVisualizers; 
        public Image[] hearts; 
        public Sprite activeHeartSprite;
        public Sprite inactiveHeartSprite;

        [Header("UI References - Result Panels")]
        public GameObject wordRushPanel;
        public GameObject resultBackgroundPanel;
        public GameObject youWinResult;
        public GameObject youLoseResult;
        
        [Header("UI References - Help Panel")]
        public GameObject helpBackgroundPanel;
        public GameObject helpPanel; // The child panel to scale up
        public Button helpButton;

        [Header("Animation Settings")]
        public float overlayFadeDuration = 0.4f;
        public float panelScaleDuration = 0.35f;
        
        [Header("Win Panel Details")]
        public TextMeshProUGUI winScoreText;
        public TextMeshProUGUI winWordsMatchText;

        [Header("Lose Panel Details")]
        public TextMeshProUGUI loseScoreText;
        public TextMeshProUGUI loseWordsMatchText;

        [Header("Transcription Settings")]
        public string placeholderText = "_____";

        // Internal State
        private List<RushPrompt> activePrompts = new List<RushPrompt>();
        private int currentPromptIndex = 0;
        private int correctCount = 0; // Track wins
        private int currentLives;
        private bool _isRecording = false;
        private bool _isProcessing = false;
        private bool _isTransitioning = false;
        private Coroutine _helpCoroutine;

        private void Awake()
        {
            if (transcriptionText != null) transcriptionText.text = placeholderText;
        }

        private async void OnEnable()
        {
            // If spawned dynamically by MinigameManager, fetch the real Category UUID!
            if (MinigameManager.Instance != null && !string.IsNullOrEmpty(MinigameManager.Instance.CurrentCategory))
            {
                string catName = MinigameManager.Instance.CurrentCategory;
                int langId = MinigameManager.Instance.CurrentLanguageId;
                
                Debug.Log($"[WordRush] Spawned via MinigameManager! Fetching UUID for category: {catName} (Lang: {langId})...");
                
                if (SupabaseManager.Instance == null || SupabaseManager.Instance.client == null) return;

                var categoryResponse = await SupabaseManager.Instance.client
                    .From<LessonCategoryModel>()
                    .Filter("name", Postgrest.Constants.Operator.Equals, catName)
                    .Get();

                if (categoryResponse.Models.Count > 0)
                {
                    string catId = categoryResponse.Models[0].Id;
                    Debug.Log($"[WordRush] Success! Found ID '{catId}' for '{catName}'. Starting game with Language {langId}!");
                    StartGame(langId, catId);
                }
                else
                {
                    Debug.LogError($"[WordRush] Database Error: Category '{catName}' not found!");
                }
            }
        }

        private void Start()
        {
            if (autoStartForTesting)
            {
                Debug.Log("<color=cyan>[WordRush] Auto-starting for testing...</color>");
                StartGame(testLanguageId, testCategoryId);
            }
        }

        public void StartGame(int langId, string catId)
        {
            StopAllCoroutines(); // Reset animations first!
            Debug.Log($"<color=cyan>[WordRush] Starting Game - Lang: {langId}, Cat: {catId}</color>");
            CurrentLanguageId = langId;
            CurrentCategoryId = catId;

            // Show help at the very beginning (with animation to ensure alpha is 1)
            ToggleHelp(true);

            // Clear old visuals immediately
            if (imageChallenges != null) imageChallenges.sprite = null;
            if (transcriptionText != null) transcriptionText.text = "";
            if (accuracyScoreText != null) accuracyScoreText.text = "";
            if (imageTextPrompt != null) imageTextPrompt.text = "Loading...";

            currentLives = maxLives;
            currentPromptIndex = 0;
            correctCount = 0;
            UpdateHeartsUI();

            // Reset internal state flags
            _isRecording = false;
            _isProcessing = false;
            _isTransitioning = false;

            // DISABLE mic until everything is downloaded and ready
            if (micButton != null) micButton.interactable = false;

            StartCoroutine(InitializeGameFlow());
            ShowGamePanel();
        }

        private IEnumerator InitializeGameFlow()
        {
            if (imageTextPrompt != null) imageTextPrompt.text = "Connecting to Supabase...";
            
            Debug.Log("[WordRush] Checking Supabase Instance...");
            if (SupabaseManager.Instance == null || SupabaseManager.Instance.client == null)
            {
                Debug.LogError("[WordRush] SupabaseManager.Instance or Client is NULL!");
                if (imageTextPrompt != null) imageTextPrompt.text = "System Error: No Database!";
                yield break;
            }

            Debug.Log("[WordRush] Fetching Prompts...");
            // 2. Fetch Prompts (Limit to a reasonable pool or fetch all and shuffle)
            var getTask = SupabaseManager.Instance.client.From<WordRushPromptModel>()
                .Select("id, clue_text, idle_image_url, happy_image_url, confused_image_url, happy_feedback, confused_feedback, Translation:vocabulary_translation_id(*)")
                .Get();

            yield return new WaitUntil(() => getTask.IsCompleted);

            if (getTask.IsFaulted)
            {
                Debug.LogError($"[WordRush] Supabase Query Failed: {getTask.Exception?.Message}");
                if (imageTextPrompt != null) imageTextPrompt.text = "Connection Error!";
                yield break;
            }

            if (getTask.Result.Models == null)
            {
                Debug.LogError("[WordRush] Supabase returned NULL models!");
                yield break;
            }

            var allModels = getTask.Result.Models;
            
            // Filter by Language manually
            var filteredModels = allModels.Where(m => m.Translation != null && m.Translation.LanguageId == CurrentLanguageId).ToList();
            
            if (!string.IsNullOrEmpty(CurrentCategoryId))
            {
                filteredModels = filteredModels.Where(m => 
                    m.Translation.Vocabulary != null && 
                    m.Translation.Vocabulary.CategoryId == CurrentCategoryId).ToList();
            }

            if (filteredModels.Count == 0)
            {
                Debug.LogWarning("[WordRush] No prompts found after filtering!");
                if (imageTextPrompt != null) imageTextPrompt.text = "No challenges found!";
                yield break;
            }

            // --- Diverse Selection Logic ---
            // 1. Group by translated text to find unique greeting types (8 types)
            var groups = filteredModels.GroupBy(m => m.Translation?.TranslatedText ?? "???").ToList();
            
            List<WordRushPromptModel> selectedModels = new List<WordRushPromptModel>();
            List<WordRushPromptModel> remainingPool = new List<WordRushPromptModel>();

            foreach (var group in groups)
            {
                var list = group.OrderBy(x => Random.value).ToList();
                // Pick the first one from each group to guarantee it's in the game
                selectedModels.Add(list[0]);
                // Put the others in the pool for random selection
                remainingPool.AddRange(list.Skip(1));
            }

            // 2. Fill the remaining slots (up to 15) from the pool
            int needed = promptsPerGame - selectedModels.Count;
            if (needed > 0)
            {
                var extra = remainingPool.OrderBy(x => Random.value).Take(needed).ToList();
                selectedModels.AddRange(extra);
            }

            // 3. Final shuffle of the 15 chosen prompts
            var finalModels = selectedModels.OrderBy(x => Random.value).Take(promptsPerGame).ToList();

            if (imageTextPrompt != null) imageTextPrompt.text = "Downloading images...";
            List<RushPrompt> downloadedPrompts = new List<RushPrompt>();
            foreach (var model in finalModels)
            {
                RushPrompt p = new RushPrompt();
                p.clueText = model.ClueText;
                p.targetPhrase = model.Translation?.TranslatedText ?? "???";
                p.happyFeedback = model.HappyFeedback;
                p.confusedFeedback = model.ConfusedFeedback;

                // Check Cache first, then fallback to Download
                if (MinigameAssetCache.Instance != null)
                {
                    p.idleSprite = MinigameAssetCache.Instance.GetSprite(model.IdleImageUrl);
                    p.happySprite = MinigameAssetCache.Instance.GetSprite(model.HappyImageUrl);
                    p.confusedSprite = MinigameAssetCache.Instance.GetSprite(model.ConfusedImageUrl);
                }

                // Fallback: If cache is empty/missing, download them now
                if (p.idleSprite == null) yield return StartCoroutine(DownloadSprite(model.IdleImageUrl, s => p.idleSprite = s));
                if (p.happySprite == null) yield return StartCoroutine(DownloadSprite(model.HappyImageUrl, s => p.happySprite = s));
                if (p.confusedSprite == null) yield return StartCoroutine(DownloadSprite(model.ConfusedImageUrl, s => p.confusedSprite = s));

                downloadedPrompts.Add(p);
            }

            SetupGame(downloadedPrompts);
        }

        private IEnumerator DownloadSprite(string url, System.Action<Sprite> callback)
        {
            if (string.IsNullOrEmpty(url)) yield break;

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(www);
                    Sprite s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    callback?.Invoke(s);
                }
            }
        }

        public void SetupGame(List<RushPrompt> allPrompts)
        {
            activePrompts = new List<RushPrompt>();
            activePrompts.AddRange(allPrompts);
            while (activePrompts.Count < promptsPerGame)
                activePrompts.Add(allPrompts[Random.Range(0, allPrompts.Count)]);
            
            activePrompts = activePrompts.OrderBy(x => Random.value).ToList();
            
            // NOW we can enable the mic!
            if (micButton != null) micButton.interactable = true;

            UpdatePromptUI();
        }

        private void UpdatePromptUI()
        {
            if (currentPromptIndex < activePrompts.Count)
            {
                var prompt = activePrompts[currentPromptIndex];
                imageTextPrompt.text = prompt.clueText;
                imageChallenges.sprite = prompt.idleSprite;
                if (transcriptionText != null) transcriptionText.text = placeholderText;
                if (accuracyScoreText != null) accuracyScoreText.text = "0%";
            }
        }

        private void UpdateHeartsUI()
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                if (hearts[i] != null)
                    hearts[i].sprite = (i < currentLives) ? activeHeartSprite : inactiveHeartSprite;
            }
        }

        // --- Recording Logic ---

        public void OnMicButtonClicked()
        {
            Debug.Log("<color=yellow>[WordRush] Mic Button Clicked!</color>");
            if (_isTransitioning || _isProcessing) 
            {
                Debug.LogWarning("[WordRush] Cannot record while transitioning or processing.");
                return;
            }

            if (!_isRecording)
            {
                Debug.Log("[WordRush] Starting Recording...");
                _isRecording = true;
                if (micButton != null) micButton.image.sprite = micRecordingSprite;
                SpeechRecorder.Instance.StartRecording();
                StartCoroutine(AnimateVisualizers());
            }
            else
            {
                Debug.Log("[WordRush] Stopping Recording...");
                _isRecording = false;
                if (micButton != null) micButton.image.sprite = micIdleSprite;
                string path = SpeechRecorder.Instance.StopRecording();
                if (!string.IsNullOrEmpty(path)) ProcessSpeech(path);
            }
        }

        private IEnumerator AnimateVisualizers()
        {
            while (_isRecording)
            {
                foreach (var v in voiceVisualizers)
                {
                    if (v == null) continue;
                    float scale = Random.Range(0.5f, 1.5f);
                    v.transform.localScale = new Vector3(1, scale, 1);
                }
                yield return new WaitForSeconds(0.1f);
            }
            foreach (var v in voiceVisualizers) if (v != null) v.transform.localScale = Vector3.one;
        }

        private void ProcessSpeech(string path)
        {
            _isProcessing = true;
            if (micButton != null) micButton.interactable = false; // Disable to prevent spam
            if (transcriptionText != null) transcriptionText.text = "Processing...";
            
            // Send the target phrase as a hint to the AI to maximize accuracy
            string targetHint = activePrompts[currentPromptIndex].targetPhrase;
            GroqWhisperManager.Instance.Transcribe(path, OnTranscriptionSuccess, OnTranscriptionError, targetHint);
        }

        private void OnTranscriptionSuccess(string result)
        {
            _isProcessing = false;
            Debug.Log($"<color=white>[WordRush] AI Heard: \"{result}\"</color>");
            if (transcriptionText != null) transcriptionText.text = result;
            
            string target = activePrompts[currentPromptIndex].targetPhrase;
            float accuracy = PhraseEvaluator.Instance.CalculateAccuracy(result, target);
            if (accuracyScoreText != null) accuracyScoreText.text = $"{accuracy:F0}%";

            if (accuracy >= passThreshold) 
            {
                correctCount++; // +1 for the final score
                StartCoroutine(HandleSuccess());
            }
            else StartCoroutine(HandleFailure());
        }

        private void OnTranscriptionError(string err)
        {
            _isProcessing = false;
            if (micButton != null) micButton.interactable = true; // Re-enable on error
            if (transcriptionText != null) transcriptionText.text = "Error! Try again.";
        }

        private IEnumerator HandleSuccess()
        {
            _isTransitioning = true;
            if (micButton != null) micButton.interactable = false; 
            
            var prompt = activePrompts[currentPromptIndex];
            if (imageChallenges != null) imageChallenges.sprite = prompt.happySprite;
            if (imageTextPrompt != null) imageTextPrompt.text = $"<color=#008000>{prompt.happyFeedback}</color>";

            yield return new WaitForSeconds(2.0f);

            currentPromptIndex++;
            if (currentPromptIndex >= activePrompts.Count) ShowWinPanel();
            else UpdatePromptUI();
            
            if (micButton != null) micButton.interactable = true; // Re-enable for next question
            _isTransitioning = false;
        }

        private IEnumerator HandleFailure()
        {
            _isTransitioning = true;
            if (micButton != null) micButton.interactable = false; 
            
            var prompt = activePrompts[currentPromptIndex];
            
            Debug.Log("<color=orange>[WordRush] Wrong Answer! Triggering confused state...</color>");
            if (imageChallenges != null) imageChallenges.sprite = prompt.confusedSprite;
            if (imageTextPrompt != null) imageTextPrompt.text = $"<color=#B22222>{prompt.confusedFeedback}</color>";
            
            currentLives--;
            UpdateHeartsUI();

            if (currentLives <= 0)
            {
                yield return new WaitForSeconds(1.0f);
                ShowLosePanel();
            }
            else
            {
                yield return new WaitForSeconds(3.0f);
                UpdatePromptUI();
                if (micButton != null) micButton.interactable = true; 
            }
            
            _isTransitioning = false;
        }

        public void OnTryAgainClicked()
        {
            // Hide everything before starting fresh
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(false);
            if (youWinResult != null) youWinResult.SetActive(false);
            if (youLoseResult != null) youLoseResult.SetActive(false);

            StartGame(CurrentLanguageId, CurrentCategoryId);
        }

        public void OnExitClicked()
        {
            // 1. Reset Game State for next time the prefab is opened
            currentPromptIndex = 0;
            correctCount = 0;
            currentLives = maxLives;
            
            // 2. Hide all sub-panels
            if (youWinResult != null) youWinResult.SetActive(false);
            if (youLoseResult != null) youLoseResult.SetActive(false);
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(false);
            if (helpBackgroundPanel != null) helpBackgroundPanel.SetActive(false);
            
            // 3. Destroy the clone properly via the Manager
            if (MinigameManager.Instance != null)
            {
                MinigameManager.Instance.HideMinigame();
            }
            else
            {
                // Fallback if no manager exists
                if (wordRushPanel != null) wordRushPanel.SetActive(false);
            }
            
            Debug.Log("[WordRush] Prefab closed and reset.");
        }

        public void ToggleHelp(bool show)
        {
            Debug.Log($"[WordRush] ToggleHelp called: {show}");
            if (helpBackgroundPanel != null)
            {
                if (_helpCoroutine != null) StopCoroutine(_helpCoroutine);
                
                if (show) 
                {
                    _helpCoroutine = StartCoroutine(ShowHelpSequence());
                }
                else 
                {
                    // Instant close to avoid animation conflicts
                    helpBackgroundPanel.SetActive(false);
                    if (helpPanel != null) helpPanel.SetActive(false);
                    Debug.Log("[WordRush] Help Panel forced closed.");
                }
            }
            else
            {
                Debug.LogError("[WordRush] ToggleHelp failed: helpBackgroundPanel is not assigned!");
            }
        }

        private IEnumerator HideHelpSequence()
        {
            CanvasGroup group = helpBackgroundPanel.GetComponent<CanvasGroup>();
            float elapsed = 0f;

            // Phase 1: Scale down and Fade out
            while (elapsed < panelScaleDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / panelScaleDuration);
                
                // Scale down (Inverse of ease-out)
                if (helpPanel != null)
                    helpPanel.transform.localScale = Vector3.one * (1f - t);
                
                if (group != null)
                    group.alpha = 1f - t;
                    
                yield return null;
            }

            if (helpPanel != null) helpPanel.transform.localScale = Vector3.zero;
            if (group != null) group.alpha = 0f;
            helpBackgroundPanel.SetActive(false);
        }

        private IEnumerator ShowHelpSequence()
        {
            // 1. Initial State
            if (helpPanel != null)
            {
                helpPanel.SetActive(false);
                helpPanel.transform.localScale = Vector3.zero;
            }

            // 2. Ensure CanvasGroup for fading
            CanvasGroup group = helpBackgroundPanel.GetComponent<CanvasGroup>();
            if (group == null) group = helpBackgroundPanel.AddComponent<CanvasGroup>();

            // Phase 1: Fade in overlay
            group.alpha = 0f;
            helpBackgroundPanel.SetActive(true);

            float elapsed = 0f;
            while (elapsed < overlayFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Clamp01(elapsed / overlayFadeDuration);
                yield return null;
            }
            group.alpha = 1f;

            // Phase 2: Scale up Help Panel
            if (helpPanel != null)
            {
                helpPanel.SetActive(true);
                elapsed = 0f;
                while (elapsed < panelScaleDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float t = Mathf.Clamp01(elapsed / panelScaleDuration);
                    float eased = 1f - Mathf.Pow(1f - t, 3f); // Ease-out cubic
                    helpPanel.transform.localScale = Vector3.one * eased;
                    yield return null;
                }
                helpPanel.transform.localScale = Vector3.one;
            }
        }

        public void ShowGamePanel() { if (wordRushPanel != null) wordRushPanel.SetActive(true); }

        private void ShowWinPanel() 
        { 
            UpdateResultUI();
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(true); 
            if (youWinResult != null) youWinResult.SetActive(true); 
            if (youLoseResult != null) youLoseResult.SetActive(false); // Safety
        }

        private void ShowLosePanel() 
        { 
            UpdateResultUI();
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(true); 
            if (youLoseResult != null) youLoseResult.SetActive(true); 
            if (youWinResult != null) youWinResult.SetActive(false); // Safety
        }

        private void UpdateResultUI()
        {
            int total = activePrompts.Count;
            string matchString = $"{correctCount} / {total}";
            float percent = ((float)correctCount / total) * 100f;
            string percentString = $"{percent:F0}%";

            // Update Win Panel
            if (winWordsMatchText != null) winWordsMatchText.text = matchString;
            if (winScoreText != null) winScoreText.text = percentString;

            // Update Lose Panel
            if (loseWordsMatchText != null) loseWordsMatchText.text = matchString;
            if (loseScoreText != null) loseScoreText.text = percentString;
        }
    }
}

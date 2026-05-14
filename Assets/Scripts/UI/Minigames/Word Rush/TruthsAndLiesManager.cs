using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Luminang.Database;

namespace Luminang.UI.Minigames
{
    public class TruthsAndLiesManager : MonoBehaviour
    {
        [Header("Scene Groups")]
        public GameObject choicesGroup;
        public GameObject speechPracticeGroup;

        [Header("Header/Phrase UI")]
        public TextMeshProUGUI wordPhraseText;
        public TextMeshProUGUI questionFeedbackText;
        public TextMeshProUGUI tipText;
        public TextMeshProUGUI roundProgressText;

        [Header("Choice UI (3 Frames)")]
        public Image[] choiceFrames; // ChoiceFrame1, 2, 3
        public TextMeshProUGUI[] choiceTexts; // The Choice child
        public Image[] checkOrWrongIcons; // The CheckOrWrong child
        public Sprite checkmarkSprite;
        public Sprite xMarkSprite;
        public Sprite invisibleSprite;

        [Header("Speech UI")]
        public Button micButton;
        public Image micButtonImage;
        public Sprite micOnSprite;
        public Sprite micOffSprite;
        public GameObject[] voiceVisualizers;
        public TextMeshProUGUI transcriptionText;
        public TextMeshProUGUI accuracyScoreValue; // The AccuracyScore TMP
        public Image correctIndicator; // Checkmark after success

        [Header("Hearts/Lives")]
        public Image[] hearts;
        public Sprite heartActive;
        public Sprite heartInactive;
        public int maxLives = 3;

        [Header("Game Settings")]
        public int totalRounds = 10;
        public float feedbackDelay = 2.0f;
        public float passThreshold = 0.75f; // 75%

        private int currentRound = 1;
        private int currentLives;
        private bool isListening = false;
        private List<TruthsAndLiesRoundData> gameRounds = new List<TruthsAndLiesRoundData>();
        private TruthsAndLiesRoundData currentRoundData;
        private int correctLieIndex;

        // Feedback Strings
        private string[] encouragementPhrases = { "You can do it!", "Keep trying!", "Almost there!", "Give it another go!" };

        [Header("Result Panels")]
        public GameObject resultBackgroundPanel;
        public GameObject youWinPanel;
        public GameObject youLosePanel;
        public TextMeshProUGUI winScoreText;
        public TextMeshProUGUI winWordsMatchText;
        public TextMeshProUGUI loseScoreText;
        public TextMeshProUGUI loseWordsMatchText;
        public GameObject mainGameContainer; // Drag For2Truths1LieGame here

        [Header("Help Panel")]
        public GameObject helpBackgroundPanel;
        public GameObject helpPanel;
        public float overlayFadeDuration = 0.4f;
        public float panelScaleDuration = 0.35f;

        [Header("Testing")]
        public bool testOnStart = false;
        public int testLanguageId = 1; // 1 for Ilokano
        public string testCategoryId = ""; // Paste your Gratitude Category ID here

        private int currentLangId;
        private string currentCatId;
        private int correctGuesses = 0;
        private Coroutine _helpCoroutine;

        private async void OnEnable()
        {
            // If spawned dynamically by MinigameManager, fetch the real Category UUID!
            if (MinigameManager.Instance != null && !string.IsNullOrEmpty(MinigameManager.Instance.CurrentCategory))
            {
                string catName = MinigameManager.Instance.CurrentCategory;
                int langId = MinigameManager.Instance.CurrentLanguageId;
                
                Debug.Log($"[2T1L] Spawned via MinigameManager! Fetching UUID for category: {catName} (Lang: {langId})...");
                
                if (SupabaseManager.Instance == null || SupabaseManager.Instance.client == null) return;

                var categoryResponse = await SupabaseManager.Instance.client
                    .From<LessonCategoryModel>()
                    .Filter("name", Postgrest.Constants.Operator.Equals, catName)
                    .Get();

                if (categoryResponse.Models.Count > 0)
                {
                    string catId = categoryResponse.Models[0].Id;
                    Debug.Log($"[2T1L] Success! Found ID '{catId}' for '{catName}'. Starting game with Language {langId}!");
                    StartGame(langId, catId);
                }
                else
                {
                    Debug.LogError($"[2T1L] Database Error: Category '{catName}' not found!");
                }
            }
        }

        private void Start()
        {
            if (testOnStart && !string.IsNullOrEmpty(testCategoryId))
            {
                Debug.Log($"[2T1L] Testing mode started! Lang: {testLanguageId}, Cat: {testCategoryId}");
                StartGame(testLanguageId, testCategoryId);
            }
        }

        public void StartGame(int langId, string catId)
        {
            StopAllCoroutines(); // Reset animations first!
            currentLangId = langId;
            currentCatId = catId;
            currentRound = 1;
            currentLives = maxLives;
            correctGuesses = 0;
            UpdateHeartsUI();

            // Reset flags
            isListening = false;

            // DISABLE mic until we get to the speech practice round
            if (micButton != null) micButton.interactable = false;
            
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(false);
            
            // Show help at the very beginning (with animation to reset alpha)
            ToggleHelp(true);
            
            StartCoroutine(InitializeGameFlow(langId, catId));
        }

        private IEnumerator InitializeGameFlow(int langId, string catId)
        {
            questionFeedbackText.text = "Loading Challenges...";
            
            // Safety Check: Make sure Supabase is in the scene
            if (SupabaseManager.Instance == null || SupabaseManager.Instance.client == null)
            {
                Debug.LogError("[2T1L] Cannot connect! You are missing the 'SupabaseManager' object in this scene. Please add your Supabase prefab or start from the Main Menu.");
                questionFeedbackText.text = "Error: No Database Connection";
                yield break;
            }

            // 1. Fetch Vocabulary for the category
            var vocabResponse = SupabaseManager.Instance.client
                .From<VocabularyTranslationModel>()
                .Filter("language_id", Postgrest.Constants.Operator.Equals, langId)
                .Get();
            
            yield return new WaitUntil(() => vocabResponse.IsCompleted);
            var words = vocabResponse.Result.Models.Where(t => t.Vocabulary != null && t.Vocabulary.CategoryId == catId).ToList();

            if (words.Count == 0)
            {
                Debug.LogError("[2T1L] No words found for category!");
                yield break;
            }

            // 2. Fetch all challenges for these words
            var challengeResponse = SupabaseManager.Instance.client
                .From<TruthsAndLiesChallengeModel>()
                .Get();
            
            yield return new WaitUntil(() => challengeResponse.IsCompleted);
            var allChallenges = challengeResponse.Result.Models;

            // 3. Build 10 Rounds (Cycling through words)
            gameRounds.Clear();
            for (int i = 0; i < totalRounds; i++)
            {
                var word = words[i % words.Count];
                var wordChallenges = allChallenges.Where(c => c.VocabularyId == word.VocabularyId).ToList();
                
                var truths = wordChallenges.Where(c => !c.IsLie).OrderBy(x => Random.value).ToList();
                var lies = wordChallenges.Where(c => c.IsLie).OrderBy(x => Random.value).ToList();

                if (truths.Count >= 2 && lies.Count >= 1)
                {
                    gameRounds.Add(new TruthsAndLiesRoundData {
                        TargetWord = word,
                        Truths = truths.Take(5).ToList(), // Keep a pool
                        Lies = lies.Take(5).ToList()
                    });
                }
            }

            if (gameRounds.Count > 0) StartRound(0);
        }

        private void StartRound(int index)
        {
            if (index >= gameRounds.Count)
            {
                ShowResult(true);
                return;
            }

            currentRound = index + 1;
            currentRoundData = gameRounds[index];
            
            // 1. Setup UI for QUIZ TIME
            choicesGroup.SetActive(true);
            speechPracticeGroup.SetActive(false);
            
            tipText.text = "Tip: Think carefully! One of these is not true...";
            questionFeedbackText.text = "Which statement is false?";
            roundProgressText.text = $"{currentRound}/{totalRounds}";
            wordPhraseText.text = currentRoundData.TargetWord.TranslatedText;

            // 2. Setup Choices
            List<TruthsAndLiesChallengeModel> roundChoices = new List<TruthsAndLiesChallengeModel>();
            roundChoices.Add(currentRoundData.Lies[Random.Range(0, currentRoundData.Lies.Count)]);
            
            var pickedTruths = currentRoundData.Truths.OrderBy(x => Random.value).Take(2).ToList();
            roundChoices.AddRange(pickedTruths);

            // Shuffle them
            var shuffled = roundChoices.OrderBy(x => Random.value).ToList();
            
            for (int i = 0; i < 3; i++)
            {
                choiceTexts[i].text = shuffled[i].ContentText;
                choiceFrames[i].color = Color.white;
                checkOrWrongIcons[i].sprite = invisibleSprite;
                
                if (shuffled[i].IsLie) correctLieIndex = i;

                // Reset button interactions
                int choiceIndex = i;
                Button btn = choiceFrames[i].GetComponent<Button>();
                if (btn == null) btn = choiceFrames[i].gameObject.AddComponent<Button>(); // Auto-add if missing
                
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => OnChoiceSelected(choiceIndex));
                btn.interactable = true;
            }
        }

        private void OnChoiceSelected(int index)
        {
            // Lock buttons
            foreach (var frame in choiceFrames) frame.GetComponent<Button>().interactable = false;

            if (index == correctLieIndex)
            {
                // CORRECT! (Found the lie)
                correctGuesses++;
                choiceFrames[index].color = Color.green;
                checkOrWrongIcons[index].sprite = checkmarkSprite;
                StartCoroutine(TransitionToSpeech());
            }
            else
            {
                // WRONG! (Clicked a truth)
                choiceFrames[index].color = Color.red;
                checkOrWrongIcons[index].sprite = xMarkSprite;
                
                // Show the right answer
                choiceFrames[correctLieIndex].color = Color.green;
                
                LoseLife();
            }
        }

        private void LoseLife()
        {
            currentLives--;
            UpdateHeartsUI();

            if (currentLives <= 0)
            {
                StartCoroutine(WaitAndShowLose());
            }
            else
            {
                StartCoroutine(WaitAndNextRound());
            }
        }

        private IEnumerator WaitAndShowLose()
        {
            yield return new WaitForSeconds(feedbackDelay);
            ShowResult(false);
        }

        private IEnumerator WaitAndNextRound()
        {
            yield return new WaitForSeconds(feedbackDelay);
            if (currentRound >= totalRounds) ShowResult(true);
            else StartRound(currentRound); // currentRound is already index + 1
        }

        private IEnumerator TransitionToSpeech()
        {
            yield return new WaitForSeconds(feedbackDelay);
            
            choicesGroup.SetActive(false);
            speechPracticeGroup.SetActive(true);
            
            tipText.text = "Tip: Speak Clearly and at a Normal Pace";
            questionFeedbackText.text = "Great! Now say the phrase!";
            
            // Reset Speech UI
            transcriptionText.text = "";
            accuracyScoreValue.text = "";
            if (correctIndicator != null) correctIndicator.gameObject.SetActive(false);
            if (micButtonImage != null) micButtonImage.sprite = micOffSprite;
            SetVisualizers(false);
        }

        private bool _isProcessing = false;

        public void OnMicClicked()
        {
            if (_isProcessing) return;

            isListening = !isListening;
            if (micButtonImage != null) micButtonImage.sprite = isListening ? micOnSprite : micOffSprite;
            SetVisualizers(isListening); // Ensure they are turned on/off

            if (isListening)
            {
                // Start Recording
                SpeechRecorder.Instance.StartRecording();
                StartCoroutine(AnimateVisualizers());
            }
            else
            {
                // Stop Recording and Process
                string path = SpeechRecorder.Instance.StopRecording();
                if (!string.IsNullOrEmpty(path)) ProcessSpeech(path);
            }
        }

        private IEnumerator AnimateVisualizers()
        {
            while (isListening)
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
            if (micButton != null) micButton.interactable = false;
            transcriptionText.text = "Processing...";

            string targetHint = currentRoundData.TargetWord.TranslatedText;
            GroqWhisperManager.Instance.Transcribe(path, OnTranscriptionSuccess, OnTranscriptionError, targetHint);
        }

        private void OnTranscriptionSuccess(string result)
        {
            _isProcessing = false;
            if (micButton != null) micButton.interactable = true;
            
            transcriptionText.text = result;
            float score = PhraseEvaluator.Instance.CalculateAccuracy(result, currentRoundData.TargetWord.TranslatedText) / 100f;
            
            accuracyScoreValue.text = (score * 100f).ToString("0") + "%";
            
            if (score >= passThreshold)
            {
                // PASS!
                accuracyScoreValue.color = new Color(0, 0.5f, 0); // Dark Green
                StartCoroutine(HandleSpeechSuccess());
            }
            else
            {
                // FAIL
                accuracyScoreValue.color = new Color(0.7f, 0, 0); // Dark Red
                questionFeedbackText.text = encouragementPhrases[Random.Range(0, encouragementPhrases.Length)];
            }
        }

        private void OnTranscriptionError(string err)
        {
            _isProcessing = false;
            if (micButton != null) micButton.interactable = true;
            transcriptionText.text = "Error! Try again.";
        }

        private IEnumerator HandleSpeechSuccess()
        {
            string successMsg = (currentRound == totalRounds) ? "You did so well!" : "Wonderful! Onto the next one!";
            questionFeedbackText.text = successMsg;
            if (correctIndicator != null) correctIndicator.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(feedbackDelay);
            
            if (currentRound < totalRounds)
            {
                StartRound(currentRound);
            }
            else
            {
                ShowResult(true);
            }
        }

        private void UpdateHeartsUI()
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i].sprite = (i < currentLives) ? heartActive : heartInactive;
            }
        }

        private void SetVisualizers(bool active)
        {
            foreach (var v in voiceVisualizers) v.SetActive(active);
            // Optionally add an animation loop for visualizers here
        }

        // --- Result Logic ---

        private void ShowResult(bool isWin)
        {
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(true);
            if (youWinPanel != null) youWinPanel.SetActive(isWin);
            if (youLosePanel != null) youLosePanel.SetActive(!isWin);
            
            float percentage = ((float)correctGuesses / totalRounds) * 100f;
            string matchString = $"{correctGuesses}/{totalRounds}";
            string scoreString = $"{percentage:F0}%";
            
            if (isWin)
            {
                if (winWordsMatchText != null) winWordsMatchText.text = matchString;
                if (winScoreText != null) winScoreText.text = scoreString;
            }
            else
            {
                if (loseWordsMatchText != null) loseWordsMatchText.text = matchString;
                if (loseScoreText != null) loseScoreText.text = scoreString;
            }
        }

        // Hook this to the "Back" button and the "Continue" button
        public void OnCloseGameClicked()
        {
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(false);
            
            if (MinigameManager.Instance != null)
            {
                MinigameManager.Instance.HideMinigame();
            }
            else if (mainGameContainer != null)
            {
                mainGameContainer.SetActive(false);
            }
        }

        // Hook this to the "Try Again" button
        public void OnTryAgainClicked()
        {
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(false);
            StartGame(currentLangId, currentCatId);
        }

        // --- Help Panel Logic ---

        public void ToggleHelp(bool show)
        {
            Debug.Log($"[2T1L] ToggleHelp called: {show}");
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
                    Debug.Log("[2T1L] Help Panel forced closed.");
                }
            }
            else
            {
                Debug.LogError("[2T1L] ToggleHelp failed: helpBackgroundPanel is not assigned!");
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
    }
}

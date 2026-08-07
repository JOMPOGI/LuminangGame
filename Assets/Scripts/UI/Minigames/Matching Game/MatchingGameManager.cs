using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luminang.UI.Minigames
{
    public class MatchingGameManager : MonoBehaviour
    {
        [Header("Data (Offline/Testing)")]
        public MatchingLessonData lessonData;
        public bool useDatabase = true;
        public string initialCategoryId; // Set these in inspector or from a menu
        public int initialLanguageId = 1; 

        [Header("Prefabs")]
        public GameObject leftCardPrefab;
        public GameObject rightCardPrefab;
        public GameObject linePrefab; // Prefab with BezierLineRenderer component

        [Header("Containers")]
        public Transform leftContainer;
        public Transform rightContainer;
        public Transform lineContainer;

        [Header("Settings")]
        public Color successLineColor = new Color(0.09f, 0.18f, 0.33f); // #172f53
        public Color failLineColor = Color.red;
        public VoiceVisualizer voiceVisualizer;

        [Header("Lives")]
        public int maxLives = 3;
        public UnityEngine.UI.Image[] heartImages;
        public Sprite heartActiveSprite;
        public Sprite heartInactiveSprite;

        // UI Panels for end-of-game feedback
        [Header("UI Panels")]
        public GameObject resultBackgroundPanel; // The translucent black overlay
        public GameObject youWinResult;          // Child inside the overlay
        public GameObject youLoseResult;         // Child inside the overlay
        public GameObject helpBackgroundPanel;   // The Help/Instructions overlay (translucent black)
        public GameObject helpPanel;             // The actual Help Panel child to scale up
        public GameObject matchingGamePanel;      // The main container for the game UI

        [Header("Transition Settings")]
        public float overlayFadeDuration = 0.4f;
        public float panelScaleDuration = 0.35f;
        public float gamePanelTransitionDuration = 0.4f;

        private int currentLives;
        private List<MatchingCard> leftCards = new List<MatchingCard>();
        private List<MatchingCard> rightCards = new List<MatchingCard>();
        private MatchingCard currentSelectedLeft;
        
        private async void Start()
        {
            if (useDatabase)
            {
                // If spawned dynamically by MinigameManager, fetch the real Category UUID!
                if (MinigameManager.Instance != null && !string.IsNullOrEmpty(MinigameManager.Instance.CurrentCategory))
                {
                    initialCategoryId = MinigameManager.Instance.CurrentCategory;
                    initialLanguageId = MinigameManager.Instance.CurrentLanguageId;
                    Debug.Log($"[MatchingGame] Spawned via MinigameManager! Fetching UUID for category: {initialCategoryId} (Lang: {initialLanguageId})...");
                }

                if (!string.IsNullOrEmpty(initialCategoryId))
                    await LoadLesson(initialCategoryId, initialLanguageId);
            }
            else if (lessonData != null)
            {
                InitializeGame();
            }

            // Start the entrance animation
            ShowGamePanel();

            // Show instructions at the very start
            ToggleHelpPanel(true);
        }

        /// <summary>
        /// Call this from any other script to change the current lesson!
        /// </summary>
        public async Task LoadLesson(string categoryId, int languageId)
        {
            initialCategoryId = categoryId;
            initialLanguageId = languageId;
            await InitializeFromDatabase(categoryId, languageId);
        }

        public async Task InitializeFromDatabase(string categoryId, int languageId)
        {
            // Clear existing
            ClearGame();

            var pairs = await CurriculumManager.Instance.GetMatchingPairs(categoryId, languageId);

            // Create Left Cards (Images)
            foreach (var pair in pairs)
            {
                GameObject cardObj = Instantiate(leftCardPrefab, leftContainer);
                MatchingCard card = cardObj.GetComponent<MatchingCard>();
                
                // For matching game, we'll use the illustration or icon as the clue
                // And the English term as the "word content" to be spoken
                card.Setup(pair.id, pair.englishTerm);
                
                // Load the sprite from your Addressables or Resources
                StartCoroutine(LoadCardSprite(card, pair.illustrationPath));
                
                leftCards.Add(card);
            }

            // Create Right Cards (Words) - Shuffle them
            var shuffledPairs = pairs.OrderBy(x => Random.value).ToList();
            foreach (var pair in shuffledPairs)
            {
                GameObject cardObj = Instantiate(rightCardPrefab, rightContainer);
                MatchingCard card = cardObj.GetComponent<MatchingCard>();
                
                // The right side shows the translated text (e.g., "Naimbag a bigat")
                card.Setup(pair.id, pair.translatedText);
                rightCards.Add(card);
            }

            // NEW: Reset lives
            ResetLives();

            // Force scroll view to top and refresh layout
            StartCoroutine(ResetScrollAndLayout());
        }

        private System.Collections.IEnumerator ResetScrollAndLayout()
        {
            // Wait a tiny bit for the Layout Groups to see the new cards
            yield return new WaitForEndOfFrame();
            
            // Force the layout to recalculate
            if (leftContainer is RectTransform rtLeft) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rtLeft);
            if (rightContainer is RectTransform rtRight) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rtRight);
            if (leftContainer.parent is RectTransform rtContent) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rtContent);

            yield return new WaitForEndOfFrame();

            // Snap to top
            if (leftContainer.parent.parent.TryGetComponent<UnityEngine.UI.ScrollRect>(out var scroll))
            {
                scroll.verticalNormalizedPosition = 1f;
            }
        }

        private void ClearGame()
        {
            foreach (Transform child in leftContainer) Destroy(child.gameObject);
            foreach (Transform child in rightContainer) Destroy(child.gameObject);
            foreach (Transform child in lineContainer) Destroy(child.gameObject);

            leftCards.Clear();
            rightCards.Clear();
        }

        private System.Collections.IEnumerator LoadCardSprite(MatchingCard card, string url)
        {
            if (string.IsNullOrEmpty(url)) yield break;

            // Check if it's a URL or a local path
            if (url.StartsWith("http"))
            {
                using (UnityEngine.Networking.UnityWebRequest loader = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
                {
                    yield return loader.SendWebRequest();

                    if (loader.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                    {
                        Texture2D texture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(loader);
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        card.Setup(card.pairID, card.wordContent, sprite);
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to download image from {url}: {loader.error}");
                    }
                }
            }
            else
            {
                // Fallback to local Resources if it's just a path
                ResourceRequest request = Resources.LoadAsync<Sprite>(url);
                yield return request;
                if (request.asset != null)
                {
                    card.Setup(card.pairID, card.wordContent, (Sprite)request.asset);
                }
            }
        }

        public void InitializeGame()
        {
            // Clear existing
            foreach (Transform child in leftContainer) Destroy(child.gameObject);
            foreach (Transform child in rightContainer) Destroy(child.gameObject);
            foreach (Transform child in lineContainer) Destroy(child.gameObject);

            leftCards.Clear();
            rightCards.Clear();

            // NEW: Reset lives
            ResetLives();

            // Create Left Cards (Images)
            foreach (var pair in lessonData.pairs)
            {
                GameObject cardObj = Instantiate(leftCardPrefab, leftContainer);
                MatchingCard card = cardObj.GetComponent<MatchingCard>();
                card.Setup(pair.pairID, pair.correctWord, pair.image);
                leftCards.Add(card);
            }

            // Create Right Cards (Words) - Shuffle them
            var shuffledPairs = lessonData.pairs.OrderBy(x => Random.value).ToList();
            foreach (var pair in shuffledPairs)
            {
                GameObject cardObj = Instantiate(rightCardPrefab, rightContainer);
                MatchingCard card = cardObj.GetComponent<MatchingCard>();
                card.Setup(pair.pairID, pair.correctWord);
                rightCards.Add(card);
            }
        }

        public void OnCardClicked(MatchingCard card)
        {
            if (card.side == MatchingCard.CardSide.Left)
            {
                if (currentSelectedLeft == card)
                {
                    // Deselect if already selected
                    currentSelectedLeft.SetSelected(false);
                    currentSelectedLeft = null;

                    // FIX: Disable mic when nothing is selected
                    if (voiceVisualizer != null) voiceVisualizer.SetReady(false);
                }
                else
                {
                    // Select new card
                    if (currentSelectedLeft != null) currentSelectedLeft.SetSelected(false);
                    currentSelectedLeft = card;
                    currentSelectedLeft.SetSelected(true);
                    
                    // NEW: Enable Mic when card is picked
                    if (voiceVisualizer != null) voiceVisualizer.SetReady(true);
                }
            }
            else
            {
                // If we click a right card, check if we have a left one selected
                if (currentSelectedLeft != null)
                {
                    TryMatch(currentSelectedLeft, card);
                }
            }
        }

        /// <summary>
        /// This is the trigger for your Speech to Text.
        /// Call this when the player speaks a word.
        /// </summary>
        public void ProcessVoiceInput(string spokenText)
        {
            if (currentSelectedLeft == null) return;

            // Find a right card that matches the spoken text
            MatchingCard targetCard = rightCards.FirstOrDefault(c => 
                !c.IsMatched && 
                c.wordContent.Equals(spokenText, System.StringComparison.OrdinalIgnoreCase)
            );

            if (targetCard != null)
            {
                TryMatch(currentSelectedLeft, targetCard);
            }
            else
            {
                // Optionally show a "no match found" feedback
                Debug.Log($"No match for: {spokenText}");
            }
        }

        private void TryMatch(MatchingCard left, MatchingCard right)
        {
            bool isCorrect = left.pairID == right.pairID;

            if (isCorrect)
            {
                CreateConnection(left, right, successLineColor);
                left.SetMatched(true);
                right.SetMatched(true);
                currentSelectedLeft = null;

                // NEW: Disable mic until another card is picked
                if (voiceVisualizer != null) voiceVisualizer.SetReady(false);

                // Check for win condition after a correct match
                if (rightCards.All(c => c.IsMatched))
                {
                    ShowWinPanel();
                }
            }
            else
            {
                // Create a temporary red line that disappears
                StartCoroutine(TemporaryFailConnection(left, right));
                left.SetMatched(false);
                right.SetMatched(false);

                // NEW: Lose a life on mistake
                LoseLife();
            }
        }

        private void ResetLives()
        {
            currentLives = maxLives;
            for (int i = 0; i < heartImages.Length; i++)
            {
                if (heartImages[i] != null) heartImages[i].sprite = heartActiveSprite;
            }
        }

        private void LoseLife()
        {
            currentLives--;
            
            // Update heart UI
            if (currentLives >= 0 && currentLives < heartImages.Length)
            {
                // We deactivate hearts from right to left
                int heartIndex = currentLives; 
                if (heartImages[heartIndex] != null) heartImages[heartIndex].sprite = heartInactiveSprite;
            }

            if (currentLives <= 0)
            {
                Debug.Log("GAME OVER!");
                ShowGameOverPanel();
            }
            else
            {
                // You can trigger a Game Over panel here later!
            }
        }

        private void CreateConnection(MatchingCard left, MatchingCard right, Color color)
        {
            GameObject lineObj = Instantiate(linePrefab, lineContainer);
            BezierLineRenderer line = lineObj.GetComponent<BezierLineRenderer>();
            line.color = color;
            line.SetPoints(left.connectionPoint, right.connectionPoint);
        }

        private System.Collections.IEnumerator TemporaryFailConnection(MatchingCard left, MatchingCard right)
        {
            GameObject lineObj = Instantiate(linePrefab, lineContainer);
            BezierLineRenderer line = lineObj.GetComponent<BezierLineRenderer>();
            line.color = failLineColor;
            line.SetPoints(left.connectionPoint, right.connectionPoint);
            
            yield return new WaitForSeconds(1f);
            
            Destroy(lineObj);
        }

        // ------------------- UI Helper Methods -------------------
        private void ShowGameOverPanel()
        {
            if (resultBackgroundPanel != null)
                StartCoroutine(ShowResultSequence(youLoseResult, youWinResult));
        }

        private void ShowWinPanel()
        {
            if (resultBackgroundPanel != null)
                StartCoroutine(ShowResultSequence(youWinResult, youLoseResult));
        }

        public void ToggleHelpPanel(bool show)
        {
            if (helpBackgroundPanel != null)
            {
                if (show) StartCoroutine(ShowHelpSequence());
                else StartCoroutine(HideHelpSequence());
            }
        }

        private IEnumerator HideHelpSequence()
        {
            CanvasGroup group = helpBackgroundPanel.GetComponent<CanvasGroup>();
            float elapsed = 0f;

            while (elapsed < panelScaleDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / panelScaleDuration);
                
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

        // --- Main Game Panel Transitions ---
        public void ShowGamePanel()
        {
            if (matchingGamePanel != null)
                StartCoroutine(TransitionGamePanel(true));
        }

        public void HideGamePanel()
        {
            if (matchingGamePanel != null)
                StartCoroutine(TransitionGamePanel(false));
        }

        public void OnContinueClicked()
        {
            HideGamePanel();
        }

        public void OnBackButtonClicked()
        {
            HideGamePanel();
        }

        public async void OnTryAgainClicked()
        {
            // Hide result panel
            if (resultBackgroundPanel != null) resultBackgroundPanel.SetActive(false);

            // Restart game based on mode
            if (useDatabase)
            {
                await LoadLesson(initialCategoryId, initialLanguageId);
            }
            else
            {
                InitializeGame();
            }
        }

        private IEnumerator TransitionGamePanel(bool appearing)
        {
            CanvasGroup group = matchingGamePanel.GetComponent<CanvasGroup>();
            if (group == null) group = matchingGamePanel.AddComponent<CanvasGroup>();

            float startAlpha = appearing ? 0f : 1f;
            float endAlpha = appearing ? 1f : 0f;
            Vector3 startScale = appearing ? Vector3.one * 0.9f : Vector3.one;
            Vector3 endScale = appearing ? Vector3.one : Vector3.one * 0.9f;

            group.alpha = startAlpha;
            matchingGamePanel.transform.localScale = startScale;
            matchingGamePanel.SetActive(true);

            float elapsed = 0f;
            while (elapsed < gamePanelTransitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / gamePanelTransitionDuration);
                
                // Ease-out cubic
                float eased = appearing ? (1f - Mathf.Pow(1f - t, 3f)) : Mathf.Pow(t, 3f);
                
                group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                matchingGamePanel.transform.localScale = Vector3.Lerp(startScale, endScale, appearing ? eased : t);
                
                yield return null;
            }

            group.alpha = endAlpha;
            matchingGamePanel.transform.localScale = endScale;

            if (!appearing)
            {
                matchingGamePanel.SetActive(false);
                // Optionally trigger a scene change or return to menu here
                Debug.Log("Game Panel Hidden - Return to Menu logic can go here");
            }
        }

        private IEnumerator ShowHelpSequence()
        {
            // Initial state
            if (helpPanel != null)
            {
                helpPanel.SetActive(false);
                helpPanel.transform.localScale = Vector3.zero;
            }

            // Ensure CanvasGroup for fading
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

        private IEnumerator ShowResultSequence(GameObject toShow, GameObject toHide)
        {
            // Hide both result panels first
            if (toHide != null) toHide.SetActive(false);
            if (toShow != null)
            {
                toShow.SetActive(false);
                toShow.transform.localScale = Vector3.zero;
            }

            // Ensure the overlay has a CanvasGroup for fading
            CanvasGroup overlayGroup = resultBackgroundPanel.GetComponent<CanvasGroup>();
            if (overlayGroup == null)
                overlayGroup = resultBackgroundPanel.AddComponent<CanvasGroup>();

            // --- Phase 1: Fade in the translucent overlay ---
            overlayGroup.alpha = 0f;
            resultBackgroundPanel.SetActive(true);

            float elapsed = 0f;
            while (elapsed < overlayFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                overlayGroup.alpha = Mathf.Clamp01(elapsed / overlayFadeDuration);
                yield return null;
            }
            overlayGroup.alpha = 1f;

            // --- Phase 2: Scale up the result panel (smooth ease-out, no bounce) ---
            if (toShow != null)
            {
                toShow.SetActive(true);
                elapsed = 0f;
                while (elapsed < panelScaleDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float t = Mathf.Clamp01(elapsed / panelScaleDuration);
                    // Ease-out cubic: fast start, smooth deceleration
                    float eased = 1f - Mathf.Pow(1f - t, 3f);
                    toShow.transform.localScale = Vector3.one * eased;
                    yield return null;
                }
                toShow.transform.localScale = Vector3.one;
            }
        }
    }
}

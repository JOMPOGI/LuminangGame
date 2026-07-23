using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public enum BookTab
{
    LanguageSelect = 0,
    Journal = 1,
    Leaderboard = 2,
    Announcements = 3
}

[System.Serializable]
public class BookmarkUI
{
    [Tooltip("The actual bookmark button.")]
    public Button tabButton;
    [Tooltip("The icon child image of the bookmark button.")]
    public Image iconImage;
    [Tooltip("The text name child of the bookmark button.")]
    public TextMeshProUGUI nameText;
}

/// <summary>
/// Manages the Book-style UI HUD in LanguageSelectionScene.
/// Supports 4 tab bookmarks with smart flip direction, content group switching,
/// and external book opening/closing triggers.
/// </summary>
public class BookSelectionManager : MonoBehaviour
{
    public static BookSelectionManager Instance { get; private set; }

    [Header("Tab Content Groups")]
    [Tooltip("Assign the content group objects in order: 0 = LanguageSelect, 1 = Journal, 2 = Leaderboard, 3 = Announcements.")]
    public GameObject[] tabContentGroups;

    [Header("Tab Bookmark Buttons")]
    [Tooltip("Configuration for the 4 bookmark tabs, ordered 0 to 3.")]
    public BookmarkUI[] bookmarkButtons;

    [Header("Bookmark Colors")]
    public Color activeTabColor = Color.white;
    public Color inactiveTabColor = new Color(0.65f, 0.65f, 0.65f, 1f);
    public Color activeIconColor = Color.white;
    public Color inactiveIconColor = new Color(0.65f, 0.65f, 0.65f, 1f);
    public Color activeTextColor = Color.white;
    public Color inactiveTextColor = new Color(0.65f, 0.65f, 0.65f, 1f);

    [Header("Book Visual & Animation (Script-Based)")]
    public Image bookImage;
    public Sprite idleBookSprite;
    public Sprite[] flipSprites;
    public Sprite[] openSprites;
    public Sprite[] closeSprites;
    [Tooltip("Time (seconds) between each sprite frame of the page flip animation. Lower = faster flip, Higher = slower flip. Try 0.05–0.15.")]
    public float timePerFrame = 0.05f;

    [Header("Tab Content Fade Animation")]
    [Tooltip("How long (seconds) the content tab fades OUT when switching tabs. Lower = faster.")]
    public float tabFadeOutDuration = 0.2f;
    [Tooltip("How long (seconds) the content tab fades IN when switching tabs. Lower = faster.")]
    public float tabFadeInDuration  = 0.2f;

    [Header("Page Content CanvasGroup (for Fading)")]
    [Tooltip("CanvasGroup surrounding the inner page contents to fade out during flips.")]
    public CanvasGroup pageContentCanvasGroup;

    [Header("HUD Panels (Optional)")]
    [Tooltip("Assign the PlayerInfoPanel if you want it hidden during transitions.")]
    public PlayerInfoPanel playerInfoPanel;
    [Tooltip("Assign the HUDGroupManager if you want it hidden during transitions.")]
    public HUDGroupManager hudGroupManager;

    [Header("Language Select Specific")]
    [Tooltip("Assign the LanguageCardManager so the book knows how to close the LevelsGroup when switching tabs.")]
    public LanguageCardManager languageCardManager;

    [Header("Sub Pages (Non-Bookmarks)")]
    [Tooltip("Drag the LevelsGroup GameObject here.")]
    public GameObject levelsGroupPanel;

    // Private States
    private int _currentTabIndex = 0;
    private bool _isTransitioning = false;
    private CanvasGroup[] _tabCanvasGroups;
    private CanvasGroup _levelsCanvasGroup;
    private bool _isLevelsGroupOpen = false;
    private CanvasGroup _currentActiveCanvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (bookmarkButtons != null)
        {
            // Wire up bookmarks
            for (int i = 0; i < bookmarkButtons.Length; i++)
            {
                int index = i;
                if (bookmarkButtons[i] != null && bookmarkButtons[i].tabButton != null)
                {
                    bookmarkButtons[i].tabButton.onClick.AddListener(() => SwitchToTab(index));
                }
            }
        }

        // Initial setup
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Set idle sprite
        if (bookImage != null && idleBookSprite != null)
            bookImage.sprite = idleBookSprite;

        // Auto-cache or add CanvasGroup on each tab content group
        if (tabContentGroups != null)
        {
            _tabCanvasGroups = new CanvasGroup[tabContentGroups.Length];
            for (int i = 0; i < tabContentGroups.Length; i++)
            {
                if (tabContentGroups[i] == null) continue;

                // Ensure all groups are active so CanvasGroup alpha can be animated
                tabContentGroups[i].SetActive(true);

                // Get or auto-add a CanvasGroup
                _tabCanvasGroups[i] = tabContentGroups[i].GetComponent<CanvasGroup>();
                if (_tabCanvasGroups[i] == null)
                    _tabCanvasGroups[i] = tabContentGroups[i].AddComponent<CanvasGroup>();

                // Only index 0 visible at start; rest hidden
                bool isFirst = (i == 0);
                _tabCanvasGroups[i].alpha = isFirst ? 1f : 0f;
                _tabCanvasGroups[i].interactable = isFirst;
                _tabCanvasGroups[i].blocksRaycasts = isFirst;
            }
        }

        if (levelsGroupPanel != null)
        {
            levelsGroupPanel.SetActive(true); // Ensure it's active so CanvasGroup fading works
            
            _levelsCanvasGroup = levelsGroupPanel.GetComponent<CanvasGroup>();
            if (_levelsCanvasGroup == null) _levelsCanvasGroup = levelsGroupPanel.AddComponent<CanvasGroup>();
            
            _levelsCanvasGroup.alpha = 0f;
            _levelsCanvasGroup.interactable = false;
            _levelsCanvasGroup.blocksRaycasts = false;
        }

        UpdateTabButtonColors(0);
        _currentTabIndex = 0;
        _currentActiveCanvas = _tabCanvasGroups != null && _tabCanvasGroups.Length > 0 ? _tabCanvasGroups[0] : null;
        _isLevelsGroupOpen = false;
    }

    public void OpenLevelsGroup()
    {
        if (_isTransitioning || _isLevelsGroupOpen || _levelsCanvasGroup == null) return;
        
        _isLevelsGroupOpen = true;
        CanvasGroup oldCanvas = _currentActiveCanvas;
        _currentActiveCanvas = _levelsCanvasGroup;

        // Flip forward from LanguagesGroup to LevelsGroup
        StartCoroutine(FlipAndSwapCanvas(oldCanvas, _levelsCanvasGroup, true, _currentTabIndex));
    }

    public void SwitchToTab(int targetIndex)
    {
        if (_isTransitioning) return;

        if (targetIndex == _currentTabIndex)
        {
            // If clicking LanguageSelect bookmark while inside LevelsGroup -> Flip back
            if (_isLevelsGroupOpen && targetIndex == 0)
            {
                _isLevelsGroupOpen = false;
                CanvasGroup canvasToClose = _currentActiveCanvas;
                _currentActiveCanvas = _tabCanvasGroups[0];

                UpdateTabButtonColors(0);
                // Flip reverse from LevelsGroup back to LanguagesGroup
                StartCoroutine(FlipAndSwapCanvas(canvasToClose, _currentActiveCanvas, false, 0));
            }
            return;
        }

        // Switching to a totally different tab (e.g. from LevelsGroup to Journal)
        _isLevelsGroupOpen = false;
        CanvasGroup oldCanvas = _currentActiveCanvas;
        CanvasGroup newCanvas = _tabCanvasGroups[targetIndex];
        _currentActiveCanvas = newCanvas;

        bool forward = targetIndex > _currentTabIndex;
        StartCoroutine(FlipAndSwapCanvas(oldCanvas, newCanvas, forward, targetIndex));
    }

    private IEnumerator FlipAndSwapCanvas(CanvasGroup oldGroup, CanvasGroup newGroup, bool forward, int targetBookmarkIndex)
    {
        _isTransitioning = true;

        if (bookImage != null && flipSprites != null && flipSprites.Length > 0)
        {
            int frameCount = flipSprites.Length;
            int midpoint = frameCount / 2;

            // 1. First half: play flip frames + fade OUT old tab group
            float fadeOutElapsed = 0f;
            float fadeOutTotal = tabFadeOutDuration > 0f ? tabFadeOutDuration : (midpoint * timePerFrame);

            if (forward)
            {
                for (int i = 0; i < midpoint; i++)
                {
                    bookImage.sprite = flipSprites[i];
                    fadeOutElapsed += timePerFrame;
                    float t = Mathf.Clamp01(fadeOutElapsed / fadeOutTotal);
                    SetCanvasGroupAlpha(oldGroup, 1f - t); // fade out
                    yield return new WaitForSeconds(timePerFrame);
                }
            }
            else
            {
                for (int i = frameCount - 1; i >= midpoint; i--)
                {
                    bookImage.sprite = flipSprites[i];
                    fadeOutElapsed += timePerFrame;
                    float t = Mathf.Clamp01(fadeOutElapsed / fadeOutTotal);
                    SetCanvasGroupAlpha(oldGroup, 1f - t); // fade out
                    yield return new WaitForSeconds(timePerFrame);
                }
            }

            // 2. Midpoint: fully hide old, prepare new at alpha 0
            SetCanvasGroupAlpha(oldGroup, 0f);
            SetCanvasGroupVisible(oldGroup, false);
            SetCanvasGroupAlpha(newGroup, 0f);
            SetCanvasGroupVisible(newGroup, true);

            // 3. Update tab state
            UpdateTabButtonColors(targetBookmarkIndex);
            _currentTabIndex = targetBookmarkIndex;

            // 4. Second half: play flip frames + fade IN new tab group
            float fadeInElapsed = 0f;
            float fadeInTotal = tabFadeInDuration > 0f ? tabFadeInDuration : (midpoint * timePerFrame);

            if (forward)
            {
                for (int i = midpoint; i < frameCount; i++)
                {
                    bookImage.sprite = flipSprites[i];
                    fadeInElapsed += timePerFrame;
                    float t = Mathf.Clamp01(fadeInElapsed / fadeInTotal);
                    SetCanvasGroupAlpha(newGroup, t); // fade in
                    yield return new WaitForSeconds(timePerFrame);
                }
            }
            else
            {
                for (int i = midpoint - 1; i >= 0; i--)
                {
                    bookImage.sprite = flipSprites[i];
                    fadeInElapsed += timePerFrame;
                    float t = Mathf.Clamp01(fadeInElapsed / fadeInTotal);
                    SetCanvasGroupAlpha(newGroup, t); // fade in
                    yield return new WaitForSeconds(timePerFrame);
                }
            }

            // 5. Snap to fully visible and return book to idle
            SetCanvasGroupAlpha(newGroup, 1f);
            if (idleBookSprite != null)
                bookImage.sprite = idleBookSprite;
        }
        else
        {
            // Fallback: smooth crossfade with no flip sprites
            float fadeElapsed = 0f;

            while (fadeElapsed < tabFadeOutDuration)
            {
                fadeElapsed += Time.deltaTime;
                SetCanvasGroupAlpha(oldGroup, 1f - Mathf.Clamp01(fadeElapsed / tabFadeOutDuration));
                yield return null;
            }
            SetCanvasGroupAlpha(oldGroup, 0f);
            SetCanvasGroupVisible(oldGroup, false);

            UpdateTabButtonColors(targetBookmarkIndex);
            _currentTabIndex = targetBookmarkIndex;
            
            SetCanvasGroupAlpha(newGroup, 0f);
            SetCanvasGroupVisible(newGroup, true);

            fadeElapsed = 0f;
            while (fadeElapsed < tabFadeInDuration)
            {
                fadeElapsed += Time.deltaTime;
                SetCanvasGroupAlpha(newGroup, Mathf.Clamp01(fadeElapsed / tabFadeInDuration));
                yield return null;
            }
            SetCanvasGroupAlpha(newGroup, 1f);
        }

        _isTransitioning = false;
    }

    private void SetCanvasGroupAlpha(CanvasGroup cg, float alpha)
    {
        if (cg != null) cg.alpha = alpha;
    }

    private void SetCanvasGroupVisible(CanvasGroup cg, bool visible)
    {
        if (cg != null)
        {
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
    }

    private void UpdateTabButtonColors(int activeIndex)
    {
        if (bookmarkButtons == null) return;

        for (int i = 0; i < bookmarkButtons.Length; i++)
        {
            var bookmark = bookmarkButtons[i];
            if (bookmark == null || bookmark.tabButton == null) continue;

            bool isActive = (i == activeIndex);

            // 1. Color the Bookmark Background button image
            var btnImg = bookmark.tabButton.GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.color = isActive ? activeTabColor : inactiveTabColor;
            }

            // 2. Color the Icon child
            if (bookmark.iconImage != null)
            {
                bookmark.iconImage.color = isActive ? activeIconColor : inactiveIconColor;
            }

            // 3. Color the Text Name child
            if (bookmark.nameText != null)
            {
                bookmark.nameText.color = isActive ? activeTextColor : inactiveTextColor;
            }
        }
    }

    private void SetContentAlpha(float alpha)
    {
        if (pageContentCanvasGroup != null)
            pageContentCanvasGroup.alpha = alpha;
    }

    // =====================================================
    // External Book Open / Close Trigger Animations
    // =====================================================

    public void OpenBook()
    {
        if (_isTransitioning) return;
        StartCoroutine(PlaySequentialSprites(openSprites, idleBookSprite));
    }

    public void CloseBook()
    {
        if (_isTransitioning) return;
        StartCoroutine(PlaySequentialSprites(closeSprites, null));
    }

    private IEnumerator PlaySequentialSprites(Sprite[] animationSprites, Sprite finalSprite)
    {
        if (bookImage == null || animationSprites == null || animationSprites.Length == 0) yield break;
        _isTransitioning = true;

        for (int i = 0; i < animationSprites.Length; i++)
        {
            bookImage.sprite = animationSprites[i];
            yield return new WaitForSeconds(timePerFrame);
        }

        if (finalSprite != null)
            bookImage.sprite = finalSprite;

        _isTransitioning = false;
    }

    // =====================================================
    // Start Game Sequence
    // =====================================================

    public void StartLanguage(RegionMode mode)
    {
        if (_isTransitioning) return;

        // Persist language selection in PhraseEvaluator
        if (PhraseEvaluator.Instance != null)
            PhraseEvaluator.Instance.SetRegion(mode);

        Debug.Log($"[BookUI] Stored region: {mode}. Proceeding...");
        StartCoroutine(TransitionToGame());
    }

    private IEnumerator TransitionToGame()
    {
        _isTransitioning = true;

        // Hide any general HUD elements
        if (playerInfoPanel != null) playerInfoPanel.Hide();
        if (hudGroupManager != null) hudGroupManager.Hide();

        // Overlay Cloud Transition
        if (MapTransitionManager.Instance != null)
        {
            MapTransitionManager.Instance.CloseMap();
            float waitTime = MapTransitionManager.Instance.transitionDuration + MapTransitionManager.Instance.staggerStrength;
            yield return new WaitForSeconds(waitTime);
        }

        // Scene Routing
        string sceneToLoad = "TutorialScene";
        if (UserProfileManager.Instance?.CurrentProfile != null)
        {
            if (UserProfileManager.Instance.CurrentProfile.HasCompletedTutorial)
            {
                sceneToLoad = "SampleScene";
            }
        }

        var loader = FindFirstObjectByType<SceneLoader>();
        if (loader != null) loader.LoadScene(sceneToLoad);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}

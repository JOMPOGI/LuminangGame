using UnityEngine;
using System.Collections;

/// <summary>
/// The central authority for Gameplay HUD visibility.
/// Use this to hide/show the HUD for Lessons, Mini-games, or Cutscenes.
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Gameplay HUD Elements")]
    [Tooltip("The movement joysticks and sprint buttons.")]
    public GameObject movementControls;
    [Tooltip("The HUD panel showing player gold and profile.")]
    public GameObject playerInfoPanel;
    [Tooltip("The floating Talk button.")]
    public GameObject talkButton;
    [Tooltip("The floating Objective text.")]
    public GameObject objectiveText;
    [Tooltip("The root of the dialogue system.")]
    public GameObject dialogueRoot;

    [Header("Global Dimmer (Optional)")]
    [Tooltip("A global dimmer image that can be used by any system.")]
    public GameObject globalDimmer;
    private CanvasGroup _dimmerCG;

    [Header("Overlays to Watch")]
    [Tooltip("If this is Active, HUD will hide.")]
    public GameObject lessonPanel;
    private bool _isHiding = false;
    private bool _isInitialized = false;

    /// <summary>
    /// Returns true if the HUD is currently allowed to be visible.
    /// </summary>
    public bool IsHUDAllowed { get; private set; }

    /// <summary>
    /// Forces the HUD to reappear and kills any active dimmers/overlays.
    /// Use this as an emergency cleanup or for scene transitions.
    /// </summary>
    [ContextMenu("Emergency: Force Restore All")]
    public void ForceRestoreAll()
    {
        if (lessonPanel != null) lessonPanel.SetActive(false);
        if (DialogueManager.Instance != null && DialogueManager.Instance.uiController != null)
            DialogueManager.Instance.uiController.HideDialogue();
            
        UpdateHUDVisibility(true);
    }

    void Awake()
    {
        Instance = this;
        Initialize();
    }

    private void Initialize()
    {
        if (_isInitialized) return;

        if (globalDimmer != null)
        {
            _dimmerCG = globalDimmer.GetComponent<CanvasGroup>();
            if (_dimmerCG == null) _dimmerCG = globalDimmer.AddComponent<CanvasGroup>();
            globalDimmer.SetActive(false);
            _dimmerCG.alpha = 0f;
        }
        _isInitialized = true;
    }

    void Update()
    {
        // THE WATCHDOG LOGIC
        // If the Lesson Panel is Active, or Dialogue is Active, we MUST hide the HUD.
        bool isLessonActive = (lessonPanel != null && lessonPanel.activeInHierarchy);
        bool isDialogueActive = (DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue);

        bool shouldHideHUD = isLessonActive || isDialogueActive;
        IsHUDAllowed = !shouldHideHUD;

        UpdateHUDVisibility(IsHUDAllowed);
    }

    private void UpdateHUDVisibility(bool visible)
    {
        if (movementControls != null && movementControls.activeSelf != visible) 
            movementControls.SetActive(visible);
            
        if (playerInfoPanel != null && playerInfoPanel.activeSelf != visible) 
            playerInfoPanel.SetActive(visible);
            
        if (objectiveText != null && objectiveText.activeSelf != visible) 
            objectiveText.SetActive(visible);

        // ONLY show the Dialogue Root if we are actually in a dialogue 
        // AND not currently in a lesson.
        bool isDialogueActive = (DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue);
        bool isLessonActive = (lessonPanel != null && lessonPanel.activeInHierarchy);
        bool showDialogueRoot = isDialogueActive && !isLessonActive;

        if (dialogueRoot != null && dialogueRoot.activeSelf != showDialogueRoot)
            dialogueRoot.SetActive(showDialogueRoot);

        // If HUD is hidden, force talk button off. 
        // If HUD is allowed, let InteractionManager decide.
        if (!visible && talkButton != null && talkButton.activeSelf)
            talkButton.SetActive(false);

        // Sync Dimmer - ONLY show dimmer if the LESSON is active. 
        if (isLessonActive && globalDimmer != null && !globalDimmer.activeSelf)
        {
            ShowDimmer(true);
        }
        else if (!isLessonActive && globalDimmer != null && globalDimmer.activeSelf && !_isHiding)
        {
            ShowDimmer(false);
        }
            
        // Sync Objective Manager
        if (ObjectiveManager.Instance != null)
        {
            if (visible) ObjectiveManager.Instance.Show();
            else ObjectiveManager.Instance.Hide();
        }

        // FORCE INPUT REFRESH (Fixes stuck camera joysticks on mobile)
        if (visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ShowDimmer(bool show, float duration = 0.3f)
    {
        if (globalDimmer == null) return;
        
        StopAllCoroutines();
        StartCoroutine(FadeDimmer(show, duration));
    }

    private IEnumerator FadeDimmer(bool show, float duration)
    {
        if (_dimmerCG == null) yield break;
        
        if (!show) _isHiding = true;

        float target = show ? 1f : 0f;
        float start = _dimmerCG.alpha;
        float elapsed = 0f;

        if (show) globalDimmer.SetActive(true);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _dimmerCG.alpha = Mathf.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        _dimmerCG.alpha = target;
        if (!show) 
        {
            globalDimmer.SetActive(false);
            _isHiding = false;
        }
    }
}

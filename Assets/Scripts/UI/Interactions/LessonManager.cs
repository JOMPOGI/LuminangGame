using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the visibility and state of the Lesson Panel.
/// Can be triggered by Dialogue Events or other game logic.
/// </summary>
public class LessonManager : MonoBehaviour
{
    public static LessonManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("The root object of the Lesson Panel prefab.")]
    public GameObject lessonPanel;
    [Tooltip("The CanvasGroup for smooth fading (optional).")]
    public CanvasGroup lessonCanvasGroup;
    
    private bool _isHiding = false;

    [Header("UI Groups to Hide")]
    [Tooltip("The root object of the entire dialogue system.")]
    public GameObject dialogueSystemRoot;
    [Tooltip("The HUD panel showing player gold and profile.")]
    public GameObject playerInfoPanel;
    [Tooltip("The floating Talk button.")]
    public GameObject talkButton;
    [Tooltip("The movement joysticks and sprint buttons.")]
    public GameObject movementControls;
    [Tooltip("The floating Objective text.")]
    public GameObject objectiveText;

    [Header("Visual Enhancements")]
    [Tooltip("How long it takes to fade in/out (seconds).")]
    public float fadeDuration = 0.4f;
    [Tooltip("A dark semi-transparent image that covers the game world during lessons.")]
    public GameObject dimmerBackground;
    
    private CanvasGroup _dimmerCG;

    void Awake()
    {
        Instance = this;
        SetupReferences();
    }

    private void SetupReferences()
    {
        if (lessonPanel != null)
        {
            lessonPanel.SetActive(false);
            if (lessonCanvasGroup == null) lessonCanvasGroup = lessonPanel.GetComponent<CanvasGroup>();
            if (lessonCanvasGroup != null) lessonCanvasGroup.alpha = 0f;
        }

        // Auto-find dimmer if not assigned
        if (dimmerBackground == null)
        {
            GameObject foundDimmer = GameObject.Find("Dimmer");
            if (foundDimmer != null) dimmerBackground = foundDimmer;
        }

        if (dimmerBackground != null)
        {
            dimmerBackground.SetActive(false);
            _dimmerCG = dimmerBackground.GetComponent<CanvasGroup>();
            if (_dimmerCG == null) _dimmerCG = dimmerBackground.AddComponent<CanvasGroup>();
            _dimmerCG.alpha = 0f;
        }
    }

    public void ShowLesson()
    {
        if (lessonPanel == null) 
        {
            Debug.LogError("[LessonManager] Cannot show lesson: Lesson Panel is not assigned!");
            return;
        }

        _isHiding = false;
        Debug.Log("[LessonManager] Showing Lesson. Cleaning HUD...");

        // Ensure dimmer is found
        if (dimmerBackground == null) SetupReferences();

        // Hide HUD
        SetHUDActive(false);

        StopAllCoroutines();
        lessonPanel.SetActive(true);
        if (dimmerBackground != null) dimmerBackground.SetActive(true);
        
        StartCoroutine(FadeRoutine(true));

        if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = false;
    }

    public void HideLesson()
    {
        Debug.Log("[LessonManager] Hiding Lesson. Restoring HUD...");
        _isHiding = true; 

        StopAllCoroutines();
        StartCoroutine(FadeRoutine(false));
    }

    private void Update()
    {
        // Continuous enforcement while lesson is active
        if (!_isHiding && lessonPanel != null && lessonPanel.activeInHierarchy)
        {
            ForceHideHUD();
        }
    }

    private void ForceHideHUD()
    {
        if (movementControls != null && movementControls.activeSelf) movementControls.SetActive(false);
        if (objectiveText != null && objectiveText.activeSelf) objectiveText.SetActive(false);
        if (playerInfoPanel != null && playerInfoPanel.activeSelf) playerInfoPanel.SetActive(false);
        if (talkButton != null && talkButton.activeSelf) talkButton.SetActive(false);
    }

    private void SetHUDActive(bool active)
    {
        Debug.Log($"[LessonManager] Setting HUD Active: {active}");
        
        if (dialogueSystemRoot != null) dialogueSystemRoot.SetActive(active);
        if (playerInfoPanel != null) playerInfoPanel.SetActive(active);
        if (talkButton != null) talkButton.SetActive(active);
        if (movementControls != null) movementControls.SetActive(active);
        if (objectiveText != null) objectiveText.SetActive(active);

        if (!active)
        {
            if (ObjectiveManager.Instance != null) ObjectiveManager.Instance.Hide();
        }
        else
        {
            if (ObjectiveManager.Instance != null) ObjectiveManager.Instance.Show();
            if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = true;
            if (dimmerBackground != null) dimmerBackground.SetActive(false);
            _isHiding = false;
        }
    }

    private IEnumerator FadeRoutine(bool show)
    {
        float targetAlpha = show ? 1f : 0f;
        float elapsed = 0f;

        float startLessonAlpha = (lessonCanvasGroup != null) ? lessonCanvasGroup.alpha : (show ? 0f : 1f);
        float startDimmerAlpha = (_dimmerCG != null) ? _dimmerCG.alpha : (show ? 0f : 1f);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;

            if (lessonCanvasGroup != null)
                lessonCanvasGroup.alpha = Mathf.Lerp(startLessonAlpha, targetAlpha, t);
            
            if (_dimmerCG != null)
                _dimmerCG.alpha = Mathf.Lerp(startDimmerAlpha, targetAlpha, t);

            yield return null;
        }

        // Final Snap
        if (lessonCanvasGroup != null) lessonCanvasGroup.alpha = targetAlpha;
        if (_dimmerCG != null) _dimmerCG.alpha = targetAlpha;

        if (!show)
        {
            SetHUDActive(true);
            lessonPanel.SetActive(false);
            Debug.Log("[LessonManager] Hide complete.");
        }
    }
}

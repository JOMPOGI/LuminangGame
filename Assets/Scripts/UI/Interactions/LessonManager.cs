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
    

    [Header("Visual Enhancements")]
    [Tooltip("How long it takes to fade in/out (seconds).")]
    public float fadeDuration = 0.4f;
    [Tooltip("A dark semi-transparent image that covers the game world during lessons.")]
    public GameObject dimmerBackground;
    
    private CanvasGroup _dimmerCG;
    private bool _isLessonActive = false;

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
        if (lessonPanel == null) return;
        
        Debug.Log("[LessonManager] ShowLesson requested.");
        _isLessonActive = true;

        StopAllCoroutines();
        lessonPanel.SetActive(true); // THE TRIGGER FOR THE WATCHDOG
        StartCoroutine(FadeRoutine(true));

        if (InteractionManager.Instance != null) InteractionManager.Instance.enabled = false;
    }

    public void HideLesson()
    {
        if (!_isLessonActive) return;
        
        Debug.Log("[LessonManager] HideLesson requested.");
        _isLessonActive = false;

        StopAllCoroutines();
        StartCoroutine(FadeRoutine(false));
    }

    private IEnumerator FadeRoutine(bool show)
    {
        float targetAlpha = show ? 1f : 0f;
        float elapsed = 0f;

        float startLessonAlpha = (lessonCanvasGroup != null) ? lessonCanvasGroup.alpha : (show ? 0f : 1f);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / fadeDuration;

            if (lessonCanvasGroup != null)
                lessonCanvasGroup.alpha = Mathf.Lerp(startLessonAlpha, targetAlpha, t);
            
            yield return null;
        }

        if (lessonCanvasGroup != null) lessonCanvasGroup.alpha = targetAlpha;

        if (!show)
        {
            lessonPanel.SetActive(false); // THE TRIGGER FOR THE WATCHDOG

            // Re-enable the interaction system so we can talk to NPCs again!
            if (InteractionManager.Instance != null) 
                InteractionManager.Instance.enabled = true;
        }
    }
}

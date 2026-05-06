using UnityEngine;
using System.Collections;

public class HUDGroupManager : MonoBehaviour
{
    [Header("Animation Settings")]
    public float slideDuration = 0.5f;
    public AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Tooltip("How far off-screen to the right the HUD should start (e.g., 500)")]
    public float offScreenX = 500f;

    [Tooltip("Match the PlayerInfoPanel delay so they appear together")]
    public float entranceDelay = 2.5f;

    private RectTransform rectTransform;
    private Vector2 targetPosition; // On-screen
    private Vector2 hiddenPosition; // Off-screen
    private Coroutine slideCoroutine;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Save the current position in the editor as our "target" on-screen position
        targetPosition = rectTransform.anchoredPosition;
        
        // Calculate the hidden position (Positive X is Right)
        hiddenPosition = new Vector2(targetPosition.x + offScreenX, targetPosition.y);
        
        // Snap off-screen immediately
        rectTransform.anchoredPosition = hiddenPosition;
    }

    private void Start()
    {
        // Animate in at the start of the scene
        StartCoroutine(DelayedShow());
    }

    private IEnumerator DelayedShow()
    {
        yield return new WaitForSeconds(entranceDelay);
        Show();
    }

    /// <summary>
    /// Slides the HUD in from the right.
    /// </summary>
    public void Show()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideRoutine(rectTransform.anchoredPosition, targetPosition));
    }

    /// <summary>
    /// Slides the HUD back off-screen to the right.
    /// </summary>
    public void Hide()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideRoutine(rectTransform.anchoredPosition, hiddenPosition));
    }

    private IEnumerator SlideRoutine(Vector2 startPos, Vector2 endPos)
    {
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(elapsed / slideDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        rectTransform.anchoredPosition = endPos;
    }
}

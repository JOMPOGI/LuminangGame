using UnityEngine;

/// <summary>
/// Makes a UI element rise up on start, then continuously bounce up and down.
/// Attach to your GameLogo_MainMenu object and remove the Animator component.
/// </summary>
public class LogoBounce : MonoBehaviour
{
    [Header("Rise Up Animation")]
    [Tooltip("Should the logo rise up from below when the scene starts?")]
    public bool playRiseOnStart = true;

    [Tooltip("How far below the logo starts (in pixels).")]
    public float riseStartOffset = -200f;

    [Tooltip("How long the rise animation takes in seconds.")]
    public float riseDuration = 0.7f;

    [Header("Bounce Animation")]
    [Tooltip("How many pixels the logo moves up and down.")]
    public float bounceHeight = 8f;

    [Tooltip("How fast the logo bounces (cycles per second).")]
    public float bounceSpeed = 0.6f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private float riseTimer = 0f;
    private bool isRising = true;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;

        if (playRiseOnStart)
        {
            // Start the logo below its original position
            rectTransform.anchoredPosition = originalPosition + new Vector2(0, riseStartOffset);
            isRising = true;
            riseTimer = 0f;
        }
        else
        {
            isRising = false;
        }
    }

    private void Update()
    {
        if (isRising)
        {
            riseTimer += Time.deltaTime;
            float t = Mathf.Clamp01(riseTimer / riseDuration);

            // Smooth ease-out curve for the rise
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            rectTransform.anchoredPosition = Vector2.Lerp(
                originalPosition + new Vector2(0, riseStartOffset),
                originalPosition,
                easedT
            );

            if (t >= 1f)
            {
                isRising = false;
                rectTransform.anchoredPosition = originalPosition;
            }
        }
        else
        {
            // Continuous smooth bounce using a sine wave
            float bounce = Mathf.Sin(Time.time * bounceSpeed * Mathf.PI * 2f) * bounceHeight;
            rectTransform.anchoredPosition = originalPosition + new Vector2(0, bounce);
        }
    }
}

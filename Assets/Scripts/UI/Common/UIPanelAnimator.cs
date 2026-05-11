using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanelAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    public float duration = 0.3f;
    public float startScale = 0.5f;
    
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Coroutine activeRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // Start the entrance animation whenever the panel is activated
        Show();
    }

    public void Show()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(AnimateIn());
    }

    public void Close()
    {
        if (activeRoutine != null) StopCoroutine(activeRoutine);
        activeRoutine = StartCoroutine(AnimateOut());
    }

    private IEnumerator AnimateIn()
    {
        float elapsed = 0;
        canvasGroup.alpha = 0;
        transform.localScale = originalScale * startScale;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            // Bounce / Overshoot effect
            float bounce = Mathf.Sin(t * Mathf.PI * 1.1f) * (1.1f - t) + t;

            transform.localScale = originalScale * bounce;
            canvasGroup.alpha = t;
            yield return null;
        }

        transform.localScale = originalScale;
        canvasGroup.alpha = 1;
        activeRoutine = null;
    }

    private IEnumerator AnimateOut()
    {
        float elapsed = 0;
        Vector3 currentScale = transform.localScale;
        float currentAlpha = canvasGroup.alpha;

        while (elapsed < duration * 0.7f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / (duration * 0.7f);

            transform.localScale = Vector3.Lerp(currentScale, originalScale * startScale, t);
            canvasGroup.alpha = Mathf.Lerp(currentAlpha, 0, t);
            yield return null;
        }

        // Deactivate the object once the animation is done
        gameObject.SetActive(false);
        activeRoutine = null;
    }
}

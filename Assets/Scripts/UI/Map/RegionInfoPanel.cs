using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RegionInfoPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public Image regionIcon;
    public TextMeshProUGUI regionNameText;
    public TextMeshProUGUI languageText;
    public TextMeshProUGUI descriptionText;
    public Button startButton;
    public Button closeButton; 

    [Header("Progress Bar")]
    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    [Header("Animation")]
    public float fadeDuration = 0.4f;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalAnchoredPos;
    private bool hasCapturedPos = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        rectTransform = GetComponent<RectTransform>();
        CapturePosition();

        // Hide by default
        canvasGroup.alpha = 0;
        rectTransform.localScale = Vector3.zero;

        // Hook up close button if assigned
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => 
            {
                if (RegionSelectionManager.Instance != null)
                    RegionSelectionManager.Instance.ResetZoom();
            });
        }
    }

    private void CapturePosition()
    {
        if (hasCapturedPos) return;
        originalAnchoredPos = rectTransform.anchoredPosition;
        hasCapturedPos = true;
    }

    public void Show(RegionData data, Vector2 startAnchoredPos, Vector2? targetAnchoredPos = null)
    {
        if (data == null) return;
        CapturePosition(); 
        
        // Update Content
        if (regionIcon != null) regionIcon.sprite = data.thumbnail;
        if (regionNameText != null) regionNameText.text = data.regionName;
        if (languageText != null) languageText.text = $"Language: {data.language}";
        if (descriptionText != null) descriptionText.text = data.description;
        
        // Update Progress Bar
        if (progressSlider != null) progressSlider.value = data.completionProgress;
        if (progressText != null) progressText.text = (data.completionProgress * 100f).ToString("0") + "%";
        
        // Show with Fade + Scale + Pop
        gameObject.SetActive(true);
        StopAllCoroutines();
        
        Vector2 finalTarget = targetAnchoredPos ?? originalAnchoredPos;
        StartCoroutine(AnimateEntrance(true, startAnchoredPos, finalTarget));
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(AnimateEntrance(false, rectTransform.anchoredPosition, originalAnchoredPos, onComplete: () => gameObject.SetActive(false)));
    }

    private System.Collections.IEnumerator AnimateEntrance(bool entering, Vector2 startPos, Vector2 targetPos, System.Action onComplete = null)
    {
        float targetAlpha = entering ? 1f : 0f;
        float startAlpha = canvasGroup.alpha;
        
        Vector3 targetScale = entering ? Vector3.one : Vector3.zero;
        Vector3 startScale = rectTransform.localScale;

        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            
            float curveT = 1f - Mathf.Pow(1f - t, 4f); 

            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, curveT);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, curveT);
            
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        rectTransform.localScale = targetScale;
        rectTransform.anchoredPosition = targetPos;
        onComplete?.Invoke();
    }
}

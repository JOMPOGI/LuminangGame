using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Image))]
public class RegionClickable : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public RegionData data;
    
    [Header("Selection & Glow")]
    public GameObject glowObject;
    public float fadeDuration = 0.35f;
    private bool isSelected = false;
    private CanvasGroup glowCanvasGroup;
    private Coroutine fadeCoroutine;

    [Header("Visual Feedback")]
    public Color hoverColor = new Color(1, 1, 0, 0.5f);
    private Color originalColor;
    private Image img;

    [Header("Region Availability")]
    public bool isAvailable = true;

    private void Start()
    {
        img = GetComponent<Image>();
        if (img != null) 
        {
            originalColor = img.color;
            img.alphaHitTestMinimumThreshold = 0.1f;
        }

        // AUTO-CREATE GLOW OBJECT: If no glow object is assigned but we have a glow sprite, create it!
        if (glowObject == null && data != null && data.glowSprite != null)
        {
            GameObject newGlow = new GameObject(gameObject.name + "_Glow", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            newGlow.transform.SetParent(transform, false);
            
            // Set up Image and SYNC with parent settings
            Image glowImg = newGlow.GetComponent<Image>();
            glowImg.sprite = data.glowSprite;
            glowImg.raycastTarget = false; 
            glowImg.preserveAspect = img.preserveAspect;
            glowImg.type = img.type;
            
            // Sync RectTransform and PIVOT (crucial for alignment)
            RectTransform glowRect = newGlow.GetComponent<RectTransform>();
            RectTransform parentRect = GetComponent<RectTransform>();
            
            glowRect.pivot = parentRect.pivot;
            glowRect.anchorMin = Vector2.zero;
            glowRect.anchorMax = Vector2.one;
            glowRect.sizeDelta = Vector2.zero;
            glowRect.anchoredPosition = Vector2.zero;
            
            glowObject = newGlow;
        }

        if (glowObject != null) 
        {
            glowCanvasGroup = glowObject.GetComponent<CanvasGroup>();
            // Add it if missing!
            if (glowCanvasGroup == null) glowCanvasGroup = glowObject.AddComponent<CanvasGroup>();
            
            glowCanvasGroup.alpha = 0;
            glowObject.SetActive(true); // Keep it active but invisible!
        }
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeGlow(selected ? 1f : 0f));
    }

    private IEnumerator FadeGlow(float targetAlpha)
    {
        if (glowCanvasGroup == null) yield break;

        float startAlpha = glowCanvasGroup.alpha;
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            
            // Smoothstep for a professional "pulse" feel
            float smoothT = t * t * (3f - 2f * t);
            
            glowCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, smoothT);
            yield return null;
        }

        glowCanvasGroup.alpha = targetAlpha;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) return; 
        
        if (RegionSelectionManager.Instance != null)
        {
            RegionSelectionManager.Instance.SelectRegion(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isAvailable) return;
        if (img != null) img.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isAvailable) return;
        if (img != null) img.color = originalColor;
    }
}

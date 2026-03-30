using UnityEngine;
using System.Collections;

public class LoadingOverlay : MonoBehaviour
{
    public static LoadingOverlay Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject loadingPanel;
    public RectTransform crystal1;
    public RectTransform crystal2;
    public RectTransform crystal3;

    [Header("Animation Settings")]
    public float delayBetweenBounces = 0.15f;
    public float bounceHeight = 20f;
    public float bounceDuration = 0.5f;
    public float pauseDuration = 0.25f;

    private CanvasGroup canvasGroup;
    private Coroutine bounceRoutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        loadingPanel.SetActive(false);
        canvasGroup.alpha = 0;
    }

    public void Show()
    {
        loadingPanel.SetActive(true);
        canvasGroup.alpha = 1;
        if (bounceRoutine != null) StopCoroutine(bounceRoutine);
        bounceRoutine = StartCoroutine(SequenceRoutine());
    }

    public void Hide()
    {
        if (bounceRoutine != null) StopCoroutine(bounceRoutine);
        loadingPanel.SetActive(false);
        canvasGroup.alpha = 0;
    }

    private IEnumerator SequenceRoutine()
    {
        while (true)
        {
            // Start all three in parallel with a slight stagger
            if (crystal1 != null) StartCoroutine(SingleBounce(crystal1));
            yield return new WaitForSecondsRealtime(delayBetweenBounces);
            
            if (crystal2 != null) StartCoroutine(SingleBounce(crystal2));
            yield return new WaitForSecondsRealtime(delayBetweenBounces);
            
            if (crystal3 != null) StartCoroutine(SingleBounce(crystal3));
            
            // Wait for the last one to finish plus the pause
            yield return new WaitForSecondsRealtime(bounceDuration + pauseDuration);
        }
    }

    private IEnumerator SingleBounce(RectTransform crystal)
    {
        if (crystal == null) yield break;

        Vector2 startPos = crystal.anchoredPosition;
        float startY = startPos.y;

        float elapsed = 0f;
        while (elapsed < bounceDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float normalizedProgress = elapsed / bounceDuration;
            
            // Simple curve: sin(pi * progress) goes 0 -> 1 -> 0
            float height = Mathf.Sin(normalizedProgress * Mathf.PI) * bounceHeight;
            
            if (crystal != null)
                crystal.anchoredPosition = new Vector2(startPos.x, startY + height);
            
            yield return null;
        }
        
        if (crystal != null)
            crystal.anchoredPosition = new Vector2(startPos.x, startY);
    }
}

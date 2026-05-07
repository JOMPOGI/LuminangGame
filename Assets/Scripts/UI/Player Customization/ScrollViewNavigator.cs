using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollViewNavigator : MonoBehaviour
{
    [Header("ScrollViews to monitor")]
    [Tooltip("Drag all your category ScrollViews here (Hair, Shirts, Pants, etc.)")]
    public ScrollRect[] scrollViews;

    [Header("Buttons")]
    public Button leftButton;
    public Button rightButton;

    [Header("Scroll Settings")]
    [Tooltip("How much to scroll per click (0.5 = half the viewport width)")]
    [Range(0.1f, 1f)]
    public float scrollAmount = 0.5f;

    [Tooltip("How fast the scroll animation is")]
    public float scrollSpeed = 8f;

    private Coroutine scrollCoroutine;

    void Start()
    {
        if (leftButton != null)
            leftButton.onClick.AddListener(() => Scroll(-scrollAmount));

        if (rightButton != null)
            rightButton.onClick.AddListener(() => Scroll(scrollAmount));
    }

    private ScrollRect GetActiveScrollView()
    {
        foreach (var sv in scrollViews)
        {
            if (sv != null && sv.gameObject.activeInHierarchy)
                return sv;
        }
        return null;
    }

    private void Scroll(float amount)
    {
        ScrollRect active = GetActiveScrollView();
        if (active == null) return;

        float target = Mathf.Clamp01(active.horizontalNormalizedPosition + amount);

        if (scrollCoroutine != null)
            StopCoroutine(scrollCoroutine);

        scrollCoroutine = StartCoroutine(SmoothScroll(active, target));
    }

    private IEnumerator SmoothScroll(ScrollRect scrollRect, float targetPos)
    {
        while (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPos) > 0.001f)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                targetPos,
                Time.deltaTime * scrollSpeed
            );
            yield return null;
        }

        scrollRect.horizontalNormalizedPosition = targetPos;
        scrollCoroutine = null;
    }
}

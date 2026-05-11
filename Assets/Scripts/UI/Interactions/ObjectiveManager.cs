using UnityEngine;
using TMPro;
using System.Collections;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance { get; private set; }
    public static System.Action<string> OnObjectiveChanged;

    [Header("UI References")]
    [Tooltip("The text component that will be updated and animated.")]
    public TextMeshProUGUI objectiveText;

    [Header("Animation Settings")]
    public float fadeDuration = 0.4f;
    [Tooltip("How far it slides from the left (e.g., 100 pixels)")]
    public float slideDistance = 150f;
    [Tooltip("Check this if the panel should slide left/right instead of up/down")]
    public bool slideHorizontal = true;

    public string CurrentObjective { get; private set; } = "";

    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector2 _originalAnchoredPos;
    private Coroutine _animCoroutine;
    private bool _isShowing = true; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (objectiveText != null)
        {
            _rectTransform = objectiveText.GetComponent<RectTransform>();
            _canvasGroup = objectiveText.GetComponent<CanvasGroup>();
            
            if (_canvasGroup == null) _canvasGroup = objectiveText.gameObject.AddComponent<CanvasGroup>();

            if (_rectTransform != null)
            {
                _originalAnchoredPos = _rectTransform.anchoredPosition;
            }

            // Always start hidden so we can animate in
            _canvasGroup.alpha = 0f;
            _isShowing = false;
            
            // Snap to the hidden position immediately
            Vector2 hiddenPos = _originalAnchoredPos;
            if (slideHorizontal) hiddenPos.x -= slideDistance;
            else hiddenPos.y += slideDistance;
            _rectTransform.anchoredPosition = hiddenPos;

            // Grab initial text if it exists
            if (!string.IsNullOrEmpty(objectiveText.text))
            {
                CurrentObjective = objectiveText.text.Trim();
            }
            
            objectiveText.gameObject.SetActive(false);
        }
    }

    private IEnumerator Start()
    {
        // Wait a small moment for the scene to settle, then slide in
        yield return new WaitForSeconds(0.5f);
        UpdateVisibility();
    }

    public void SetObjective(string newObjective)
    {
        string oldObjective = CurrentObjective;
        string cleanObjective = newObjective != null ? newObjective.Trim() : "";

        // Standard Redundancy Check
        if (cleanObjective == oldObjective)
        {
            Debug.Log($"[ObjectiveManager] Update skipped. New objective matches current: '{cleanObjective}'");
            return;
        }

        Debug.Log($"[ObjectiveManager] EVENT: Objective changing FROM '{oldObjective}' TO '{cleanObjective}'");
        
        CurrentObjective = cleanObjective;
        if (objectiveText != null) 
        {
            objectiveText.text = cleanObjective;
            Debug.Log($"[ObjectiveManager] UI Component '{objectiveText.name}' text property updated.");
        }
        else
        {
            Debug.LogError("[ObjectiveManager] FAILED: Objective Text component is missing or unassigned!");
        }
        
        UpdateVisibility();
        OnObjectiveChanged?.Invoke(cleanObjective);
    }

    private void UpdateVisibility()
    {
        bool hasObjective = !string.IsNullOrEmpty(CurrentObjective);
        Debug.Log($"[ObjectiveManager] Visibility Check. Current: '{CurrentObjective}' (HasText: {hasObjective})");
        
        if (hasObjective) Show();
        else Hide();
    }

    public void Hide()
    {
        if (!_isShowing) return; 
        _isShowing = false;
        
        if (objectiveText == null) return;

        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _animCoroutine = StartCoroutine(AnimatePanel(false));
    }

    public void Show()
    {
        if (_isShowing) return; 
        _isShowing = true;

        if (objectiveText == null) return;
        
        if (_animCoroutine != null) StopCoroutine(_animCoroutine);
        _animCoroutine = StartCoroutine(AnimatePanel(true));
    }

    private IEnumerator AnimatePanel(bool show)
    {
        if (_canvasGroup == null || _rectTransform == null)
        {
            Debug.LogWarning("[ObjectiveManager] Missing CanvasGroup or RectTransform on objective text!");
            yield break;
        }

        if (show) 
        {
            objectiveText.gameObject.SetActive(true);
            Debug.Log("[ObjectiveManager] Animating Show...");
        }

        float startAlpha = _canvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;

        Vector2 hiddenPos = _originalAnchoredPos;
        if (slideHorizontal) hiddenPos.x -= slideDistance;
        else hiddenPos.y += slideDistance;

        Vector2 startPos = _rectTransform.anchoredPosition;
        Vector2 targetPos = show ? _originalAnchoredPos : hiddenPos;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);

            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, eased);
            _rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, eased);

            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
        _rectTransform.anchoredPosition = targetPos;

        if (!show) 
        {
            objectiveText.gameObject.SetActive(false);
            Debug.Log("[ObjectiveManager] Animating Hide complete.");
        }
    }

    // Update loop removed to prevent fighting with HUDManager watchdog
}

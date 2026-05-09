using UnityEngine;

public class QuestIndicator : MonoBehaviour
{
    [Header("Settings")]
    public string requiredObjective = "Talk to Kalaw";
    public float hoverSpeed = 3f;
    public float hoverAmount = 0.2f;

    private SpriteRenderer _renderer;
    private Vector3 _startPos;

    void Awake()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _startPos = transform.localPosition;
        UpdateVisibility("");
    }

    void OnEnable() => ObjectiveManager.OnObjectiveChanged += UpdateVisibility;
    void OnDisable() => ObjectiveManager.OnObjectiveChanged -= UpdateVisibility;

    void Update()
    {
        if (ObjectiveManager.Instance == null) return;

        // 1. Determine if we should be visible
        string cleanRequired = requiredObjective != null ? requiredObjective.Trim() : "";
        bool isCorrectObjective = ObjectiveManager.Instance.CurrentObjective == cleanRequired;
        bool isInDialogue = DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue;
        bool shouldBeVisible = isCorrectObjective && !isInDialogue;

        // 2. Apply visibility
        if (_renderer != null && _renderer.enabled != shouldBeVisible)
        {
            _renderer.enabled = shouldBeVisible;
        }

        // 3. Hover animation (only if visible)
        if (shouldBeVisible)
        {
            float newY = _startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
            transform.localPosition = new Vector3(_startPos.x, newY, _startPos.z);
        }
    }

    // This is still here for events, but Update handles the heavy lifting now
    void UpdateVisibility(string newObjective) { /* Handled in Update */ }
}

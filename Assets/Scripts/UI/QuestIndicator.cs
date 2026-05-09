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
        // Hide if in dialogue
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue)
        {
            if (_renderer != null) _renderer.enabled = false;
            return;
        }

        // Hover animation
        if (_renderer != null && _renderer.enabled)
        {
            float newY = _startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
            transform.localPosition = new Vector3(_startPos.x, newY, _startPos.z);
        }
    }

    void UpdateVisibility(string newObjective)
    {
        bool shouldShow = newObjective == requiredObjective;
        if (_renderer != null) _renderer.enabled = shouldShow;
    }
}

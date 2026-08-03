using UnityEngine;

/// <summary>
/// Professional Quest Indicator that syncs with the ObjectiveManager.
/// Automatically hides/shows based on the current quest objective.
/// </summary>
public class QuestIndicator : MonoBehaviour
{
    [Header("Settings")]
    public string requiredObjective = "Talk to Kalaw";
    
    [Header("Animation")]
    public float hoverSpeed = 3f;
    public float hoverAmount = 0.2f;

    [Header("Target")]
    [Tooltip("The visual object to toggle. If empty, toggles the SpriteRenderer on this object.")]
    public GameObject visualRoot;

    private SpriteRenderer _myRenderer;
    private Vector3 _startPos;
    private bool _isInitialized = false;
    private bool _matchesObjective = false;
    private InteractableNPC _parentNPC;

    void Awake()
    {
        _startPos = transform.localPosition;
        
        // Safety: If visualRoot is this object or null, we find a child instead
        if (visualRoot == null || visualRoot == gameObject)
        {
            // Search for children even if they are inactive!
            _myRenderer = GetComponentInChildren<SpriteRenderer>(true);
            
            if (_myRenderer != null && _myRenderer.gameObject != gameObject)
            {
                visualRoot = _myRenderer.gameObject;
            }
        }
        
        _parentNPC = GetComponentInParent<InteractableNPC>();
        _isInitialized = true;
    }

    void OnEnable()
    {
        ObjectiveManager.OnObjectiveChanged += HandleObjectiveChanged;
        // Initial sync for objective
        if (ObjectiveManager.Instance != null)
        {
            HandleObjectiveChanged(ObjectiveManager.Instance.CurrentObjective);
        }
    }

    void OnDisable()
    {
        ObjectiveManager.OnObjectiveChanged -= HandleObjectiveChanged;
    }

    private void HandleObjectiveChanged(string newObjective)
    {
        if (!_isInitialized) return;

        // Perform the "Heavy" string matching only when the objective actually changes
        _matchesObjective = !string.IsNullOrEmpty(newObjective) && 
                            newObjective.StartsWith(requiredObjective.Trim(), System.StringComparison.OrdinalIgnoreCase);
    }

    void Update()
    {
        if (!_isInitialized) return;

        // If attached to an NPC, perfectly sync with its exact objective target status
        if (_parentNPC != null)
        {
            string currentObj = ObjectiveManager.Instance != null ? ObjectiveManager.Instance.CurrentObjective : "";
            _matchesObjective = _parentNPC.IsTargetOfObjective(currentObj);
        }

        // 1. Check if we are in a dialogue right now (Reliable every-frame check)
        bool isInDialogue = DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue;

        // 2. Final Visibility: We only show if the quest matches AND we aren't talking
        bool shouldBeVisible = _matchesObjective && !isInDialogue;

        SetVisibility(shouldBeVisible);

        // 3. Hover animation (only if actually visible)
        if (shouldBeVisible)
        {
            float newY = _startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
            transform.localPosition = new Vector3(_startPos.x, newY, _startPos.z);
        }
    }

    private void SetVisibility(bool visible)
    {
        // Option A: Toggle the specific visual root object
        if (visualRoot != null && visualRoot != gameObject)
        {
            if (visualRoot.activeSelf != visible) visualRoot.SetActive(visible);
        }
        // Option B: Toggle just the renderer
        else if (_myRenderer != null)
        {
            if (_myRenderer.enabled != visible) _myRenderer.enabled = visible;
        }
    }
}

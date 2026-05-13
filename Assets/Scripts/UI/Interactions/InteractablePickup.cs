using UnityEngine;

/// <summary>
/// A specialized pickup script that only glows when a quest is active.
/// Inherits from InteractableBase to work with the Talk/Interact button system.
/// </summary>
public class InteractablePickup : InteractableBase
{
    [Header("Quest Settings")]
    public string requiredObjective = "Collect yarns";
    public bool hideOnPickup = true; // New Toggle!
    public bool canBeClicked = true; // New Toggle!
    
    [Header("Glow Appearance (HDR)")]
    [ColorUsage(true, true)]
    public Color glowColor = new Color(4f, 3.5f, 0.5f, 1f);
    public float pulseSpeed = 3f;
    public float streakHeight = 1.5f;

    private Renderer _renderer;
    private Material _mat;
    private LineRenderer _line;
    private bool _matchesObjective = false;
    private bool _isInitialized = false;

    protected virtual void Awake()
    {
        SetupVisuals();
        _isInitialized = true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ObjectiveManager.OnObjectiveChanged += HandleObjectiveChanged;
        if (ObjectiveManager.Instance != null) HandleObjectiveChanged(ObjectiveManager.Instance.CurrentObjective);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ObjectiveManager.OnObjectiveChanged -= HandleObjectiveChanged;
    }

    private void HandleObjectiveChanged(string newObjective)
    {
        if (!_isInitialized) return;

        _matchesObjective = !string.IsNullOrEmpty(newObjective) && 
                            newObjective.StartsWith(requiredObjective.Trim(), System.StringComparison.OrdinalIgnoreCase);
        
        // Only enable interaction if the quest matches AND the designer allowed it
        interactionEnabled = _matchesObjective && canBeClicked;
        
        if (_line != null) _line.gameObject.SetActive(_matchesObjective);
        if (!_matchesObjective && _mat != null) _mat.SetColor("_EmissionColor", Color.black);
    }

    public override void Interact()
    {
        if (!interactionEnabled) return;

        Debug.Log($"[InteractablePickup] {gameObject.name} interaction triggered!");
        
        // Fire the standard event from the base class
        OnInteract?.Invoke();
        
        // Only hide if the toggle is checked
        if (hideOnPickup)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!_matchesObjective || !gameObject.activeInHierarchy) return;

        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        float intensity = Mathf.Lerp(1f, 8f, t);

        if (_mat != null) _mat.SetColor("_EmissionColor", glowColor * intensity);

        if (_line != null)
        {
            _line.startWidth = Mathf.Lerp(0.02f, 0.15f, t);
            _line.startColor = new Color(glowColor.r, glowColor.g, glowColor.b, Mathf.Lerp(0.1f, 0.8f, t));
        }
    }

    private void SetupVisuals()
    {
        // 1. Setup Emission
        _renderer = GetComponentInChildren<Renderer>();
        if (_renderer != null)
        {
            _mat = _renderer.material;
            _mat.EnableKeyword("_EMISSION");
        }

        // 2. Setup Light Streak
        GameObject streakGO = new GameObject("_QuestStreak");
        streakGO.transform.SetParent(transform);
        streakGO.transform.localPosition = Vector3.zero;

        _line = streakGO.AddComponent<LineRenderer>();
        _line.positionCount = 2;
        _line.useWorldSpace = false;
        _line.SetPosition(0, Vector3.zero);
        _line.SetPosition(1, Vector3.up * streakHeight);
        _line.endWidth = 0f;

        Shader urpUnlit = Shader.Find("Universal Render Pipeline/Unlit");
        Material lineMat = new Material(urpUnlit != null ? urpUnlit : Shader.Find("Unlit/Color"));
        lineMat.SetFloat("_Surface", 1);
        lineMat.SetFloat("_Blend", 3);
        lineMat.SetColor("_BaseColor", glowColor);
        _line.material = lineMat;
        
        streakGO.SetActive(false);
    }
}

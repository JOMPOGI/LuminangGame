using UnityEngine;
using UnityEngine.Events;

public class ProximityTrigger : MonoBehaviour
{
    [Header("Settings")]
    public float triggerDistance = 3f;
    public string requiredObjective;
    public bool triggerOnce = true;
    
    [Header("Events")]
    public UnityEvent OnTriggered;

    private bool _hasTriggered = false;
    private Transform _player;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) _player = player.transform;
    }

    void Update()
    {
        if (_hasTriggered || _player == null) return;

        // Check if quest matches
        if (!string.IsNullOrEmpty(requiredObjective))
        {
            if (ObjectiveManager.Instance == null || 
                !ObjectiveManager.Instance.CurrentObjective.StartsWith(requiredObjective, System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
        }

        // Check distance
        float distance = Vector3.Distance(transform.position, _player.position);
        if (distance <= triggerDistance)
        {
            ExecuteTrigger();
        }
    }

    private void ExecuteTrigger()
    {
        _hasTriggered = triggerOnce;
        Debug.Log($"[ProximityTrigger] Location reached: {gameObject.name}");
        OnTriggered?.Invoke();
    }
}

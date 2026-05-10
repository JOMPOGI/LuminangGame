using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script handles special "hide-and-seek" or "proximity" events.
/// When the player gets close enough, it triggers a dialogue, updates the objective,
/// and fires any custom events (like making the NPC disappear).
/// </summary>
public class ProximityDisappear : MonoBehaviour
{
    [Header("Requirement")]
    [Tooltip("Optional: Only trigger if this is the current objective. Leave empty to always trigger.")]
    public string requiredObjective;

    [Header("Trigger Settings")]
    [Tooltip("How close the player needs to be to trigger this event.")]
    public float triggerDistance = 5f;
    [Tooltip("The tag of the player object.")]
    public string playerTag = "Player";

    [Header("Dialogue Integration")]
    [Tooltip("The dialogue to automatically start when triggered.")]
    public DialogueNode dialogueToStart;
    [Tooltip("Optional: The NPC's animator if the dialogue needs it.")]
    public Animator npcAnimator;
    [Tooltip("Optional: The InteractableNPC component if the dialogue needs it.")]
    public InteractableNPC npcReference;

    [Header("Objective Update")]
    [Tooltip("The new objective text to show (e.g., 'Talk to Apo Lakay'). Leave empty to not change objective.")]
    public string newObjective;

    [Header("Events")]
    [Tooltip("Add actions here, like 'GameObject.SetActive(false)' to make the NPC disappear.")]
    public UnityEvent OnTriggered;

    private bool _hasTriggered = false;
    private Transform _playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning($"[ProximityDisappear] Could not find player with tag '{playerTag}'!");
        }
    }

    void Update()
    {
        // Don't check if already triggered or if player is missing
        if (_hasTriggered || _playerTransform == null) return;

        // Check objective requirement if set
        if (!string.IsNullOrEmpty(requiredObjective) && ObjectiveManager.Instance != null)
        {
            // Trim both to be safe against trailing spaces
            string currentObj = ObjectiveManager.Instance.CurrentObjective.Trim();
            string requiredObj = requiredObjective.Trim();

            if (currentObj != requiredObj)
                return;
        }

        float distance = Vector3.Distance(transform.position, _playerTransform.position);
        
        if (distance <= triggerDistance)
        {
            ExecuteTrigger();
        }
    }

    private void ExecuteTrigger()
    {
        _hasTriggered = true;
        Debug.Log($"[ProximityDisappear] Triggered proximity event on {gameObject.name}");

        // 1. Update Objective first (so it's ready when dialogue starts)
        if (!string.IsNullOrEmpty(newObjective) && ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective(newObjective);
        }

        // 2. Start Dialogue
        if (dialogueToStart != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(dialogueToStart, npcAnimator, npcReference);
        }

        // 3. Fire Custom Events (User can use this to hide the NPC, play a sound, etc.)
        OnTriggered?.Invoke();
    }
    
    // Gizmo to help the user see the trigger range in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}

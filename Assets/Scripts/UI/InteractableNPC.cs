using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Attach this script to any NPC or object you want to interact with.
/// It registers itself with the InteractionManager when the player is close.
/// </summary>
public class InteractableNPC : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("How close the player must be to see the Talk button.")]
    public float interactionDistance = 3f;

    [Tooltip("What happens when the player clicks the Talk button?")]
    public UnityEvent OnInteract;

    [Header("UI Customization")]
    [Tooltip("The text to display on the button when near this NPC (e.g., 'Talk', 'Inspect', 'Pick up')")]
    public string promptText = "Talk";

    [Header("Dialogue Settings (Optional)")]
    [Tooltip("If assigned, clicking Talk will start a branching conversation using this node.")]
    public DialogueNode startingDialogueNode;
    
    [Tooltip("The Animator of this NPC, used to play animations during dialogue (Optional).")]
    public Animator npcAnimator;

    [Tooltip("Fires when the branching dialogue conversation completely finishes. Use this to resume idle animations!")]
    public UnityEvent OnDialogueEnd;

    [Tooltip("Fires when the player picks a wrong answer choice. Wire this to KalawIdleTest.PlayWrongAnswerReaction()!")]
    public UnityEvent OnWrongAnswer;

    /// <summary>
    /// Set to true by KalawIdleTest while the wrong-answer animation is playing.
    /// DialogueManager polls this to know when to re-show the question.
    /// </summary>
    [HideInInspector] public bool isWrongAnswerPlaying = false;

    /// <summary>
    /// Called by the InteractionManager when the player clicks the button
    /// while standing near this specific NPC.
    /// </summary>
    public void Interact()
    {
        Debug.Log($"[InteractableNPC] Interacting with {gameObject.name}");
        
        // If a dialogue node is assigned, start the branching conversation
        if (startingDialogueNode != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(startingDialogueNode, npcAnimator, this);
        }

        // Always fire the UnityEvent for backwards compatibility
        OnInteract?.Invoke();
    }

    void OnEnable()
    {
        if (InteractionManager.Instance != null)
            InteractionManager.Instance.RegisterNPC(this);
    }

    void OnDisable()
    {
        if (InteractionManager.Instance != null)
            InteractionManager.Instance.UnregisterNPC(this);
    }

    void Start()
    {
        // Fallback registration in case it enables before manager is awake
        if (InteractionManager.Instance != null)
            InteractionManager.Instance.RegisterNPC(this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0.5f, 0.3f);
        Gizmos.DrawSphere(transform.position, interactionDistance);
        Gizmos.color = new Color(0f, 1f, 0.5f, 1f);
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}

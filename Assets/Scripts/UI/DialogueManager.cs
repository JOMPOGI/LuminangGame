using UnityEngine;

/// <summary>
/// Global manager that controls branching conversations.
/// Takes over when InteractionManager triggers a dialogue start.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    [Tooltip("The script that handles the visual display of the dialogue box.")]
    public DialogueUIController uiController;

    private Animator _currentNPCAnimator;
    private InteractableNPC _currentNPC;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Called by InteractableNPC when the player clicks the Talk button.
    /// </summary>
    public void StartDialogue(DialogueNode startNode, Animator npcAnimator, InteractableNPC npc)
    {
        _currentNPCAnimator = npcAnimator;
        _currentNPC = npc;

        // Hide the proximity Talk button because we are now in conversation mode
        if (InteractionManager.Instance != null && InteractionManager.Instance.talkButton != null)
        {
            InteractionManager.Instance.talkButton.gameObject.SetActive(false);
        }

        // Process the first node
        ProcessNode(startNode);
    }

    private void ProcessNode(DialogueNode node)
    {
        if (node == null)
        {
            EndDialogue();
            return;
        }

        // 1. Play Animation (if specified)
        if (!string.IsNullOrEmpty(node.animationTrigger) && _currentNPCAnimator != null)
        {
            // Reset all potential triggers to avoid animation queueing issues
            // Usually, it's safer just to set the specific trigger
            _currentNPCAnimator.SetTrigger(node.animationTrigger);
        }

        // 2. Display UI and Choices
        if (uiController != null)
        {
            // We pass a callback method that the UI will call when a choice is clicked
            uiController.DisplayNode(node, OnChoiceSelected);
        }
    }

    /// <summary>
    /// Triggered by the DialogueUIController when the player clicks a choice button.
    /// </summary>
    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (choice.isWrong && _currentNPC != null)
        {
            // Hide dialogue box, play reaction, then come back to the question
            StartCoroutine(HandleWrongAnswer(choice.nextNode));
            return;
        }

        // Correct (or neutral) choice — advance normally
        ProcessNode(choice.nextNode);
    }

    /// <summary>
    /// Hides the dialogue box, waits for the wrong-answer animation to finish,
    /// then re-processes the node (loops back to the question).
    /// </summary>
    private System.Collections.IEnumerator HandleWrongAnswer(DialogueNode returnToNode)
    {
        Debug.Log($"[DialogueManager] Handling wrong answer. Returning to: {(returnToNode != null ? returnToNode.name : "NULL")}");

        // 1. Hide the dialogue box immediately
        if (uiController != null)
            uiController.HideDialogue();

        // 2. Fire OnWrongAnswer — this calls KalawIdleTest.PlayWrongAnswerReaction()
        if (_currentNPC != null)
        {
            _currentNPC.OnWrongAnswer?.Invoke();
        }

        // 3. Wait one frame for isWrongAnswerPlaying to be set
        yield return null;

        // 4. Wait until the animation is done (KalawIdleTest sets flag to false when done)
        float elapsed = 0f;
        while (_currentNPC != null && _currentNPC.isWrongAnswerPlaying && elapsed < 6f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (elapsed >= 6f) Debug.LogWarning("[DialogueManager] Wrong answer animation timed out.");

        // 5. Re-show the question node
        if (returnToNode != null)
        {
            Debug.Log($"[DialogueManager] Resuming dialogue node: {returnToNode.name}");
            ProcessNode(returnToNode);
        }
        else
        {
            Debug.LogError("[DialogueManager] Cannot resume: returnToNode is NULL.");
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        Debug.Log("[DialogueManager] Conversation ended.");
        
        if (uiController != null)
            uiController.HideDialogue();

        // Fire the end event so the NPC can resume its idle animation
        if (_currentNPC != null && _currentNPC.OnDialogueEnd != null)
        {
            _currentNPC.OnDialogueEnd.Invoke();
        }

        if (InteractionManager.Instance != null)
        {
            InteractionManager.Instance.ForceCheckProximity();
        }

        _currentNPCAnimator = null;
        _currentNPC = null;
    }
}

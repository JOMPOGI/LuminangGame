using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Global manager that controls branching conversations.
/// Takes over when InteractionManager triggers a dialogue start.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    /// <summary>
    /// True for the entire duration of a conversation.
    /// InteractionManager and QuestIndicator use this to hide themselves.
    /// </summary>
    public bool IsInDialogue { get; private set; } = false;

    [Header("References")]
    [Tooltip("The script that handles the visual display of the dialogue box.")]
    public DialogueUIController uiController;

    private Animator _currentNPCAnimator;
    private InteractableNPC _currentNPC;

    // ── History for Prev button ───────────────────────────────────
    private readonly Stack<DialogueNode> _nodeHistory = new Stack<DialogueNode>();
    private DialogueNode _activeNode;
    private bool _navigatingBack = false;

    public bool CanGoBack => _nodeHistory.Count > 0;
    private string _pendingEventName;

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
        IsInDialogue = true;

        // Hide the proximity Talk button
        if (InteractionManager.Instance != null && InteractionManager.Instance.talkButton != null)
        {
            InteractionManager.Instance.talkButton.gameObject.SetActive(false);
        }

        // Process the first node
        ProcessNode(startNode);
    }

    private void ProcessNode(DialogueNode node, bool skipAnimation = false)
    {
        if (node == null)
        {
            EndDialogue();
            return;
        }

        // Track history (skip when navigating back to avoid double-pushing)
        if (!_navigatingBack && _activeNode != null)
            _nodeHistory.Push(_activeNode);
        _navigatingBack = false;
        _activeNode = node;

        // 1. Play Animation (or reset to Idle if none specified)
        if (_currentNPCAnimator != null)
        {
            if (!string.IsNullOrEmpty(node.animationTrigger))
                _currentNPCAnimator.SetTrigger(node.animationTrigger);
            else
                _currentNPCAnimator.SetTrigger("Idle"); // Force back to idle if bubble is empty
        }

        // 1.5 Fire Start Event (Immediate)
        if (!string.IsNullOrEmpty(node.triggerEventName) && _currentNPC != null)
        {
            _currentNPC.HandleDialogueEvent(node.triggerEventName);
        }

        // 1.6 Store End Event to fire when this node is COMPLETED
        _pendingEventName = node.endEventName;

        // 2. Display UI and update nav buttons
        if (uiController != null)
        {
            uiController.DisplayNode(node, OnChoiceSelected, skipAnimation);
            uiController.SetNavigation(canGoBack: _nodeHistory.Count > 0);
        }
    }

    /// <summary>
    /// Called by the Prev button in DialogueUIController.
    /// </summary>
    public void GoToPreviousNode()
    {
        if (_nodeHistory.Count == 0) return;
        _navigatingBack = true;
        _activeNode = null;
        ProcessNode(_nodeHistory.Pop());
    }

    /// <summary>
    /// Triggered by the DialogueUIController when the player clicks a choice button.
    /// </summary>
    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (choice == null)
        {
            // Fire the event BEFORE we clean up the NPC reference
            FirePendingEvent(_currentNPC);
            EndDialogue();
            return;
        }

        FirePendingEvent(_currentNPC); 

        // ── Handle Choice-Specific Events ──
        if (!string.IsNullOrEmpty(choice.choiceEvent) && _currentNPC != null)
        {
            _currentNPC.HandleDialogueEvent(choice.choiceEvent);
        }

        if (choice.isWrong && _currentNPC != null)
        {
            StartCoroutine(HandleWrongAnswer(choice.nextNode));
            return;
        }

        ProcessNode(choice.nextNode);
    }

    private System.Collections.IEnumerator HandleWrongAnswer(DialogueNode returnToNode)
    {
        Debug.Log($"[DialogueManager] Handling wrong answer. Returning to: {(returnToNode != null ? returnToNode.name : "NULL")}");

        // Only hide the dialogue UI visually — do NOT touch movementUI since we're still in dialogue
        if (uiController != null)
            uiController.HideChoicesOnly();

        // Trigger the NPC wrong-answer animation
        if (_currentNPC != null)
            _currentNPC.TriggerWrongAnswerAnimation();

        yield return null;

        // Wait for wrong-answer animation to finish (max 6 sec safety timeout)
        float elapsed = 0f;
        while (_currentNPC != null && _currentNPC.isWrongAnswerPlaying && elapsed < 6f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (elapsed >= 6f) Debug.LogWarning("[DialogueManager] Wrong answer animation timed out.");

        if (returnToNode != null)
        {
            // Clear history so Prev button doesn't show when we loop back to the start
            _nodeHistory.Clear();
            _activeNode = null;
            ProcessNode(returnToNode, skipAnimation: true); // SKIP ANIMATION HERE
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        Debug.Log("[DialogueManager] Conversation ended.");
        IsInDialogue = false;
        _nodeHistory.Clear();
        _activeNode = null;
        
        if (uiController != null)
            uiController.HideDialogue();

        if (_currentNPC != null)
        {
            // Force NPC back to Idle when dialogue ends
            if (_currentNPCAnimator != null)
                _currentNPCAnimator.SetTrigger("Idle");

            if (_currentNPC.OnDialogueEnd != null)
                _currentNPC.OnDialogueEnd.Invoke();
            
            // Re-disable interaction if it's a one-time thing, or if we are launching a lesson/tutorial flow
            if (_currentNPC.disableAfterInteraction)
            {
                _currentNPC.interactionEnabled = false;
            }
        }

        if (InteractionManager.Instance != null)
        {
            InteractionManager.Instance.ForceCheckProximity();
        }

        _currentNPCAnimator = null;
        _currentNPC = null;
    }

    private void FirePendingEvent(InteractableNPC npc)
    {
        if (!string.IsNullOrEmpty(_pendingEventName))
        {
            Debug.Log($"[DialogueManager] FirePendingEvent: Sending '{_pendingEventName}' to NPC: {(npc != null ? npc.name : "NULL")}");
            if (npc != null)
            {
                npc.HandleDialogueEvent(_pendingEventName);
            }
        }
        _pendingEventName = null;
    }

    /// <summary>
    /// Programmatically advances the dialogue by choosing the first choice
    /// (or ending dialogue if there are no choices). Used by STT adapter.
    /// </summary>
    public void AdvanceDialogue()
    {
        if (_activeNode != null)
        {
            if (_activeNode.choices != null && _activeNode.choices.Count > 0)
            {
                // Select the first non-wrong choice if possible, or just the first choice
                DialogueChoice choice = _activeNode.choices.Find(c => !c.isWrong);
                if (choice == null) choice = _activeNode.choices[0];
                OnChoiceSelected(choice);
            }
            else
            {
                OnChoiceSelected(null);
            }
        }
    }
}


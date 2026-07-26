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
    /// Called by UnityEvents or cutscenes with just a DialogueNode.
    /// </summary>
    public void StartDialogue(DialogueNode startNode)
    {
        StartDialogue(startNode, null, null);
    }

    /// <summary>
    /// Called by InteractableNPC when the player clicks the Talk button.
    /// </summary>
    public void StartDialogue(DialogueNode startNode, Animator npcAnimator, InteractableNPC npc)
    {
        Debug.Log($"[DialogueManager] StartDialogue called with node: {(startNode == null ? "NULL" : startNode.name)}");
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
            Debug.Log("<color=red>[DialogueManager] ProcessNode received NULL node! Ending dialogue.</color>");
            EndDialogue();
            return;
        }

        Debug.Log($"<color=yellow>[DialogueManager] ProcessNode -> Loading Node: '{node.name}', Dialogue Text: '{node.dialogueText}'</color>");

        // Track history (skip when navigating back to avoid double-pushing)
        if (!_navigatingBack && _activeNode != null)
            _nodeHistory.Push(_activeNode);
        _navigatingBack = false;
        _activeNode = node;

        // 1. Play Animation (or reset to Idle if none specified)
        if (_currentNPCAnimator != null)
        {
            if (!string.IsNullOrEmpty(node.animationTrigger))
            {
                SafeSetTrigger(_currentNPCAnimator, node.animationTrigger);
            }
            else
            {
                SafeSetTrigger(_currentNPCAnimator, "Idle"); // Force back to idle if bubble is empty
            }
        }

        // 1.5 Fire Start Event (Immediate)
        if (!string.IsNullOrEmpty(node.triggerEventName))
        {
            HandleGlobalDialogueEvent(node.triggerEventName);
        }

        // 1.6 Store End Event to fire when this node is COMPLETED
        _pendingEventName = node.endEventName;

        // Check if the node has an STT choice and assign PendingSTTChoice immediately so the UI controller knows.
        PendingSTTChoice = null;
        if (node.choices != null)
        {
            foreach (var choice in node.choices)
            {
                bool isSTTChoice = !string.IsNullOrEmpty(choice.expectedSTTWord) ||
                                  (choice.choiceEvent != null && choice.choiceEvent.Trim().Equals("StartSTT", System.StringComparison.OrdinalIgnoreCase));

                if (isSTTChoice)
                {
                    PendingSTTChoice = choice;
                    break;
                }
            }
        }

        // Automatically show TeachingOverlayPanel if this is an STT node
        if (PendingSTTChoice != null && TeachingOverlayPanel.Instance != null)
        {
            TeachingOverlayPanel.Instance.ShowForPendingSTT(node.triggerEventName);
        }

        // 2. Display UI and update nav buttons
        if (uiController != null)
        {
            if (!string.IsNullOrEmpty(_injectedPrefix))
            {
                uiController.injectedPrefixText = _injectedPrefix;
                _injectedPrefix = "";
            }
            uiController.DisplayNode(node, OnChoiceSelected, skipAnimation);
            uiController.SetNavigation(canGoBack: _nodeHistory.Count > 0);
        }
        else
        {
            Debug.LogError("[DialogueManager] uiController is NULL! The dialogue panel cannot be shown!");
        }
    }

    public DialogueChoice PendingSTTChoice { get; set; }

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
            
            // If this choice triggers STT, pause dialogue advancement
            if (choice.choiceEvent.Trim().Equals("StartSTT", System.StringComparison.OrdinalIgnoreCase))
            {
                PendingSTTChoice = choice;
                if (uiController != null) 
                {
                    uiController.ToggleSTTRecording(choice);
                }
                return; // PAUSE here. STTDialogueAdapter will call CompleteSTT later.
            }
        }

        if (choice.isWrong && _currentNPC != null)
        {
            StartCoroutine(HandleWrongAnswer(choice.nextNode));
            return;
        }

        ProcessNode(choice.nextNode);
    }

    private string _injectedPrefix = "";

    public void CompleteSTT(bool success, string prefixText = "")
    {
        Debug.Log($"<color=cyan>[DialogueManager] CompleteSTT called with success={success}. PendingSTTChoice: {(PendingSTTChoice != null ? PendingSTTChoice.expectedSTTWord : "NULL")}</color>");

        DialogueChoice choice = PendingSTTChoice;
        if (choice == null && _activeNode != null && _activeNode.choices != null && _activeNode.choices.Count > 0)
        {
            choice = _activeNode.choices[0];
            Debug.Log($"<color=cyan>[DialogueManager] Used activeNode.choices[0] fallback. Target nextNode: {(choice != null && choice.nextNode != null ? choice.nextNode.name : "NULL")}</color>");
        }

        if (choice != null)
        {
            PendingSTTChoice = null;
            
            if (success)
            {
                if (!string.IsNullOrEmpty(prefixText))
                {
                    _injectedPrefix = prefixText;
                }

                Debug.Log($"<color=green>[DialogueManager] STT SUCCESS! Loading nextNode: {(choice.nextNode != null ? choice.nextNode.name : "NULL (Ends Dialogue)")}</color>");
                ProcessNode(choice.nextNode);
            }
            else
            {
                Debug.Log("<color=red>[DialogueManager] STT FAILED!</color>");
                if (_currentNPC != null)
                    StartCoroutine(HandleWrongAnswer(_activeNode));
            }
        }
        else
        {
            Debug.LogWarning("[DialogueManager] CompleteSTT called but no choice or activeNode choice could be found!");
        }
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
            {
                SafeSetTrigger(_currentNPCAnimator, "Idle");
            } // Automatically exit close up to restore camera state
            _currentNPC.ExitCloseUp();

            if (_currentNPC.OnDialogueEnd != null)
                _currentNPC.OnDialogueEnd.Invoke();
            
            // Re-disable interaction if it's a one-time thing, or if we are launching a lesson/tutorial flow
            if (_currentNPC.disableAfterInteraction)
            {
                _currentNPC.interactionEnabled = false;
            }
        }

        // Hide TeachingOverlayPanel if active
        if (TeachingOverlayPanel.Instance != null)
        {
            TeachingOverlayPanel.Instance.Hide();
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
            HandleGlobalDialogueEvent(_pendingEventName);
        }
        
        if (npc != null)
        {
            npc.isWrongAnswerPlaying = false;
        }
        
        _pendingEventName = null;
    }

    /// <summary>
    /// Handles dialogue events globally (SetObjective, TeachingOverlayPanel, and custom NPC events).
    /// Works even if _currentNPC is null!
    /// </summary>
    private void HandleGlobalDialogueEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName)) return;

        string cleanEventName = eventName.Trim();
        Debug.Log($"[DialogueManager] Handling Event: '{cleanEventName}'");

        // 1. Handle SetObjective: or SetObjective_
        if (cleanEventName.StartsWith("SetObjective:", System.StringComparison.OrdinalIgnoreCase))
        {
            string newObjText = cleanEventName.Substring("SetObjective:".Length).Trim();
            if (ObjectiveManager.Instance != null && !string.IsNullOrEmpty(newObjText))
            {
                ObjectiveManager.Instance.SetObjective(newObjText);
                Debug.Log($"[DialogueManager] Objective set to: '{newObjText}'");
            }
        }
        else if (cleanEventName.StartsWith("SetObjective_", System.StringComparison.OrdinalIgnoreCase))
        {
            string newObjText = cleanEventName.Substring("SetObjective_".Length).Trim();
            if (ObjectiveManager.Instance != null && !string.IsNullOrEmpty(newObjText))
            {
                ObjectiveManager.Instance.SetObjective(newObjText);
                Debug.Log($"[DialogueManager] Objective set to: '{newObjText}'");
            }
        }

        // 2. Handle TeachingOverlayPanel events
        if (cleanEventName.StartsWith("ShowTeachingPanel", System.StringComparison.OrdinalIgnoreCase))
        {
            if (TeachingOverlayPanel.Instance != null)
            {
                TeachingOverlayPanel.Instance.ShowFromEvent(cleanEventName);
            }
        }
        else if (cleanEventName.Equals("HideTeachingPanel", System.StringComparison.OrdinalIgnoreCase))
        {
            if (TeachingOverlayPanel.Instance != null)
            {
                TeachingOverlayPanel.Instance.Hide();
            }
        }
        else if (TeachingOverlayPanel.Instance != null && PendingSTTChoice != null)
        {
            // Fallback: if triggerEventName is just a background name (e.g. "maayongBuntag" or "bg_morning"), pass it to TeachingOverlayPanel
            TeachingOverlayPanel.Instance.ShowForPendingSTT(cleanEventName);
        }

        // 3. Forward to current NPC if available
        if (_currentNPC != null)
        {
            _currentNPC.HandleDialogueEvent(cleanEventName);
        }
    }

    private void SafeSetTrigger(Animator animator, string triggerName)
    {
        if (animator == null || string.IsNullOrEmpty(triggerName)) return;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger && param.name == triggerName)
            {
                animator.SetTrigger(triggerName);
                return;
            }
        }
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


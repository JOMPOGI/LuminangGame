using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Dialogue Node", menuName = "Dialogue/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [Header("NPC Settings")]
    [Tooltip("The name of the NPC speaking (optional).")]
    public string speakerName;
    
    [TextArea(3, 5)]
    [Tooltip("What the NPC says in the dialogue box.")]
    public string dialogueText;

    [Tooltip("Trigger name to send to the NPC's Animator (e.g., 'DoPointing'). Leave empty for no animation.")]
    public string animationTrigger;

    [Header("Player Options")]
    [Tooltip("The choices the player has. If this list is empty, the conversation ends.")]
    public List<DialogueChoice> choices = new List<DialogueChoice>();
}

[System.Serializable]
public class DialogueChoice
{
    [Tooltip("What the player's button will say (e.g., 'Yes', 'No', 'Tell me more').")]
    public string choiceText;

    [Tooltip("The next Dialogue Node to load if the player clicks this option. If left empty, clicking this ends the conversation.")]
    public DialogueNode nextNode;

    [Tooltip("Mark this true if this is a WRONG answer. The NPC's OnWrongAnswer event will fire before advancing to the next node.")]
    public bool isWrong;
}

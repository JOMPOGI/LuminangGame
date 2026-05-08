using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Handles the visual display of the Dialogue System.
/// You can customize the look of the Dialogue Box here without breaking the core logic.
/// </summary>
public class DialogueUIController : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The main container for the dialogue box (e.g., the stretchable image panel).")]
    public GameObject dialoguePanel;
    
    [Tooltip("The container where choice buttons will be spawned (should have a Vertical Layout Group).")]
    public Transform choicesContainer;

    [Header("Text Elements")]
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueText;

    [Header("Prefabs")]
    [Tooltip("The stretchable button prefab to use for choices (e.g., your talk_button).")]
    public GameObject choiceButtonPrefab;

    [Header("Optional UI")]
    [Tooltip("The UI group containing Joysticks/Buttons that should be hidden during dialogue.")]
    public GameObject movementUI;

    [Tooltip("The background/container for choices that should be hidden when dialogue is off.")]
    public GameObject choicesGroup;

    private List<GameObject> _activeChoiceButtons = new List<GameObject>();

    void Awake()
    {
        // Hide panel by default when the game starts
        HideDialogue();
    }

    /// <summary>
    /// Displays a single node's text and creates the choice buttons.
    /// </summary>
    public void DisplayNode(DialogueNode node, System.Action<DialogueChoice> onChoiceSelected)
    {
        ShowDialogue(true);

        // Update Text
        if (speakerNameText != null) 
            speakerNameText.text = string.IsNullOrEmpty(node.speakerName) ? "" : node.speakerName;
            
        if (dialogueText != null) 
            dialogueText.text = node.dialogueText;

        // Clear old buttons
        ClearChoices();

        // Spawn new buttons for each choice
        foreach (var choice in node.choices)
        {
            GameObject newButtonObj = Instantiate(choiceButtonPrefab, choicesContainer);
            newButtonObj.SetActive(true); // Safety: Force the button to be active even if the prefab is disabled
            _activeChoiceButtons.Add(newButtonObj);

            // Set button text
            TextMeshProUGUI btnText = newButtonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = choice.choiceText;
            }

            // Hook up the click event
            Button btn = newButtonObj.GetComponent<Button>();
            if (btn != null)
            {
                // We have to cache the choice variable for the lambda
                DialogueChoice currentChoice = choice;
                btn.onClick.AddListener(() => onChoiceSelected(currentChoice));
            }
        }
    }

    public void ShowDialogue(bool show)
    {
        dialoguePanel.SetActive(show);
        
        // Toggle choices background
        if (choicesGroup != null)
        {
            choicesGroup.SetActive(show);
        }

        // Hide movement UI when dialogue is shown, and show it when dialogue is hidden
        if (movementUI != null)
        {
            movementUI.SetActive(!show);
        }
    }

    public void HideDialogue()
    {
        ShowDialogue(false);
        ClearChoices();
    }

    private void ClearChoices()
    {
        foreach (var btn in _activeChoiceButtons)
        {
            if (btn != null) Destroy(btn);
        }
        _activeChoiceButtons.Clear();
    }
}

using UnityEngine;

/// <summary>
/// Connects the STTGameController's evaluation events to the Dialogue System.
/// Attach this to the DialogueManager GameObject.
/// </summary>
public class STTDialogueAdapter : MonoBehaviour
{
    private void OnEnable()
    {
        STTGameController.OnSTTEvaluationComplete += HandleSTTResult;
    }

    private void OnDisable()
    {
        STTGameController.OnSTTEvaluationComplete -= HandleSTTResult;
    }

    private void HandleSTTResult(bool success, string spokenPhrase)
    {
        if (DialogueManager.Instance != null)
        {
            if (success)
            {
                Debug.Log($"[STTDialogueAdapter] STT Success for phrase: {spokenPhrase}. Advancing Dialogue.");
                DialogueManager.Instance.AdvanceDialogue();
            }
            else
            {
                Debug.Log($"[STTDialogueAdapter] STT Failed. Firing OnWrongAnswer.");
                // Here we could trigger a specific fail node, or let the player retry.
                // DialogueManager.Instance.HandleWrongAnswer();
            }
        }
    }
}

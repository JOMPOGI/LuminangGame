using UnityEngine;

/// <summary>
/// Connects the STTGameController's evaluation events to the Dialogue System.
/// Attach this to the DialogueManager GameObject.
/// </summary>
public class STTDialogueAdapter : MonoBehaviour
{
    private void OnEnable()
    {
        STTVoiceVisualizerAdapter.OnSTTEvaluationComplete += HandleSTTResult;
    }

    private void OnDisable()
    {
        STTVoiceVisualizerAdapter.OnSTTEvaluationComplete -= HandleSTTResult;
    }

    private void HandleSTTResult(bool success, string spokenPhrase, string prefixText)
    {
        // If TeachingOverlayPanel is currently handling the teaching UI flow, skip here to prevent double-calls
        if (TeachingOverlayPanel.Instance != null && TeachingOverlayPanel.Instance.gameObject.activeInHierarchy)
        {
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.PendingSTTChoice != null)
        {
            var choice = DialogueManager.Instance.PendingSTTChoice;
            string expectedWord = choice.expectedSTTWord;

            if (success)
            {
                Debug.Log($"[STTDialogueAdapter] STT Success for expected phrase: '{expectedWord}'. Spoken: '{spokenPhrase}'. Advancing Dialogue.");
                DialogueManager.Instance.CompleteSTT(true, prefixText);
            }
            else
            {
                Debug.Log($"[STTDialogueAdapter] STT Failed for expected phrase: '{expectedWord}'. Spoken: '{spokenPhrase}'.");
                DialogueManager.Instance.CompleteSTT(false, prefixText);
            }
        }
    }
}

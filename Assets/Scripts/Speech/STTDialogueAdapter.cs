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
        if (DialogueManager.Instance != null && DialogueManager.Instance.PendingSTTChoice != null)
        {
            var choice = DialogueManager.Instance.PendingSTTChoice;
            string expectedWord = choice.expectedSTTWord;

            bool isMatch = false;

            if (success)
            {
                if (string.IsNullOrEmpty(expectedWord))
                {
                    isMatch = true; // No specific word expected, any success is fine.
                }
                else
                {
                    // Check if the spoken phrase contains or matches the expected word
                    isMatch = spokenPhrase.Trim().Equals(expectedWord.Trim(), System.StringComparison.OrdinalIgnoreCase);
                }
            }

            if (isMatch)
            {
                Debug.Log($"[STTDialogueAdapter] STT Success for expected phrase: {expectedWord}. Advancing Dialogue.");
                DialogueManager.Instance.CompleteSTT(true, prefixText);
            }
            else
            {
                Debug.Log($"[STTDialogueAdapter] STT Failed or word did not match expected: '{expectedWord}'. Heard: '{spokenPhrase}'. Firing OnWrongAnswer.");
                DialogueManager.Instance.CompleteSTT(false, prefixText);
            }
        }
    }
}

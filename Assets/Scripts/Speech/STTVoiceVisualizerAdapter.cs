using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Luminang.UI.Minigames;
using System;

/// <summary>
/// Bridges the Rush Game's VoiceVisualizer UI button with the STT engine
/// so the player can physically tap the mic to start speaking during dialogue.
/// Spawns a dynamic UI panel to show STT feedback (Transcript, Accuracy, Feedback).
/// </summary>
public class STTVoiceVisualizerAdapter : MonoBehaviour
{
    // Delegate now passes (bool success, string rawTranscript, string prefixText)
    public static Action<bool, string, string> OnSTTEvaluationComplete;
    
    public VoiceVisualizer visualizer;
    public bool isRecording { get; private set; } = false;

    private void Start()
    {
        EnsureDependencies();

        if (visualizer == null)
            visualizer = GetComponent<VoiceVisualizer>();
            
        if (visualizer != null)
        {
            // Clear old minigame listeners just in case
            Button btn = visualizer.GetComponentInChildren<Button>(true);
            if (btn != null)
            {
                btn.onClick.RemoveListener(OnMicClicked);
                btn.onClick.AddListener(OnMicClicked);
            }
        }

        // Hide by default until STT is needed
        gameObject.SetActive(false);
    }

    private void EnsureDependencies()
    {
        if (SpeechRecorder.Instance == null && FindFirstObjectByType<SpeechRecorder>() == null)
        {
            new GameObject("SpeechRecorder").AddComponent<SpeechRecorder>();
        }

        if (GroqWhisperManager.Instance == null && FindFirstObjectByType<GroqWhisperManager>() == null)
        {
            new GameObject("GroqWhisperManager").AddComponent<GroqWhisperManager>();
        }

        if (PhraseEvaluator.Instance == null && FindFirstObjectByType<PhraseEvaluator>() == null)
        {
            new GameObject("PhraseEvaluator").AddComponent<PhraseEvaluator>();
        }
    }

    private void OnEnable()
    {
        OnSTTEvaluationComplete += HandleResult;
    }

    private void OnDisable()
    {
        OnSTTEvaluationComplete -= HandleResult;
        if (isRecording)
        {
            StopRecording();
        }
    }

    public void ShowAndPrepare()
    {
        gameObject.SetActive(true);

        if (visualizer != null)
        {
            visualizer.SetReady(true);
        }
    }

    public void OnMicClicked()
    {
        if (!isRecording)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    private void StartRecording()
    {
        isRecording = true;
        
        if (DialogueManager.Instance != null && DialogueManager.Instance.uiController != null)
        {
            DialogueManager.Instance.uiController.UpdateSTTStatus("<color=green>Listening...</color>");
        }
        
        // Start the visualizer UI animations if attached
        if (visualizer != null) visualizer.StartListening();
        
        // Start the actual STT recording pipeline
        SpeechRecorder.Instance.StartRecording();
    }

    private void StopRecording()
    {
        isRecording = false;
        if (visualizer != null) visualizer.StopListening();
        
        string filePath = SpeechRecorder.Instance.StopRecording();
        if (!string.IsNullOrEmpty(filePath))
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.uiController != null)
            {
                DialogueManager.Instance.uiController.UpdateSTTStatus("<color=yellow>Processing...</color>");
            }
                
            GroqWhisperManager.Instance.Transcribe(filePath, OnTranscriptionSuccess, OnTranscriptionError);
        }
        else
        {
            if (DialogueManager.Instance != null && DialogueManager.Instance.uiController != null)
            {
                DialogueManager.Instance.uiController.UpdateSTTStatus("<color=red>Recording failed. Try again.</color>");
            }
        }
    }

    private void OnTranscriptionSuccess(string result)
    {
        PhraseEvaluator.Instance.FindBestMatch(result, (bestEntry, bestLang, accuracy, isEnglish) =>
        {
            if (bestEntry != null && isEnglish)
            {
                string prefix = $"<color=#00FFFF>You said:</color> \"{result}\"\n\n";
                OnSTTEvaluationComplete?.Invoke(false, result, prefix);
            }
            else if (bestEntry != null)
            {
                string prefix = $"<color=#00FFFF>You said:</color> \"{result}\" (<color=yellow>{accuracy:F0}% Match</color>)\n\n";
                // Only consider it a success if accuracy >= 80%
                bool success = accuracy >= 80f;
                OnSTTEvaluationComplete?.Invoke(success, result, prefix);
            }
            else
            {
                // Unrecognized completely
                string prefix = $"<color=#00FFFF>You said:</color> \"{result}\"\n\n";
                OnSTTEvaluationComplete?.Invoke(false, result, prefix);
            }
        });
    }

    private void OnTranscriptionError(string error)
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.uiController != null)
        {
            DialogueManager.Instance.uiController.UpdateSTTStatus($"<color=red>Error! {error}</color>");
        }
    }

    private void HandleResult(bool success, string phrase, string prefix)
    {
        if (success)
        {
            // Give them 2.5 seconds to read the positive feedback before hiding and progressing dialogue
            Invoke(nameof(HideMic), 2.5f);
        }
    }

    private void HideMic()
    {
        gameObject.SetActive(false);
    }
}

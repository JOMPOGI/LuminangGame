using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class STTGameController : MonoBehaviour
{
    public static STTGameController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Button micButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private TextMeshProUGUI transcriptText;
    [SerializeField] private GameObject listeningIndicator;
    [SerializeField] private Button retryButton;

    private bool _isRecording = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        micButton.onClick.AddListener(OnMicButtonClicked);
        retryButton.onClick.AddListener(ResetUI);
        ResetUI();
    }

    private void OnMicButtonClicked()
    {
        if (!_isRecording)
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
        _isRecording = true;
        SpeechRecorder.Instance.StartRecording();
        statusText.text = "Listening...";
        listeningIndicator.SetActive(true);
        micButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
    }

    private void StopRecording()
    {
        _isRecording = false;
        listeningIndicator.SetActive(false);
        micButton.GetComponentInChildren<TextMeshProUGUI>().text = "Speak";
        
        string filePath = SpeechRecorder.Instance.StopRecording();
        if (!string.IsNullOrEmpty(filePath))
        {
            ProcessSpeech(filePath);
        }
        else
        {
            statusText.text = "Recording failed. Try again.";
        }
    }

    private void ProcessSpeech(string filePath)
    {
        statusText.text = "Processing...";
        GroqWhisperManager.Instance.Transcribe(filePath, OnTranscriptionSuccess, OnTranscriptionError);
    }

    private void OnTranscriptionSuccess(string result)
    {
        transcriptText.text = $"Heard: \"{result}\"";
        
        // 1. Strict Validation (User-friendly message)
        if (!LexiconValidator.Instance.ValidateText(result))
        {
            statusText.text = "Keep trying!";
            accuracyText.text = "---";
            feedbackText.text = "That word doesn't sound familiar.";
            retryButton.gameObject.SetActive(true);
            return;
        }

        // 2. Auto-Discovery
        statusText.text = "Checking...";
        var (bestEntry, bestLang, accuracy, isEnglish) = PhraseEvaluator.Instance.FindBestMatch(result);

        if (bestEntry != null && accuracy >= 60f) 
        {
            if (isEnglish)
            {
                statusText.text = "That's English!";
                accuracyText.text = "---";
                feedbackText.text = "Try again!";
                retryButton.gameObject.SetActive(true);
            }
            else
            {
                string matchedText = bestEntry.GetPhrase(bestLang);
                string feedback = PhraseEvaluator.Instance.GetFeedback(accuracy);

                statusText.text = $"Detected: {bestLang.ToUpper()}";
                accuracyText.text = $"{accuracy:F0}% Match";
                feedbackText.text = $"{feedback}!";
                
                // Hide retry button only on high accuracy
                retryButton.gameObject.SetActive(accuracy < 85f);
            }
        }
        else
        {
            statusText.text = "Didn't catch that!";
            accuracyText.text = "---";
            feedbackText.text = "Let's try that again!";
            retryButton.gameObject.SetActive(true);
        }
    }

    private void OnTranscriptionError(string error)
    {
        statusText.text = "Error: " + error;
        retryButton.gameObject.SetActive(true);
    }

    private void ResetUI()
    {
        accuracyText.text = "";
        feedbackText.text = "";
        transcriptText.text = "";
        statusText.text = "Ready to listen...";
        retryButton.gameObject.SetActive(false);
        listeningIndicator.SetActive(false);
    }
}

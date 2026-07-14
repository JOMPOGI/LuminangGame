using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class STTGameController : MonoBehaviour
{
    public static Action<bool, string> OnSTTEvaluationComplete;
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

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
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

    public void SelectIlokano() 
    {
        PhraseEvaluator.Instance.SetRegion(RegionMode.Ilokano);
        statusText.text = "Region: ILOKANO Ready";
        ResetUIResults();
    }

    public void SelectCebuano() 
    {
        PhraseEvaluator.Instance.SetRegion(RegionMode.Cebuano);
        statusText.text = "Region: CEBUANO Ready";
        ResetUIResults();
    }

    public void SelectBossBattle() 
    {
        PhraseEvaluator.Instance.SetRegion(RegionMode.BossBattle);
        statusText.text = "BOSS BATTLE: All Languages Active";
        ResetUIResults();
    }

    private void ResetUIResults()
    {
        accuracyText.text = "";
        feedbackText.text = "";
        transcriptText.text = "";
    }

    private void OnTranscriptionSuccess(string result)
    {
        statusText.text = "Processing Results...";

        PhraseEvaluator.Instance.FindBestMatch(result, (bestEntry, bestLang, accuracy, isEnglish) =>
        {
            string transcript = $"Heard: \"{result}\"";

            if (bestEntry != null && isEnglish)
            {
                statusText.text = "English Detected!";
                accuracyText.text = "---";
                feedbackText.text = $"{transcript}\nPlease try saying it in {bestLang.ToUpper()}!";
                retryButton.gameObject.SetActive(true);
            }
            else if (bestEntry != null)
            {
                string feedback = PhraseEvaluator.Instance.GetFeedback(accuracy);
                statusText.text = $"{bestLang.ToUpper()} Detected";
                accuracyText.text = $"{accuracy:F0}% Match";
                
                feedbackText.text = $"{transcript}\n{feedback}";
                retryButton.gameObject.SetActive(accuracy < 80f);
                OnSTTEvaluationComplete?.Invoke(accuracy >= 80f, bestEntry.GetPhrase(bestLang));
            }
            else
            {
                statusText.text = "No Match Found";
                accuracyText.text = "---";
                feedbackText.text = $"{transcript}\nPlease try again!";
                retryButton.gameObject.SetActive(true);
                OnSTTEvaluationComplete?.Invoke(false, result);
            }
        });

        // Clear separate transcript text to prevent overlap
        transcriptText.text = ""; 
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

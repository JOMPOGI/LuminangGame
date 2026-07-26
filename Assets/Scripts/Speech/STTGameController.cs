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
        if (statusText != null) statusText.text = "Listening...";
        if (listeningIndicator != null) listeningIndicator.SetActive(true);
        if (micButton != null) micButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
    }

    private void StopRecording()
    {
        _isRecording = false;
        if (listeningIndicator != null) listeningIndicator.SetActive(false);
        if (micButton != null) micButton.GetComponentInChildren<TextMeshProUGUI>().text = "Speak";
        
        string filePath = SpeechRecorder.Instance.StopRecording();
        if (!string.IsNullOrEmpty(filePath))
        {
            ProcessSpeech(filePath);
        }
        else
        {
            if (statusText != null) statusText.text = "Recording failed. Try again.";
        }
    }

    private void ProcessSpeech(string filePath)
    {
        if (statusText != null) statusText.text = "Processing...";
        
        string langCode = "";
        if (PhraseEvaluator.Instance.CurrentRegion == RegionMode.Ilokano)
            langCode = "tl"; // Tagalog/Filipino as fallback for regional languages
        else if (PhraseEvaluator.Instance.CurrentRegion == RegionMode.Cebuano)
            langCode = "ceb"; // Cebuano
        
        GroqWhisperManager.Instance.Transcribe(filePath, OnTranscriptionSuccess, OnTranscriptionError, "", langCode);
    }

    public void SelectIlokano() 
    {
        PhraseEvaluator.Instance.SetRegion(RegionMode.Ilokano);
        if (statusText != null) statusText.text = "Region: ILOKANO Ready";
        ResetUIResults();
    }

    public void SelectCebuano() 
    {
        PhraseEvaluator.Instance.SetRegion(RegionMode.Cebuano);
        if (statusText != null) statusText.text = "Region: CEBUANO Ready";
        ResetUIResults();
    }

    public void SelectBossBattle() 
    {
        PhraseEvaluator.Instance.SetRegion(RegionMode.BossBattle);
        if (statusText != null) statusText.text = "Region: BOSS BATTLE Ready";
        ResetUIResults();
    }

    private void ResetUIResults()
    {
        if (accuracyText != null) accuracyText.text = "";
        if (feedbackText != null) feedbackText.text = "";
        if (transcriptText != null) transcriptText.text = "";
    }

    private void OnTranscriptionSuccess(string result)
    {
        if (statusText != null) statusText.text = "Processing Results...";

        PhraseEvaluator.Instance.FindBestMatch(result, (bestEntry, bestLang, accuracy, isEnglish, matchResult) =>
        {
            string transcript = $"Heard: \"{result}\"";

            if (bestEntry != null && isEnglish)
            {
                if (statusText != null) statusText.text = "English Detected!";
                if (accuracyText != null) accuracyText.text = "---";
                if (feedbackText != null) feedbackText.text = $"{transcript}\nPlease try saying it in {bestLang.ToUpper()}!";
                if (retryButton != null) retryButton.gameObject.SetActive(true);
            }
            else if (bestEntry != null)
            {
                if (matchResult == "uncertain")
                {
                    if (statusText != null) statusText.text = "Unclear Speech";
                    if (accuracyText != null) accuracyText.text = $"{accuracy:F0}% Match";
                    if (feedbackText != null) feedbackText.text = $"{transcript}\nI couldn't quite catch that. Please speak a bit more clearly.";
                    if (retryButton != null) retryButton.gameObject.SetActive(true);
                    // Do not invoke OnSTTEvaluationComplete so quest doesn't fail immediately
                }
                else
                {
                    string feedback = PhraseEvaluator.Instance.GetFeedback(accuracy);
                    if (statusText != null) statusText.text = $"{bestLang.ToUpper()} Detected";
                    if (accuracyText != null) accuracyText.text = $"{accuracy:F0}% Match";
                    
                    if (feedbackText != null) feedbackText.text = $"{transcript}\n{feedback}";
                    bool passed = matchResult == "pass" || matchResult == "correct" || accuracy >= 80f;
                    if (retryButton != null) retryButton.gameObject.SetActive(!passed);
                    OnSTTEvaluationComplete?.Invoke(passed, bestEntry.GetPhrase(bestLang));
                }
            }
            else
            {
                if (statusText != null) statusText.text = "No Match Found";
                if (accuracyText != null) accuracyText.text = "---";
                if (feedbackText != null) feedbackText.text = $"{transcript}\nPlease try again!";
                if (retryButton != null) retryButton.gameObject.SetActive(true);
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

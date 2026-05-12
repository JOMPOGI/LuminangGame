using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

    public void SelectMaranao() 
    {
        PhraseEvaluator.Instance.SetRegion(RegionMode.Maranao);
        statusText.text = "Region: MARANAO Ready";
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
        // 1. Lenient Validation
        if (!LexiconValidator.Instance.ValidateText(result))
        {
            Debug.Log("Some words not in lexicon, but proceeding with fuzzy match.");
        }

        // 2. Multi-Phrase Discovery
        statusText.text = "Processing Results...";
        var matches = PhraseEvaluator.Instance.FindAllMatches(result);

        if (matches.Count > 0) 
        {
            string transcript = $"Heard: \"{result}\"";
            
            // Build a list of all detected regional phrases
            string detectedLangs = string.Join(", ", matches.Select(m => m.language.ToUpper()).Distinct());
            string detectedPhrases = string.Join(", ", matches.Select(m => m.entry.GetPhrase(m.language)));
            float averageScore = matches.Average(m => m.score);

            statusText.text = $"{detectedLangs} Detected";
            accuracyText.text = $"{averageScore:F0}% Avg Match";
            feedbackText.text = $"{transcript}\nDetected: <color=yellow>{detectedPhrases}</color>";
            
            // Show retry button unless it's a passing score (80%+)
            retryButton.gameObject.SetActive(averageScore < 80f);
        }
        else
        {
            // Fallback to Best Match for English detection
            var (bestEntry, bestLang, accuracy, isEnglish) = PhraseEvaluator.Instance.FindBestMatch(result);
            string transcript = $"Heard: \"{result}\"";

            if (bestEntry != null && isEnglish)
            {
                statusText.text = "English Detected!";
                accuracyText.text = "---";
                feedbackText.text = $"{transcript}\nPlease try saying it in {bestLang.ToUpper()}!";
                retryButton.gameObject.SetActive(true);
            }
            else
            {
                statusText.text = "No Match Found";
                accuracyText.text = "---";
                feedbackText.text = $"{transcript}\nPlease try again!";
                retryButton.gameObject.SetActive(true);
            }
        }

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

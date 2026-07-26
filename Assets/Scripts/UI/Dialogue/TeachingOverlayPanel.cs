using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Full-screen teaching overlay shown during NPC word-teaching dialogues.
/// Displays a background image, prompt text, and a mic tap button (active/inactive).
/// Directly handles SpeechRecorder, GroqWhisperManager, and PhraseEvaluator (same as STTGameController).
/// </summary>
public class TeachingOverlayPanel : MonoBehaviour
{
    public static TeachingOverlayPanel Instance { get; private set; }

    [Header("Panel Root")]
    public CanvasGroup canvasGroup;

    [Header("Background")]
    public Image backgroundImage;
    public Sprite[] backgroundOptions;

    [Header("Prompt Text")]
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI tapToStopText;

    [Header("Mic Button")]
    public Button micButton;
    public Image micButtonImage;
    public Sprite micInactiveSprite;
    public Sprite micActiveSprite;

    [Header("Movement Controls")]
    public GameObject movementControls;

    [Header("Animation")]
    public float fadeDuration = 0.3f;

    // ── Private State ──────────────────────────────────────────────
    private bool _isRecording = false;
    private string _targetWord = "";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ─────────────────────────────────────────────────────────────────
    // Public API
    // ─────────────────────────────────────────────────────────────────

    public void ShowFromEvent(string eventName)
    {
        string autoWord = "";
        if (DialogueManager.Instance != null && DialogueManager.Instance.PendingSTTChoice != null)
        {
            autoWord = DialogueManager.Instance.PendingSTTChoice.expectedSTTWord;
        }

        string[] parts = eventName.Split(':');
        string bgName = "";
        string manualWord = "";

        if (parts.Length == 3)
        {
            manualWord = parts[1].Trim();
            bgName = parts[2].Trim();
        }
        else if (parts.Length == 2)
        {
            bgName = parts[1].Trim();
        }
        else if (parts.Length == 1)
        {
            bgName = parts[0].Trim();
        }

        string finalWord = !string.IsNullOrEmpty(autoWord) ? autoWord : manualWord;
        Show(finalWord, bgName);
    }

    public void ShowForPendingSTT(string backgroundName = "")
    {
        string word = "";
        if (DialogueManager.Instance != null && DialogueManager.Instance.PendingSTTChoice != null)
            word = DialogueManager.Instance.PendingSTTChoice.expectedSTTWord;
        Show(word, backgroundName);
    }

    public void Show(string word, string backgroundName = "")
    {
        _targetWord = word;
        _isRecording = false;

        EnsureSpeechEngineDependencies();

        if (backgroundImage != null && !string.IsNullOrEmpty(backgroundName))
        {
            Sprite found = FindBackground(backgroundName);
            if (found != null) backgroundImage.sprite = found;
        }

        ResetPromptText();

        if (tapToStopText != null)
            tapToStopText.gameObject.SetActive(false);

        SetMicState(false);

        if (micButton != null)
        {
            micButton.gameObject.SetActive(true);
            micButton.onClick.RemoveAllListeners();
            micButton.onClick.AddListener(OnMicButtonTapped);
        }

        HideMovementControls(true);

        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        if (!gameObject.activeInHierarchy) return;
        StopAllCoroutines();
        StartCoroutine(FadeOut());
    }

    private void ResetPromptText()
    {
        if (promptText != null)
        {
            promptText.text = string.IsNullOrEmpty(_targetWord)
                ? "Tap the mic and speak!"
                : $"Tap and speak the word <b>\"{_targetWord}\"</b> into the mic";
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Direct STT Flow (Matching STTGameController)
    // ─────────────────────────────────────────────────────────────────

    private void OnMicButtonTapped()
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
        SetMicState(true);

        if (tapToStopText != null)
        {
            tapToStopText.text = "Tap to stop";
            tapToStopText.gameObject.SetActive(true);
        }

        if (promptText != null)
            promptText.text = "Listening... Speak clearly!";

        if (SpeechRecorder.Instance != null)
        {
            SpeechRecorder.Instance.StartRecording();
        }

        Debug.Log($"[TeachingOverlayPanel] Started recording for word: '{_targetWord}'");
    }

    private void StopRecording()
    {
        _isRecording = false;
        SetMicState(false);

        if (tapToStopText != null)
            tapToStopText.gameObject.SetActive(false);

        if (promptText != null)
            promptText.text = "Processing your voice...";

        string filePath = "";
        if (SpeechRecorder.Instance != null)
        {
            filePath = SpeechRecorder.Instance.StopRecording();
        }

        Debug.Log($"[TeachingOverlayPanel] Stopped recording. Audio path: '{filePath}'");

        if (!string.IsNullOrEmpty(filePath))
        {
            GroqWhisperManager.Instance.Transcribe(filePath, OnTranscriptionSuccess, OnTranscriptionError);
        }
        else
        {
            if (promptText != null)
                promptText.text = "<color=#FF7777>Recording failed. Tap to try again.</color>";
        }
    }

    private void OnTranscriptionSuccess(string transcribedText)
    {
        Debug.Log($"[TeachingOverlayPanel] Transcribed speech: \"{transcribedText}\"");

        if (promptText != null)
            promptText.text = "Evaluating speech...";

        string target = !string.IsNullOrEmpty(_targetWord) ? _targetWord : 
            (DialogueManager.Instance != null && DialogueManager.Instance.PendingSTTChoice != null ? DialogueManager.Instance.PendingSTTChoice.expectedSTTWord : "");

        if (!string.IsNullOrEmpty(target) && PhraseEvaluator.Instance != null)
        {
            PhraseEvaluator.Instance.EvaluateSpeech(target, transcribedText, (transcript, scorePercent, evalResult) =>
            {
                bool success = scorePercent >= 80f;
                Debug.Log($"[TeachingOverlayPanel] Evaluation score: {scorePercent:F0}%. Result: {evalResult}");

                if (success)
                {
                    HandleSuccess(transcribedText);
                }
                else
                {
                    HandleFailure();
                }
            });
        }
        else if (PhraseEvaluator.Instance != null)
        {
            PhraseEvaluator.Instance.FindBestMatch(transcribedText, (bestEntry, bestLang, accuracy, isEnglish, matchResult) =>
            {
                bool success = accuracy >= 80f && !isEnglish;
                if (success)
                {
                    HandleSuccess(transcribedText);
                }
                else
                {
                    HandleFailure();
                }
            });
        }
    }

    private void HandleSuccess(string transcribedText)
    {
        Debug.Log($"<color=green>[TeachingOverlayPanel] HandleSuccess called for speech: '{transcribedText}'. Firing CompleteSTT(true)...</color>");

        // 1. Display success message on overlay prompt
        if (promptText != null)
            promptText.text = "<color=#55FF55><b>Great job! Correct!</b></color>";

        // 2. Hide the mic button so player uses NEXT>> on dialogue panel
        if (micButton != null)
            micButton.gameObject.SetActive(false);

        // 3. IMMEDIATELY complete STT in DialogueManager to load success node (e.g. Tiptip_Word_1_Success)
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.CompleteSTT(true);
        }

        // NOTE: We do NOT call Hide() here! Panel & background stay active during the success node!
    }

    private void HandleFailure()
    {
        // Clean failure message — reshow prompt with retry warning
        if (promptText != null)
            promptText.text = $"<color=#FF7777><b>Not quite! Try again.</b></color>\nTap and speak the word <b>\"{_targetWord}\"</b> into the mic";

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.CompleteSTT(false);
        }

        SetMicState(false);
    }

    private void OnTranscriptionError(string error)
    {
        Debug.LogError($"[TeachingOverlayPanel] Transcription Error: {error}");
        if (promptText != null)
            promptText.text = $"<color=#FF7777>Error: {error}</color>\nTap to try again.";

        SetMicState(false);
    }

    private void SetMicState(bool active)
    {
        if (micButtonImage == null) return;
        micButtonImage.sprite = active ? micActiveSprite : micInactiveSprite;
    }

    private void EnsureSpeechEngineDependencies()
    {
        if (SpeechRecorder.Instance == null && FindFirstObjectByType<SpeechRecorder>() == null)
            new GameObject("SpeechRecorder").AddComponent<SpeechRecorder>();

        if (GroqWhisperManager.Instance == null && FindFirstObjectByType<GroqWhisperManager>() == null)
            new GameObject("GroqWhisperManager").AddComponent<GroqWhisperManager>();

        if (PhraseEvaluator.Instance == null && FindFirstObjectByType<PhraseEvaluator>() == null)
            new GameObject("PhraseEvaluator").AddComponent<PhraseEvaluator>();
    }

    private Sprite FindBackground(string name)
    {
        if (backgroundOptions == null) return null;
        foreach (var sprite in backgroundOptions)
        {
            if (sprite != null && sprite.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return sprite;
        }
        return null;
    }

    private IEnumerator FadeIn()
    {
        if (canvasGroup == null) yield break;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        if (canvasGroup == null) { gameObject.SetActive(false); yield break; }
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        float elapsed = 0f;
        float startAlpha = canvasGroup.alpha;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        HideMovementControls(false);
    }

    private void HideMovementControls(bool hide)
    {
        if (movementControls == null)
            movementControls = GameObject.Find("Movement_Controls");

        if (movementControls != null)
        {
            if (hide) movementControls.SetActive(false);
            else
            {
                bool inDialogue = DialogueManager.Instance != null && DialogueManager.Instance.IsInDialogue;
                if (!inDialogue) movementControls.SetActive(true);
            }
        }
    }
}

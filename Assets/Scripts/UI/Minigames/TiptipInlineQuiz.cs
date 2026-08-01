using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the Tiptip floating pop-up quiz that appears in the corner of the screen
/// during the Ronnie conversation. Does NOT use a full-screen overlay — it is a small
/// animated card that slides in from the side.
///
/// HOW TO USE:
/// 1. This script lives on the TiptipQuizBubble prefab.
/// 2. Triggered by a dialogue event "StartTiptipQuiz:QuizId" (QuizId = "A", "B", or "C").
/// 3. TiptipInlineQuizData defines which quiz shows for each ID.
/// 4. Player taps one of 4 choice buttons → correct = RecordPopupCorrect, then dialogue resumes.
/// 5. Wrong choice: buttons flash red, correct button highlights, then dialogue resumes.
///
/// NOTE: Uses MinigameManager flow (StartMinigame) so dialogue pauses while quiz is shown.
/// The prefab should have a MinigameEndTrigger on the continue button (auto-dismissed after answer).
/// </summary>
public class TiptipInlineQuiz : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────
    // Inspector References
    // ─────────────────────────────────────────────────────────

    [Header("UI References")]
    [Tooltip("The question text shown to the player.")]
    public TextMeshProUGUI questionText;

    [Tooltip("The 4 choice buttons (must be exactly 4).")]
    public TiptipQuizChoice[] choiceButtons;

    [Tooltip("Tiptip's portrait image (optional, for character flavour).")]
    public Image tiptipPortrait;

    [Tooltip("Root CanvasGroup used for the slide-in animation.")]
    public CanvasGroup canvasGroup;

    [Tooltip("The RectTransform used to slide the panel in.")]
    public RectTransform panelRect;

    [Header("Animation")]
    [Tooltip("How far off-screen the panel starts (pixels). Negative = starts left.")]
    public float slideOffsetX = -400f;

    [Tooltip("Duration of the slide-in animation (seconds).")]
    public float slideInDuration = 0.35f;

    [Tooltip("Delay after player answers before auto-dismissing (seconds).")]
    public float autoDismissDelay = 1.2f;

    // ─────────────────────────────────────────────────────────
    // Built-in Quiz Data
    // ─────────────────────────────────────────────────────────

    [System.Serializable]
    public class QuizData
    {
        public string quizId;
        [TextArea] public string question;
        public string correctAnswer;
        public string[] wrongAnswers; // exactly 3
    }

    [Header("Quiz Data — wire A, B, C here")]
    public List<QuizData> quizzes = new List<QuizData>
    {
        new QuizData
        {
            quizId = "A",
            question = "Quick! Ronnie seems friendly.\nHow do you say 'good day' in Cebuano?",
            correctAnswer = "Maayong aldaw",
            wrongAnswers = new[] { "paalam", "maayo ra ko", "kumusta" }
        },
        new QuizData
        {
            quizId = "B",
            question = "Ronnie asked if you'd like to see his work.\nWhat do you say if you AGREE?",
            correctAnswer = "oo",
            wrongAnswers = new[] { "dili", "dili ko kasabot", "paalam" }
        },
        new QuizData
        {
            quizId = "C",
            question = "Ronnie helped you so much!\nHow do you say 'thank you for your help'?",
            correctAnswer = "salamat sa imong tabang",
            wrongAnswers = new[] { "pasayloa ko", "tabi", "daghang salamat" }
        }
    };

    // ─────────────────────────────────────────────────────────
    // State
    // ─────────────────────────────────────────────────────────

    private QuizData _activeQuiz;
    private bool _answered = false;
    private Vector2 _targetPos;

    // ─────────────────────────────────────────────────────────
    // Init
    // ─────────────────────────────────────────────────────────

    private void Start()
    {
        // Default to Quiz A if no explicit Load() called
        string quizId = MinigameManager.Instance != null ? MinigameManager.Instance.CurrentCategory : "A";
        Load(quizId);
        StartCoroutine(SlideIn());
    }

    /// <summary>
    /// Loads and displays a quiz by its ID.
    /// Called automatically from Start(), using MinigameManager.CurrentCategory as the ID.
    /// </summary>
    public void Load(string quizId)
    {
        _activeQuiz = quizzes.Find(q => q.quizId.Equals(quizId, System.StringComparison.OrdinalIgnoreCase));

        if (_activeQuiz == null)
        {
            Debug.LogWarning($"[TiptipInlineQuiz] Quiz '{quizId}' not found. Using first quiz.");
            _activeQuiz = quizzes.Count > 0 ? quizzes[0] : null;
            if (_activeQuiz == null) return;
        }

        if (questionText != null)
            questionText.text = _activeQuiz.question;

        // Build shuffled tile list: 1 correct + 3 wrong
        List<string> options = new List<string>(_activeQuiz.wrongAnswers);
        options.Add(_activeQuiz.correctAnswer);
        Shuffle(options);

        for (int i = 0; i < choiceButtons.Length && i < options.Count; i++)
        {
            bool isCorrect = options[i] == _activeQuiz.correctAnswer;
            choiceButtons[i].Setup(options[i], isCorrect, OnChoiceSelected);
        }

        _answered = false;
    }

    // ─────────────────────────────────────────────────────────
    // Answer Handling
    // ─────────────────────────────────────────────────────────

    private void OnChoiceSelected(TiptipQuizChoice chosen)
    {
        if (_answered) return;
        _answered = true;

        // Disable all buttons immediately
        foreach (var btn in choiceButtons)
            btn.SetInteractable(false);

        if (chosen.IsCorrect)
        {
            chosen.PlayCorrect();
            if (ConversationTestManager.Instance != null)
                ConversationTestManager.Instance.RecordPopupCorrect();
            StartCoroutine(AutoDismiss());
        }
        else
        {
            chosen.PlayWrong();
            // Reveal the correct answer
            foreach (var btn in choiceButtons)
                if (btn.IsCorrect) btn.PlayCorrect();
            StartCoroutine(AutoDismiss());
        }
    }

    // ─────────────────────────────────────────────────────────
    // Animations
    // ─────────────────────────────────────────────────────────

    private IEnumerator SlideIn()
    {
        if (panelRect == null) yield break;

        _targetPos = panelRect.anchoredPosition;
        panelRect.anchoredPosition = new Vector2(slideOffsetX, _targetPos.y);

        if (canvasGroup != null) canvasGroup.alpha = 0f;

        float elapsed = 0f;
        while (elapsed < slideInDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / slideInDuration);
            panelRect.anchoredPosition = new Vector2(Mathf.Lerp(slideOffsetX, _targetPos.x, t), _targetPos.y);
            if (canvasGroup != null) canvasGroup.alpha = t;
            yield return null;
        }

        panelRect.anchoredPosition = _targetPos;
        if (canvasGroup != null) canvasGroup.alpha = 1f;
    }

    private IEnumerator AutoDismiss()
    {
        yield return new WaitForSeconds(autoDismissDelay);
        if (MinigameManager.Instance != null)
            MinigameManager.Instance.HideMinigame();
        else
            Destroy(gameObject);
    }

    // ─────────────────────────────────────────────────────────
    // Helpers
    // ─────────────────────────────────────────────────────────

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

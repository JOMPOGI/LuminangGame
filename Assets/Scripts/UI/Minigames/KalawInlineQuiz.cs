using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls Kalaw's floating pop-up quiz in Calle Crisologo.
/// Mirrors TiptipInlineQuiz from Magellan's Cross, but uses Ilocano vocabulary
/// matching the Calle Crisologo Level 1-3 curriculum.
///
/// HOW TO USE:
/// 1. This script lives on the KalawQuizBubble prefab (cloned from TiptipQuizBubble).
/// 2. Triggered by a dialogue event "StartTiptipQuiz:A" (A, B, C, D, E = different quests).
/// 3. Player taps one of 4 choice buttons — correct = quiz dismissed, dialogue resumes.
/// 4. Wrong choice: buttons flash red, correct answer highlighted, then auto-dismissed.
/// </summary>
public class KalawInlineQuiz : MonoBehaviour
{
    // ─────────────────────────────────────────────────────────
    // Inspector References
    // ─────────────────────────────────────────────────────────

    [Header("UI References")]
    [Tooltip("The question text shown to the player.")]
    public TextMeshProUGUI questionText;

    [Tooltip("The 4 choice buttons (must be exactly 4).")]
    public TiptipQuizChoice[] choiceButtons;

    [Tooltip("Kalaw's portrait image (optional).")]
    public Image kalawPortrait;

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
    // Ilocano Quiz Data (Calle Crisologo Curriculum)
    // ─────────────────────────────────────────────────────────

    [System.Serializable]
    public class QuizData
    {
        public string quizId;
        [TextArea] public string question;
        public string correctAnswer;
        public string[] wrongAnswers; // exactly 3
    }

    [Header("Quiz Data — Ilocano curriculum (A=Greetings W1-W4, B=Requests W23-W27, C=Directions W28-W37, D=ActionVerbs W48-W55, E=Interrogatives W75-W81)")]
    public List<QuizData> quizzes = new List<QuizData>
    {
        // A — Kalaw's Quiz: tests Words 1–4 (kumusta, kumusta ka, nasayaat ak, naimbag a bigat)
        // Fires AFTER Kalaw_W04_Success per curriculum doc
        new QuizData
        {
            quizId = "A",
            question = "Kalaw is testing you!\nHow do you say 'Good morning' in Ilocano?",
            correctAnswer = "naimbag a bigat",
            wrongAnswers = new[] { "naimbag a malem", "naimbag a rabii", "naimbag nga aldaw" }
        },
        // B — Lito's Quiz: tests Requests W23–W27
        // Fires AFTER Lito teaches W27 (mabalin kadi agsaludsod)
        new QuizData
        {
            quizId = "B",
            question = "Lito is testing your requests!\nHow do you say 'Please wait' in Ilocano?",
            correctAnswer = "urayennak",
            wrongAnswers = new[] { "tulunganak", "mabalin kadi agsaludsod", "ikanmo man" }
        },
        // C — Klara's Quiz: tests Directions W28–W37
        // Fires AFTER Klara teaches W37 (uray ditoy)
        new QuizData
        {
            quizId = "C",
            question = "Klara is testing your directions!\nHow do you say 'Go straight' in Ilocano?",
            correctAnswer = "agdiretso",
            wrongAnswers = new[] { "agliko iti kannigid", "agliko iti kannawan", "agsardeng ditoy" }
        },
        // D — After Action Verbs W48–W55 (Rayo / Aling Rosa)
        new QuizData
        {
            quizId = "D",
            question = "After a long walk, you are hungry!\nHow do you say 'eat' in Ilocano?",
            correctAnswer = "mangan",
            wrongAnswers = new[] { "uminom", "maturog", "agsao" }
        },
        // E — After Interrogatives W75–W81 (Lola Bebang)
        new QuizData
        {
            quizId = "E",
            question = "Lola Bebang has one last question!\nHow do you say 'how many' in Ilocano?",
            correctAnswer = "mano",
            wrongAnswers = new[] { "ania", "asino", "kaano" }
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
        string quizId = MinigameManager.Instance != null ? MinigameManager.Instance.CurrentCategory : "A";
        Load(quizId);
        StartCoroutine(SlideIn());
    }

    public void Load(string quizId)
    {
        _activeQuiz = quizzes.Find(q => q.quizId.Equals(quizId, System.StringComparison.OrdinalIgnoreCase));

        if (_activeQuiz == null)
        {
            Debug.LogWarning($"[KalawInlineQuiz] Quiz '{quizId}' not found. Using first quiz.");
            _activeQuiz = quizzes.Count > 0 ? quizzes[0] : null;
            if (_activeQuiz == null) return;
        }

        if (questionText != null)
            questionText.text = _activeQuiz.question;

        // Build shuffled options: 1 correct + 3 wrong
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

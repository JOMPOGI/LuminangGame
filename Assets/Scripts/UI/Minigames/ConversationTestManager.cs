using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Tracks player performance across any "Converse with NPC" proficiency test.
/// 
/// DESIGN: DontDestroyOnLoad singleton so it can be reused across scenes/chapters.
/// 
/// HOW TO USE:
/// 1. Wire Ronnie_Rigged's Dialogue Events to call:
///    "ConversationTest_Start"      — resets counters (fires at test start)
///    "ConversationTest_Correct"    — increments correct count (fire on success nodes' endEventName)
///    "ConversationTest_Wrong"      — increments wrong count   (fire on wrong nodes' endEventName)
///    "ConversationTest_Evaluate"   — reads score, fires outcome events (fire on final node)
///
/// 2. Listen to OnExcellent / OnGood / OnNeedsPractice UnityEvents in the Inspector
///    to route Ronnie's final dialogue branch.
/// </summary>
public class ConversationTestManager : MonoBehaviour
{
    public static ConversationTestManager Instance { get; private set; }

    // ─────────────────────────────────────────────────────────
    // Inspector
    // ─────────────────────────────────────────────────────────

    [Header("Evaluation Settings")]
    [Tooltip("Number of correct answers required for Excellent rating")]
    public int excellentThreshold = 8;
    
    [Tooltip("Number of correct answers required for Good rating")]
    public int goodThreshold = 5;

    [Header("Prefabs")]
    [Tooltip("Reference to the TiptipQuizBubble prefab for pop-up quizzes.")]
    public GameObject tiptipQuizPrefab;

    [Header("Outcome Dialogue Nodes")]
    [Tooltip("Drag Ronnie_Test_End_Excellent here. Dialogue auto-routes to this node on Excellent result.")]
    public DialogueNode excellentNode;

    [Tooltip("Drag Ronnie_Test_End_Good here. Dialogue auto-routes to this node on Good result.")]
    public DialogueNode goodNode;

    [Tooltip("Drag Ronnie_Test_End_Retry here. Dialogue auto-routes to this node on Needs Practice result.")]
    public DialogueNode retryNode;

    [Header("Unity Events (Optional Hooks)")]
    public UnityEvent onExcellent;
    public UnityEvent onGood;
    public UnityEvent onNeedsPractice;

    // ─────────────────────────────────────────────────────────
    // State
    // ─────────────────────────────────────────────────────────

    public int CorrectCount  { get; private set; }
    public int WrongCount    { get; private set; }
    public int PopupCorrect  { get; private set; }  // bonus tracking for pop-up quizzes
    public string ActiveTestId { get; private set; }

    public string LastOutcome { get; private set; } // "Excellent", "Good", "NeedsPractice"

    // ─────────────────────────────────────────────────────────
    // Lifecycle
    // ─────────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ─────────────────────────────────────────────────────────
    // Public API — called by dialogue events via HandleDialogueEvent
    // ─────────────────────────────────────────────────────────

    /// <summary>Resets all counters. Call at the beginning of a new test.</summary>
    public void StartTest(string testId = "")
    {
        ActiveTestId = testId;
        CorrectCount = 0;
        WrongCount   = 0;
        PopupCorrect = 0;
        LastOutcome  = "";
        Debug.Log($"[ConversationTestManager] Test started: '{testId}'");
    }

    /// <summary>Call when the player answers a main STT/choice moment correctly.</summary>
    public void RecordCorrect()
    {
        CorrectCount++;
        Debug.Log($"[ConversationTestManager] ✓ Correct! Total: {CorrectCount}");
    }

    /// <summary>Call when the player answers a main moment incorrectly.</summary>
    public void RecordWrong()
    {
        WrongCount++;
        Debug.Log($"[ConversationTestManager] ✗ Wrong. Total wrong: {WrongCount}");
    }

    /// <summary>Call when the player answers a Tiptip pop-up quiz correctly (bonus tracking).</summary>
    public void RecordPopupCorrect()
    {
        PopupCorrect++;
        Debug.Log($"[ConversationTestManager] 💭 Pop-up quiz correct! Total: {PopupCorrect}");
    }

    /// <summary>
    /// Evaluates the accumulated score and fires the appropriate outcome event.
    /// Also stores the outcome in LastOutcome for dialogue node branching.
    /// </summary>
    public void Evaluate()
    {
        int total = CorrectCount + WrongCount;
        Debug.Log($"[ConversationTestManager] EVALUATE — Correct: {CorrectCount}, Wrong: {WrongCount}, Total: {total}");

        DialogueNode targetNode = null;

        if (CorrectCount >= excellentThreshold)
        {
            LastOutcome = "Excellent";
            targetNode = excellentNode;
            Debug.Log("[ConversationTestManager] → Outcome: EXCELLENT");
            onExcellent?.Invoke();
        }
        else if (CorrectCount >= goodThreshold)
        {
            LastOutcome = "Good";
            targetNode = goodNode;
            Debug.Log("[ConversationTestManager] → Outcome: GOOD");
            onGood?.Invoke();
        }
        else
        {
            LastOutcome = "NeedsPractice";
            targetNode = retryNode;
            Debug.Log("[ConversationTestManager] → Outcome: NEEDS PRACTICE");
            onNeedsPractice?.Invoke();
        }

        // Auto-route: jump dialogue directly to the outcome node
        if (targetNode != null && DialogueManager.Instance != null)
        {
            Debug.Log($"[ConversationTestManager] Auto-routing dialogue to '{targetNode.name}'");
            DialogueManager.Instance.JumpToNode(targetNode);
        }
        else if (targetNode == null)
        {
            Debug.LogWarning("[ConversationTestManager] No outcome DialogueNode assigned! Drag the correct node into the Inspector.");
        }
    }

    /// <summary>
    /// Handles dialogue event strings forwarded from DialogueManager.
    /// Events: ConversationTest_Start, ConversationTest_Correct, ConversationTest_Wrong,
    ///         ConversationTest_PopupCorrect, ConversationTest_Evaluate,
    ///         ConversationTest_Start:TestId (colon-separated test ID variant)
    /// </summary>
    public void HandleEvent(string eventName)
    {
        if (string.IsNullOrEmpty(eventName)) return;

        string lower = eventName.ToLower();

        if (lower.StartsWith("conversationtest_start"))
        {
            string testId = eventName.Contains(":") ? eventName.Split(':')[1].Trim() : "unnamed";
            StartTest(testId);
        }
        else if (lower == "conversationtest_correct")
        {
            RecordCorrect();
        }
        else if (lower == "conversationtest_wrong")
        {
            RecordWrong();
        }
        else if (lower == "conversationtest_popupcorrect")
        {
            RecordPopupCorrect();
        }
        else if (lower == "conversationtest_evaluate")
        {
            Evaluate();
        }
    }

    /// <summary>
    /// Called via UnityEvent from DialogueNode. 
    /// Takes a single string (A, B, or C) so it works in the Unity Inspector dropdown.
    /// </summary>
    public void ShowTiptipQuiz(string quizId)
    {
        if (MinigameManager.Instance != null && tiptipQuizPrefab != null)
        {
            MinigameManager.Instance.StartMinigameWithCategory(tiptipQuizPrefab, quizId, 0);
        }
        else
        {
            Debug.LogWarning("[ConversationTestManager] Cannot show quiz. Missing MinigameManager or TiptipQuizPrefab.");
        }
    }
}

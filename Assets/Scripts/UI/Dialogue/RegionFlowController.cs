using System.Collections;
using UnityEngine;

// Updated RegionMode enum to match other project definitions
public enum RegionMode { Ilokano, Cebuano, BossBattle }

/// <summary>
/// Orchestrates the full region experience using existing managers.
/// Flow: Cutscene → Lesson UI → Speech‑to‑Text evaluation → (optional) Minigame → Quest → Crystal trial → Journal update.
/// Calls only methods that exist in the codebase; missing features are marked with TODO comments.
/// </summary>
public class RegionFlowController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private LessonManager lessonManager;
    [SerializeField] private MinigameManager minigameManager;
    [SerializeField] private JournalManager journalManager;
    [SerializeField] private ObjectiveManager objectiveManager;
    [SerializeField] private CutscenePlayer cutscenePlayer;

    private Coroutine _flowRoutine;

    /// <summary>
    /// Starts the region flow for the given mode.
    /// </summary>
    public void StartRegion(RegionMode mode)
    {
        if (_flowRoutine != null)
            StopCoroutine(_flowRoutine);
        _flowRoutine = StartCoroutine(RegionFlowRoutine(mode));
    }

    private IEnumerator RegionFlowRoutine(RegionMode mode)
    {
        // 1. Play cutscene (if any)
        if (cutscenePlayer != null)
            yield return StartCoroutine(cutscenePlayer.Play());
        else
            yield return null;

        // 2. Show lesson UI for this region
        if (lessonManager != null)
        {
            // LessonManager shows a lesson panel by category name.
            lessonManager.ShowLessonWithCategory(mode.ToString());
        }
        else
        {
            Debug.LogWarning("[RegionFlowController] LessonManager reference missing.");
        }

        // 3. Wait for STT evaluation result (success flag).
        bool sttSuccess = false;
        void OnSTTResult(bool success, string _) => sttSuccess = success;
        STTGameController.OnSTTEvaluationComplete += OnSTTResult;
        while (!sttSuccess)
            yield return null;
        STTGameController.OnSTTEvaluationComplete -= OnSTTResult;

        // 4. Advance dialogue if needed.
        DialogueManager.Instance?.AdvanceDialogue();

        // 5. Optional minigame – placeholder.
        // TODO: Determine the appropriate minigame prefab for this region and launch it.
        // Example (requires a prefab reference):
        // yield return minigameManager.StartMinigameWithCategory(minigamePrefab, mode.ToString(), /*languageId*/ 1);

        // 6. Start quest for this lesson – placeholder.
        // TODO: Implement quest start logic via ObjectiveManager.
        // Example: objectiveManager.SetCounterObjective("Find Items ; 3 ; Quest Complete");

        // 7. Play crystal trial – placeholder coroutine.
        yield return PlayCrystalTrial(mode);

        // 8. Update journal – placeholder.
        // TODO: Record progress in JournalManager, e.g., unlocking lore.
        // Example: journalManager.UnlockLore($"Region_{mode}");

        Debug.Log($"[RegionFlowController] Completed flow for {mode}");
    }

    private IEnumerator PlayCrystalTrial(RegionMode mode)
    {
        Debug.Log($"[RegionFlowController] Playing crystal trial for {mode}");
        // Placeholder implementation – replace with actual trial logic.
        yield return new WaitForSeconds(2f);
    }
}


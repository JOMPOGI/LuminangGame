using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple bridge script to allow Prefabs to end minigames.
/// Attach this to a Button in your Minigame Prefab!
/// </summary>
[RequireComponent(typeof(Button))]
public class MinigameEndTrigger : MonoBehaviour
{
    void Start()
    {
        // Automatically hook up the click event
        GetComponent<Button>().onClick.AddListener(EndMinigame);
    }

    public void EndMinigame()
    {
        Debug.Log("[MinigameEndTrigger] Ending minigame/lesson and resuming dialogue...");

        // 1. Hide Lesson if LessonManager is active
        if (LessonManager.Instance != null)
        {
            LessonManager.Instance.HideLesson();
        }

        // 2. Hide Minigame via MinigameManager
        if (MinigameManager.Instance != null)
        {
            MinigameManager.Instance.HideMinigame();
        }

        // 3. Fail-safe: Direct resume check for DialogueManager if any paused minigame choice remains
        if (DialogueManager.Instance != null && DialogueManager.Instance.PendingMinigameChoice != null)
        {
            DialogueManager.Instance.CompleteMinigame();
        }
    }
}

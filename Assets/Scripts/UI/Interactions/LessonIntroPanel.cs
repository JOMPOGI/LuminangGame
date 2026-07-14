using UnityEngine;
using System;

/// <summary>
/// Displays a Lesson Introduction Panel before the vocabulary lesson begins by utilizing
/// the pre-existing GenericModal component. Bypasses scene-hierarchy placement issues.
/// </summary>
public class LessonIntroPanel : MonoBehaviour
{
    public static LessonIntroPanel Instance { get; private set; }

    [HideInInspector] public GameObject panelRoot; // Kept for compatibility checks

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        panelRoot = gameObject; // Bind self as the target reference
    }

    /// <summary>
    /// Shows the Lesson Introduction Panel using GenericModal.
    /// </summary>
    public void ShowForCategory(string categoryName)
    {
        LessonCategoryConfig cfg = LessonCategoryConfig.Find(categoryName);

        string categoryTitle = cfg != null ? cfg.categoryDisplayName : "Language Lesson";
        string lessonTitle = cfg != null ? cfg.lessonDisplayName : categoryName;
        string lessonDesc = cfg != null ? cfg.lessonDescription : "Master these expressions before continuing.";
        int catNum = cfg != null ? cfg.categoryNumber : 1;
        int lesNum = cfg != null ? cfg.lessonNumber : 1;

        // Build the formatted display message matching the requested specifications
        string message = $"<b>{categoryTitle.ToUpper()}</b>\n" +
                         $"Category {catNum} • Lesson {lesNum}: {lessonTitle}\n\n" +
                         $"{lessonDesc}";

        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective($"Start Lesson: {categoryName}");
        }

        if (GenericModal.Instance != null)
        {
            // Use existing GenericModal confirm window
            GenericModal.Instance.ShowConfirm(
                message, 
                "Start Lesson", 
                () => OnStartLessonPressed(categoryName), 
                "Cancel", 
                OnCancelPressed
            );
        }
        else
        {
            // Fallback directly to LessonManager if GenericModal is not in the scene
            Debug.LogWarning("[LessonIntroPanel] GenericModal.Instance not found! Falling back to lesson.");
            StartLessonDirectly(categoryName);
        }
    }

    private void OnStartLessonPressed(string categoryName)
    {
        StartLessonDirectly(categoryName);
    }

    private void OnCancelPressed()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("Talk to Kalaw");
        }
    }

    private void StartLessonDirectly(string categoryName)
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective($"Learn {categoryName}");
        }

        if (LessonManager.Instance != null)
        {
            LessonManager.Instance.ShowLessonWithCategory(categoryName);
        }
    }
}


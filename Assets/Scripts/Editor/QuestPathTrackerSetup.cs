#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public static class QuestPathTrackerSetup
{
    [MenuItem("Luminang/Quest System/Add Genshin Quest Path Tracker to Scene")]
    public static void AddTrackerToScene()
    {
        QuestPathTracker existing = Object.FindFirstObjectByType<QuestPathTracker>();
        if (existing != null)
        {
            Selection.activeGameObject = existing.gameObject;
            Debug.Log("[QuestPathTrackerSetup] QuestPathTracker already exists in this scene!");
            return;
        }

        GameObject trackerObj = new GameObject("GenshinQuestPathTracker");
        QuestPathTracker tracker = trackerObj.AddComponent<QuestPathTracker>();

        // Set up tags / player reference if present
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            tracker.playerTransform = playerObj.transform;
        }

        Undo.RegisterCreatedObjectUndo(trackerObj, "Create Genshin Quest Path Tracker");
        Selection.activeGameObject = trackerObj;

        Debug.Log("Successfully created Genshin-style QuestPathTracker in current scene!");
    }

    [MenuItem("Luminang/Quest System/Attach QuestTargetMarker to Selected GameObject")]
    public static void AttachMarkerToSelected()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("Selection Required", "Please select a GameObject in the scene or hierarchy first.", "OK");
            return;
        }

        QuestTargetMarker marker = selected.GetComponent<QuestTargetMarker>();
        if (marker == null)
        {
            marker = Undo.AddComponent<QuestTargetMarker>(selected);
            marker.requiredObjective = selected.name; // Default to object name
            Debug.Log($"[QuestPathTrackerSetup] Added QuestTargetMarker to {selected.name}. Set 'requiredObjective' in Inspector.");
        }
        else
        {
            Debug.Log($"[QuestPathTrackerSetup] {selected.name} already has a QuestTargetMarker component.");
        }
    }
}
#endif

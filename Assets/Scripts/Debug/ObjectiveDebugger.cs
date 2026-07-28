using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>
/// Custom Editor for ObjectiveDebugger — renders checkboxes as a radio-button group.
/// Only active in the Unity Editor; stripped from builds.
/// </summary>
[CustomEditor(typeof(ObjectiveDebugger))]
public class ObjectiveDebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObjectiveDebugger debugger = (ObjectiveDebugger)target;

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("🧪 Objective Debugger", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "EDITOR ONLY — stripped from builds.\n" +
            "Check any objective below to instantly set it as the active objective during Play Mode.\n" +
            "Unchecking the active one clears the objective.",
            MessageType.Info);

        EditorGUILayout.Space(6);

        // Render objective list
        if (debugger.objectives == null || debugger.objectives.Count == 0)
        {
            EditorGUILayout.HelpBox("Add objective strings to the list below.", MessageType.Warning);
        }

        // Draw the list of objectives as a radio-button group
        for (int i = 0; i < debugger.objectives.Count; i++)
        {
            string obj = debugger.objectives[i];
            bool isActive = debugger.activeIndex == i;

            EditorGUILayout.BeginHorizontal();
            bool newActive = EditorGUILayout.Toggle(isActive, GUILayout.Width(20));
            EditorGUILayout.LabelField(string.IsNullOrEmpty(obj) ? $"[Entry {i}]" : obj);
            EditorGUILayout.EndHorizontal();

            if (newActive && !isActive)
            {
                debugger.activeIndex = i;
                if (Application.isPlaying)
                    debugger.ApplyObjective(i);
            }
            else if (!newActive && isActive)
            {
                debugger.activeIndex = -1;
                if (Application.isPlaying)
                    debugger.ClearObjective();
            }
        }

        EditorGUILayout.Space(8);

        // Edit the objectives list
        EditorGUILayout.LabelField("Objectives List", EditorStyles.boldLabel);
        SerializedObject so = new SerializedObject(target);
        so.Update();
        SerializedProperty objList = so.FindProperty("objectives");
        EditorGUILayout.PropertyField(objList, new GUIContent("Objectives"), true);
        so.ApplyModifiedProperties();

        EditorGUILayout.Space(4);

        // Quick-clear button
        if (GUILayout.Button("Clear Active Objective"))
        {
            debugger.activeIndex = -1;
            if (Application.isPlaying)
                debugger.ClearObjective();
        }

        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}
#endif

/// <summary>
/// Debug utility: lets you instantly switch the active gameplay objective in Play Mode
/// by checking a radio button in the Inspector.
/// 
/// HOW TO USE:
/// 1. Attach this script to any GameObject in your scene (e.g. DebugTools or GameManagers).
/// 2. Populate the 'Objectives' list with all possible objective strings your game uses
///    (e.g. "Talk to Tiptip", "Talk to Ellai", "Maayong buntag", etc).
/// 3. Hit Play Mode in Unity.
/// 4. Select this GameObject in the Hierarchy — you will see the Objective Debugger panel.
/// 5. Check the checkbox next to any objective to instantly activate it.
/// 6. Unchecking the active one clears the objective display.
/// </summary>
public class ObjectiveDebugger : MonoBehaviour
{
    [Tooltip("Full list of objectives to display as checkboxes in the Inspector.")]
    public List<string> objectives = new List<string>
    {
        "Explore the area",
        "Talk to Tiptip when ready",
        "Talk to Tiptip",
        "Talk to Ellai",
        "Maayong buntag",
        "Maayong adlaw"
    };

    [HideInInspector]
    public int activeIndex = -1;

    private void Awake()
    {
        // Ensure this is editor-only — disable in production builds
#if !UNITY_EDITOR
        gameObject.SetActive(false);
        Destroy(this);
#endif
    }

    /// <summary>
    /// Applies the objective at the given index to ObjectiveManager.
    /// Called by the custom Editor when a checkbox is ticked.
    /// </summary>
    public void ApplyObjective(int index)
    {
        if (index < 0 || index >= objectives.Count) return;
        string obj = objectives[index];
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective(obj);
            Debug.Log($"<color=yellow>[ObjectiveDebugger] Active objective set to: \"{obj}\"</color>");
        }
        else
        {
            Debug.LogWarning("[ObjectiveDebugger] ObjectiveManager.Instance is null! Make sure the scene has an ObjectiveManager.");
        }
    }

    /// <summary>
    /// Clears the current objective.
    /// </summary>
    public void ClearObjective()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("");
            Debug.Log("<color=yellow>[ObjectiveDebugger] Objective cleared.</color>");
        }
    }
}

using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

public class QuestFlowEditor : EditorWindow
{
    [SerializeField]
    public List<DialogueNode> questNodes = new List<DialogueNode>();
    
    private ReorderableList _reorderableList;
    private Vector2 _scrollPos;

    [MenuItem("Luminang/Quest Flow Manager")]
    public static void ShowWindow()
    {
        GetWindow<QuestFlowEditor>("Quest Flow Manager");
    }

    private void OnEnable()
    {
        _reorderableList = new ReorderableList(questNodes, typeof(DialogueNode), true, true, true, true);

        _reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Drag and Drop Dialogue Nodes to set story order");
        };

        _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            rect.y += 2;
            questNodes[index] = (DialogueNode)EditorGUI.ObjectField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                questNodes[index],
                typeof(DialogueNode),
                false
            );
        };
    }

    private void OnGUI()
    {
        GUILayout.Label("Quest Flow Manager", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Arrange your Dialogue Nodes in the exact order you want the story to progress. Click 'Apply Quest Flow' to automatically write the 'SetObjective' commands into the End Event Name of each node!", MessageType.Info);

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        _reorderableList.DoLayoutList();
        EditorGUILayout.EndScrollView();

        GUILayout.Space(10);
        if (GUILayout.Button("Apply Quest Flow", GUILayout.Height(40)))
        {
            ApplyQuestFlow();
        }
    }

    private void ApplyQuestFlow()
    {
        if (questNodes == null || questNodes.Count < 2)
        {
            EditorUtility.DisplayDialog("Error", "You need at least 2 Dialogue Nodes in the list to create a flow!", "OK");
            return;
        }

        int updatedCount = 0;
        for (int i = 0; i < questNodes.Count - 1; i++)
        {
            DialogueNode current = questNodes[i];
            DialogueNode next = questNodes[i + 1];

            if (current != null && next != null)
            {
                // Find out the target objective based on the next node's speaker
                string targetSpeaker = next.speakerName;
                if (string.IsNullOrEmpty(targetSpeaker))
                {
                    Debug.LogWarning($"[QuestFlowEditor] Node {next.name} has no Speaker Name set! Skipping link.");
                    continue;
                }

                string newEvent = "SetObjective: Talk to " + targetSpeaker;
                
                Undo.RecordObject(current, "Update Quest Flow");
                current.endEventName = newEvent;
                EditorUtility.SetDirty(current);
                updatedCount++;
            }
        }
        
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Success!", $"Successfully linked {updatedCount} dialogues together!\n\nYour quest indicator will now automatically jump to the next NPC in your list when each conversation ends.", "Awesome!");
    }
}

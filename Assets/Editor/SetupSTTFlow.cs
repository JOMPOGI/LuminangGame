using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

public class SetupSTTFlow : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Luminang/Setup STT Flow (Calle Crisologo)")]
    public static void SetupFlow()
    {
        AssetDatabase.Refresh();
        string dir = "Assets/Dialogues/GDD_Flow/";

        // 1. Exact GameObject name mapping to the _Start nodes generated linearly
        var npcMap = new Dictionary<string, string>()
        {
            {"Kalaw", "Kalaw_Start"},
            {"Ronnie_Rigged", "Ronnie_Start"},
            {"Sally_Rigged", "Sally_Start"},
            {"Lito_Rigged", "Lito_Start"},
            {"NPC_Apo_Lakay", "ApoLakay_Start"},
            {"Tomas_Rigged", "Tomas_Start"},
            {"Klara_Rigged", "Klara_Start"},
            {"Tala_Rigged", "Tala_Start"},
            {"MangLance_Rigged", "MangLance_Start"},
            {"Rayo_Rigged", "Rayo_Start"},
            {"AlingRosa_Rigged", "AlingRosa_Start"},
            {"LolaNida_Rigged", "LolaNida_Start"},
            {"Neneng_Rigged", "Neneng_Start"},
            {"AlingRiza_Rigged", "AlingRiza_Start"},
            {"Pedro_Rigged", "Pedro_Start"},
            {"LolaBebang_Rigged", "LolaBebang_Start"}
        };

        // Stages beyond "Start" mapping: { GameObject, List of (Objective String, Asset Name) }
        var questStages = new Dictionary<string, List<(string, string)>>()
        {
            {"Kalaw", new List<(string, string)> { ("Return to Kalaw for Final Test", "Kalaw_Final_Start") }},
            {"Ronnie_Rigged", new List<(string, string)> { ("Talk to Ronnie again", "Ronnie_Identity_Start") }},
            {"Sally_Rigged", new List<(string, string)> { ("Talk to Sally for Requests", "Sally_Requests_Start") }},
            {"Lito_Rigged", new List<(string, string)> { ("Talk to Lito for Directions", "Lito_Directions_Start") }},
            {"Klara_Rigged", new List<(string, string)> { ("Talk to Klara for Counting", "Klara_Counting_Start") }},
            {"MangLance_Rigged", new List<(string, string)> { ("Talk to Mang Lance for Verbs", "MangLance_Verbs_Start") }},
            {"AlingRosa_Rigged", new List<(string, string)> { ("Talk to Aling Rosa for Linking Verbs", "AlingRosa_Linking_Start") }},
            {"AlingRiza_Rigged", new List<(string, string)> { ("Talk to Aling Riza for Pronouns", "AlingRiza_Pronouns_Start") }},
            {"LolaBebang_Rigged", new List<(string, string)> { ("Talk to Lola Bebang for Interrogatives", "LolaBebang_Interrogatives_Start") }}
        };

        foreach (var kvp in npcMap)
        {
            GameObject go = GameObject.Find(kvp.Key);
            if (go == null)
            {
                Debug.LogWarning($"[SetupSTTFlow] Could not find GameObject: {kvp.Key}");
                continue;
            }

            InteractableNPC npc = go.GetComponent<InteractableNPC>();
            if (npc == null) continue;

            npc.interactionEnabled = (go.name == "Kalaw");
            npc.disableAfterInteraction = false; 
            npc.dialogueEvents.Clear();
            npc.questDialogues.Clear(); // Clear existing quest dialogues

            // Set Default Dialogue (Start)
            string assetPath = dir + kvp.Value + ".asset";
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(assetPath);
            if (node == null) Debug.LogError($"[SetupSTTFlow] Missing Dialogue Asset: {assetPath}");
            npc.defaultDialogue = node;

            // Assign Multi-stage Quest Dialogues
            if (questStages.ContainsKey(kvp.Key))
            {
                foreach (var stage in questStages[kvp.Key])
                {
                    string stagePath = dir + stage.Item2 + ".asset";
                    DialogueNode stageNode = AssetDatabase.LoadAssetAtPath<DialogueNode>(stagePath);
                    if (stageNode != null)
                    {
                        var qd = new InteractableNPC.QuestDialogue();
                        qd.requiredObjective = stage.Item1;
                        qd.dialogueNode = stageNode;
                        npc.questDialogues.Add(qd);
                    }
                    else
                    {
                        Debug.LogError($"[SetupSTTFlow] Missing Quest Dialogue Asset: {stagePath}");
                    }
                }
            }

            EditorUtility.SetDirty(npc);
        }

        AssetDatabase.SaveAssets();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("GDD RPG Linear Flow Successfully Setup with Quest Stages!");
    }
}
#endif

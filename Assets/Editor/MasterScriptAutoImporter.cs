using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class MasterScriptAutoImporter : EditorWindow
{
    [System.Serializable]
    public class DialogueData
    {
        public int word_num;
        public string speaker;
        public string teach_text;
        public string stt_expected;
        public string success_text;
    }

    [System.Serializable]
    public class DialogueDataList
    {
        public List<DialogueData> data;
    }

    [MenuItem("Luminang/Massive Script Auto Importer")]
    public static void RunImport()
    {
        string jsonPath = "Assets/Dialogues/master_dialogues.json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("JSON not found at " + jsonPath);
            return;
        }

        string json = File.ReadAllText(jsonPath);
        // Quick hack for JSON array reading in Unity
        json = "{\"data\":" + json + "}";
        DialogueDataList parsed = JsonUtility.FromJson<DialogueDataList>(json);

        if (parsed == null || parsed.data == null)
        {
            Debug.LogError("Failed to parse JSON");
            return;
        }

        string outFolder = "Assets/Dialogues/MassiveImport";
        if (!AssetDatabase.IsValidFolder(outFolder))
        {
            AssetDatabase.CreateFolder("Assets/Dialogues", "MassiveImport");
        }

        List<DialogueNode> createdNodes = new List<DialogueNode>();

        for (int i = 0; i < parsed.data.Count; i++)
        {
            var d = parsed.data[i];
            
            // Clean speaker name (remove newlines from regex)
            string cleanSpeaker = d.speaker.Trim();
            if (cleanSpeaker.Contains("\n"))
            {
                string[] parts = cleanSpeaker.Split('\n');
                cleanSpeaker = parts[parts.Length - 1].Trim();
            }

            // Create Teach Node
            string teachName = $"{d.word_num:00}_{cleanSpeaker}_Teach";
            DialogueNode teachNode = CreateNode(outFolder, teachName, cleanSpeaker, d.teach_text);

            // Create Success Node
            string successName = $"{d.word_num:00}_{cleanSpeaker}_Success";
            DialogueNode successNode = CreateNode(outFolder, successName, cleanSpeaker, d.success_text);

            // Wire Teach -> Success via STT
            teachNode.choices.Clear();
            teachNode.choices.Add(new DialogueChoice {
                choiceText = "Speak",
                choiceEvent = "StartSTT",
                expectedSTTWord = d.stt_expected.Trim(),
                nextNode = successNode
            });
            EditorUtility.SetDirty(teachNode);

            // Wire Success -> Next
            if (i < parsed.data.Count - 1)
            {
                var nextD = parsed.data[i + 1];
                string nextSpeaker = nextD.speaker.Trim();
                if (nextSpeaker.Contains("\n"))
                {
                    string[] parts = nextSpeaker.Split('\n');
                    nextSpeaker = parts[parts.Length - 1].Trim();
                }

                if (cleanSpeaker == nextSpeaker)
                {
                    // Same NPC -> Link via Next Choice
                    string nextTeachName = $"{nextD.word_num:00}_{nextSpeaker}_Teach";
                    DialogueNode nextTeachNode = CreateNode(outFolder, nextTeachName, nextSpeaker, nextD.teach_text);
                    
                    successNode.choices.Clear();
                    successNode.choices.Add(new DialogueChoice {
                        choiceText = "Next",
                        nextNode = nextTeachNode
                    });
                }
                else
                {
                    // Different NPC -> Set Objective
                    successNode.endEventName = "SetObjective: Talk to " + nextSpeaker;
                }
            }
            else
            {
                successNode.endEventName = "SetObjective: Return to Kalaw for Final Proficiency Test";
            }
            EditorUtility.SetDirty(successNode);

            createdNodes.Add(teachNode);
        }

        AssetDatabase.SaveAssets();

        // Scene Assignment
        GameObject[] sceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int npcsUpdated = 0;

        // Group by speaker
        Dictionary<string, DialogueNode> startingNodes = new Dictionary<string, DialogueNode>();
        foreach (var d in parsed.data)
        {
            string cleanSpeaker = d.speaker.Trim();
            if (cleanSpeaker.Contains("\n"))
            {
                string[] parts = cleanSpeaker.Split('\n');
                cleanSpeaker = parts[parts.Length - 1].Trim();
            }
            if (!startingNodes.ContainsKey(cleanSpeaker))
            {
                string firstTeachName = $"{d.word_num:00}_{cleanSpeaker}_Teach";
                startingNodes[cleanSpeaker] = AssetDatabase.LoadAssetAtPath<DialogueNode>($"{outFolder}/{firstTeachName}.asset");
            }
        }

        foreach (GameObject go in sceneObjects)
        {
            InteractableNPC npc = go.GetComponent<InteractableNPC>();
            if (npc != null)
            {
                string nName = go.name.ToLower();
                foreach (var kvp in startingNodes)
                {
                    string speakerLow = kvp.Key.ToLower();
                    if (speakerLow == "kyros" && nName.Contains("kyros")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "irah" && nName.Contains("irah")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "jom" && nName.Contains("jom")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "lito" && nName.Contains("lito")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "apo lakay" && nName.Contains("apo lakay")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "tomas" && nName.Contains("tomas")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "klara" && nName.Contains("klara")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "tala" && nName.Contains("tala")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "mang lance" && nName.Contains("mang lance")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "rayo" && nName.Contains("rayo")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "aling rosa" && nName.Contains("aling rosa")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "lola nida" && nName.Contains("lola nida")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "neneng" && nName.Contains("neneng")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "aling riza" && nName.Contains("aling riza")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "lola bebang" && nName.Contains("lola bebang")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "sally" && nName.Contains("sally")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "ronnie" && nName.Contains("ronnie")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                    else if (speakerLow == "kalaw" && nName.Contains("kalaw")) Assign(npc, kvp.Key, kvp.Value, ref npcsUpdated);
                }
            }
        }

        EditorUtility.DisplayDialog("Massive Integration Complete!", 
            $"Successfully created {parsed.data.Count * 2} dialogue nodes!\n\nWired them all up with STT targets, linked the conversation chains, and automatically updated {npcsUpdated} characters in your scene!", "Epic!");
    }

    private static void Assign(InteractableNPC npc, string speaker, DialogueNode node, ref int count)
    {
        Undo.RecordObject(npc, "Massive Script Setup");
        // Clear out old ones
        npc.questDialogues.Clear();
        npc.questDialogues.Add(new InteractableNPC.QuestDialogue { 
            requiredObjective = "Talk to " + speaker, 
            dialogueNode = node 
        });
        
        // Ensure the new node is also the default dialogue so the old one never plays!
        npc.defaultDialogue = node;
        
        // Ensure Kalaw triggers on intro objective if he's the first
        if (speaker.ToLower().Contains("kalaw"))
        {
            npc.questDialogues.Add(new InteractableNPC.QuestDialogue { 
                requiredObjective = "Find Fruit", 
                dialogueNode = node 
            });
        }
        
        EditorUtility.SetDirty(npc);
        count++;
    }

    private static DialogueNode CreateNode(string folderPath, string fileName, string speakerName, string text)
    {
        string fullPath = $"{folderPath}/{fileName}.asset";
        DialogueNode existing = AssetDatabase.LoadAssetAtPath<DialogueNode>(fullPath);
        if (existing == null)
        {
            DialogueNode newNode = ScriptableObject.CreateInstance<DialogueNode>();
            newNode.speakerName = speakerName;
            newNode.dialogueText = text;
            AssetDatabase.CreateAsset(newNode, fullPath);
            return newNode;
        }
        else
        {
            existing.speakerName = speakerName;
            existing.dialogueText = text;
            return existing;
        }
    }
}

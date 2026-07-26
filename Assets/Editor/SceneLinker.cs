using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class SceneLinker : EditorWindow
{
    [MenuItem("Luminang/Emergency Scene Linker")]
    public static void RunImport()
    {
        string outFolder = "Assets/Dialogues/MassiveImport";
        
        GameObject[] sceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int npcsUpdated = 0;

        string[] guids = AssetDatabase.FindAssets("*_Teach t:DialogueNode", new[] { outFolder });
        
        // Find the FIRST teach node for each speaker
        Dictionary<string, DialogueNode> startingNodes = new Dictionary<string, DialogueNode>();
        Dictionary<string, int> lowestWordNum = new Dictionary<string, int>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            
            // Format: "13_Jom_Teach"
            string[] parts = fileName.Split('_');
            if (parts.Length >= 3)
            {
                if (int.TryParse(parts[0], out int wordNum))
                {
                    string speaker = parts[1];
                    if (!lowestWordNum.ContainsKey(speaker) || wordNum < lowestWordNum[speaker])
                    {
                        lowestWordNum[speaker] = wordNum;
                        startingNodes[speaker] = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
                    }
                }
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

        EditorSceneManager.SaveOpenScenes();
        Debug.Log($"Emergency Linker Complete! Wired {npcsUpdated} characters in your scene using existing ScriptableObjects.");
    }

    private static void Assign(InteractableNPC npc, string speaker, DialogueNode node, ref int count)
    {
        Undo.RecordObject(npc, "Emergency Script Setup");
        npc.questDialogues.Clear();
        npc.questDialogues.Add(new InteractableNPC.QuestDialogue { 
            requiredObjective = "Talk to " + speaker, 
            dialogueNode = node 
        });
        
        npc.defaultDialogue = node;
        
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
}

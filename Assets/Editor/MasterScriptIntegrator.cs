using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MasterScriptIntegrator : EditorWindow
{
    [MenuItem("Luminang/Integrate Master Script Into Scene")]
    public static void RunIntegration()
    {
        string folderPath = "Assets/Dialogues/UpdatedMasterScript";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            EditorUtility.DisplayDialog("Error", "Could not find the Generated Dialogues folder! Did you run the Generator script first?", "OK");
            return;
        }

        // 1. Load All Nodes
        var nodes = new Dictionary<string, DialogueNode>();
        string[] guids = AssetDatabase.FindAssets("t:DialogueNode", new[] { folderPath });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
            if (node != null)
            {
                nodes[node.name] = node;
            }
        }

        if (nodes.Count == 0) return;

        // 2. Wire Multi-Part Conversations
        LinkNext(nodes, "02_Kalaw_PostQuest1", "02_Kalaw_PostQuest2");
        LinkNext(nodes, "02_Kalaw_PostQuest2", "02_Kalaw_PostQuest3");
        LinkNext(nodes, "02_Kalaw_PostQuest3", "02_Kalaw_PostQuest4");
        LinkNext(nodes, "02_Kalaw_PostQuest4", "02_Kalaw_PostQuest5");
        SetEndObjective(nodes, "02_Kalaw_PostQuest5", "SetObjective: Learn Word 1");

        // Word 1: kumusta
        LinkNext(nodes, "03_Kalaw_Word1_Teach1", "03_Kalaw_Word1_Teach2");
        LinkSTT(nodes, "03_Kalaw_Word1_Teach2", "kumusta", "04_Kalaw_Word1_Success");
        SetEndObjective(nodes, "04_Kalaw_Word1_Success", "SetObjective: Learn Word 2");

        // Word 2: kumusta ka
        LinkNext(nodes, "05_Kalaw_Word2_Teach1", "05_Kalaw_Word2_Teach2");
        LinkSTT(nodes, "05_Kalaw_Word2_Teach2", "kumusta ka", "06_Kalaw_Word2_Success");
        SetEndObjective(nodes, "06_Kalaw_Word2_Success", "SetObjective: Learn Word 3");

        // Word 3: nasayaat ak
        LinkNext(nodes, "07_Kalaw_Word3_Teach1", "07_Kalaw_Word3_Teach2");
        LinkSTT(nodes, "07_Kalaw_Word3_Teach2", "nasayaat ak", "08_Kalaw_Word3_Success");
        SetEndObjective(nodes, "08_Kalaw_Word3_Success", "SetObjective: Learn Word 4");

        // Word 4: naimbag a bigat
        LinkNext(nodes, "09_Kalaw_Word4_Teach1", "09_Kalaw_Word4_Teach2");
        LinkSTT(nodes, "09_Kalaw_Word4_Teach2", "naimbag a bigat", "10_Kalaw_Word4_Success");
        SetEndObjective(nodes, "10_Kalaw_Word4_Success", "SetObjective: Talk to Kyros");

        // Word 5: naimbag a malem
        LinkNext(nodes, "11_Kyros_Word5_Teach1", "11_Kyros_Word5_Teach2");
        LinkSTT(nodes, "11_Kyros_Word5_Teach2", "naimbag a malem", "12_Kyros_Word5_Success");
        SetEndObjective(nodes, "12_Kyros_Word5_Success", "SetObjective: Learn Word 6");

        // Word 6: naimbag a rabii
        LinkSTT(nodes, "13_Kyros_Word6_Teach", "naimbag a rabii", "14_Kyros_Word6_Success");
        SetEndObjective(nodes, "14_Kyros_Word6_Success", "SetObjective: Learn Word 7");

        // Word 7: naimbag nga aldaw
        LinkSTT(nodes, "15_Kyros_Word7_Teach", "naimbag nga aldaw", "16_Kyros_Word7_Success");
        SetEndObjective(nodes, "16_Kyros_Word7_Success", "SetObjective: Learn Word 8");

        // Word 8: agpakada akon
        LinkSTT(nodes, "17_Kyros_Word8_Teach", "agpakada akon", "18_Kyros_Word8_Success");
        SetEndObjective(nodes, "18_Kyros_Word8_Success", "SetObjective: Talk to Irah");

        // Final Test
        LinkNext(nodes, "19_Kalaw_FinalTest_Intro1", "19_Kalaw_FinalTest_Intro2");
        LinkNext(nodes, "19_Kalaw_FinalTest_Intro2", "19_Kalaw_FinalTest_Intro3");
        LinkNext(nodes, "19_Kalaw_FinalTest_Intro3", "19_Kalaw_FinalTest_Intro4");
        LinkNext(nodes, "19_Kalaw_FinalTest_Intro4", "19_Kalaw_FinalTest_Intro5");
        // Test system handles scores
        
        LinkNext(nodes, "24_Kalaw_Final_Dialogue1", "24_Kalaw_Final_Dialogue2");
        LinkNext(nodes, "24_Kalaw_Final_Dialogue2", "24_Kalaw_Final_Dialogue3");
        LinkNext(nodes, "24_Kalaw_Final_Dialogue3", "24_Kalaw_Final_Dialogue4");
        LinkNext(nodes, "24_Kalaw_Final_Dialogue4", "24_Kalaw_Final_Dialogue5");
        LinkNext(nodes, "24_Kalaw_Final_Dialogue5", "24_Kalaw_Final_Dialogue6");

        AssetDatabase.SaveAssets();

        // 3. Setup Scene Characters
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int npcsUpdated = 0;

        foreach (GameObject go in allObjects)
        {
            string cleanName = go.name.ToLower();
            if (cleanName.Contains("kalaw"))
            {
                InteractableNPC npc = go.GetComponent<InteractableNPC>();
                if (npc != null)
                {
                    Undo.RecordObject(npc, "Integrate Kalaw");
                    npc.questDialogues.Clear();

                    AddQuestDialogue(npc, "Talk to Kalaw", GetNode(nodes, "01_Kalaw_Initial"));
                    AddQuestDialogue(npc, "Give Fruit", GetNode(nodes, "02_Kalaw_PostQuest1"));
                    AddQuestDialogue(npc, "Learn Word 1", GetNode(nodes, "03_Kalaw_Word1_Teach1"));
                    AddQuestDialogue(npc, "Learn Word 2", GetNode(nodes, "05_Kalaw_Word2_Teach1"));
                    AddQuestDialogue(npc, "Learn Word 3", GetNode(nodes, "07_Kalaw_Word3_Teach1"));
                    AddQuestDialogue(npc, "Learn Word 4", GetNode(nodes, "09_Kalaw_Word4_Teach1"));
                    AddQuestDialogue(npc, "Return to Kalaw for Final Proficiency Test", GetNode(nodes, "19_Kalaw_FinalTest_Intro1"));

                    EditorUtility.SetDirty(npc);
                    npcsUpdated++;
                }
            }
            else if (cleanName.Contains("kyros"))
            {
                InteractableNPC npc = go.GetComponent<InteractableNPC>();
                if (npc != null)
                {
                    Undo.RecordObject(npc, "Integrate Kyros");
                    npc.questDialogues.Clear();

                    AddQuestDialogue(npc, "Talk to Kyros", GetNode(nodes, "11_Kyros_Word5_Teach1"));
                    AddQuestDialogue(npc, "Learn Word 6", GetNode(nodes, "13_Kyros_Word6_Teach"));
                    AddQuestDialogue(npc, "Learn Word 7", GetNode(nodes, "15_Kyros_Word7_Teach"));
                    AddQuestDialogue(npc, "Learn Word 8", GetNode(nodes, "17_Kyros_Word8_Teach"));

                    EditorUtility.SetDirty(npc);
                    npcsUpdated++;
                }
            }
        }

        EditorUtility.DisplayDialog("Integration Complete!", 
            $"Successfully wired all STT connections and dialogue chains!\n\nUpdated {npcsUpdated} NPCs in the scene with the new Master Script quests.", "Awesome!");
    }

    private static DialogueNode GetNode(Dictionary<string, DialogueNode> dict, string name)
    {
        return dict.ContainsKey(name) ? dict[name] : null;
    }

    private static void LinkNext(Dictionary<string, DialogueNode> dict, string current, string next)
    {
        if (dict.TryGetValue(current, out DialogueNode c) && dict.TryGetValue(next, out DialogueNode n))
        {
            c.choices.Clear();
            c.choices.Add(new DialogueChoice { choiceText = "Next", nextNode = n });
            EditorUtility.SetDirty(c);
        }
    }

    private static void LinkSTT(Dictionary<string, DialogueNode> dict, string current, string expectedWord, string next)
    {
        if (dict.TryGetValue(current, out DialogueNode c) && dict.TryGetValue(next, out DialogueNode n))
        {
            c.choices.Clear();
            c.choices.Add(new DialogueChoice { 
                choiceText = "Speak", 
                choiceEvent = "StartSTT", 
                expectedSTTWord = expectedWord,
                nextNode = n 
            });
            EditorUtility.SetDirty(c);
        }
    }

    private static void SetEndObjective(Dictionary<string, DialogueNode> dict, string nodeName, string objectiveStr)
    {
        if (dict.TryGetValue(nodeName, out DialogueNode n))
        {
            n.endEventName = objectiveStr;
            EditorUtility.SetDirty(n);
        }
    }

    private static void AddQuestDialogue(InteractableNPC npc, string objective, DialogueNode node)
    {
        if (node != null)
        {
            npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = objective, dialogueNode = node });
        }
    }
}

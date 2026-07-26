using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class UltimateScriptImporter : EditorWindow
{
    [MenuItem("Luminang/Implement Final Master Script (Evaluation)")]
    public static void RunImport()
    {
        string outFolder = "Assets/Dialogues/MassiveImport";

        // Generate Final Evaluation Nodes
        
        // Phase 1: Comprehension
        DialogueNode p1 = CreateNode(outFolder, "84_Kalaw_Eval_Phase1", "Kalaw", "Now for your final evaluation! Listen carefully. You are walking and someone tells you: 'Agdiretso.' What should you do?");
        p1.triggerEventName = "FinalEval_Phase1: Agdiretso"; // Let the manager know it started
        p1.choices.Clear();
        p1.choices.Add(new DialogueChoice { choiceText = "Turn Left", isWrong = true });
        p1.choices.Add(new DialogueChoice { choiceText = "Turn Right", isWrong = true });
        p1.choices.Add(new DialogueChoice { choiceText = "Stop", isWrong = true });
        p1.choices.Add(new DialogueChoice { choiceText = "Go Straight", choiceEvent = "RecordPhase1Score" }); 

        // Phase 2: Vocabulary
        DialogueNode p2 = CreateNode(outFolder, "85_Kalaw_Eval_Phase2", "Kalaw", "Correct! Now, imagine you meet a local vendor early in the morning. How do you say 'Good morning'?");
        p2.triggerEventName = "FinalEval_Phase2: naimbag a bigat";
        p2.choices.Clear();
        p2.choices.Add(new DialogueChoice { choiceText = "Speak", choiceEvent = "StartSTT" });

        // Phase 3: Guided Sentence Production
        DialogueNode p3 = CreateNode(outFolder, "86_Kalaw_Eval_Phase3", "Kalaw", "Imagine you've just met someone new. Tell them your name by saying 'My name is...' followed by your real name.");
        p3.triggerEventName = "FinalEval_Phase3: ti nagan ko ket"; // Looking for structural template
        p3.choices.Clear();
        p3.choices.Add(new DialogueChoice { choiceText = "Speak", choiceEvent = "StartSTT" });

        // Phase 4: Functional Communication
        DialogueNode p4 = CreateNode(outFolder, "87_Kalaw_Eval_Phase4", "Kalaw", "You are looking for your hotel but you are lost. How do you politely say 'Can you help me?'");
        p4.triggerEventName = "FinalEval_Phase4: mabalin kadi a tulunganak";
        p4.choices.Clear();
        p4.choices.Add(new DialogueChoice { choiceText = "Speak", choiceEvent = "StartSTT" });

        // Phase 5: Grammar
        DialogueNode p5 = CreateNode(outFolder, "88_Kalaw_Eval_Phase5", "Kalaw", "You see a beautiful hand-woven fabric. Ask the vendor, 'How much is this?'");
        p5.triggerEventName = "FinalEval_Phase5: sagmamano daytoy";
        p5.choices.Clear();
        p5.choices.Add(new DialogueChoice { choiceText = "Speak", choiceEvent = "StartSTT" });

        // Phase 6: Free Response
        DialogueNode p6 = CreateNode(outFolder, "89_Kalaw_Eval_Phase6", "Kalaw", "Now, introduce yourself briefly as if you just arrived in Vigan. Tell them your name and where you are from.");
        p6.triggerEventName = "FinalEval_Phase6: kumusta ti nagan ko ket taga";
        p6.choices.Clear();
        p6.choices.Add(new DialogueChoice { choiceText = "Speak", choiceEvent = "StartSTT" });
        p6.endEventName = "FinalEval_Complete";

        // Result Nodes
        DialogueNode resAdvanced = CreateNode(outFolder, "90_Result_Advanced", "Kalaw", "Excellent! You are an Advanced Speaker! The Crystal of Language glows brightly!");
        DialogueNode resProficient = CreateNode(outFolder, "90_Result_Proficient", "Kalaw", "Great job! You are a Proficient Traveler. You can communicate effectively here!");
        DialogueNode resDeveloping = CreateNode(outFolder, "90_Result_Developing", "Kalaw", "Good effort. You are a Developing Speaker. Keep practicing with the locals!");
        DialogueNode resBeginning = CreateNode(outFolder, "90_Result_Beginning", "Kalaw", "You are a Beginning Speaker. Don't worry, every journey starts with a single word. Keep practicing!");

        // Wire them up
        p1.choices[3].nextNode = p2;
        p2.choices[0].nextNode = p3;
        p3.choices[0].nextNode = p4;
        p4.choices[0].nextNode = p5;
        p5.choices[0].nextNode = p6;
        p6.choices[0].nextNode = null; // Manager will inject the result node

        // Save
        EditorUtility.SetDirty(p1);
        EditorUtility.SetDirty(p2);
        EditorUtility.SetDirty(p3);
        EditorUtility.SetDirty(p4);
        EditorUtility.SetDirty(p5);
        EditorUtility.SetDirty(p6);
        AssetDatabase.SaveAssets();

        // Assign to FinalEvaluationManager in the Scene
        GameObject evalObj = GameObject.Find("FinalEvaluationManager");
        if (evalObj == null)
        {
            evalObj = new GameObject("FinalEvaluationManager");
            evalObj.AddComponent<FinalEvaluationManager>();
        }
        var manager = evalObj.GetComponent<FinalEvaluationManager>();
        manager.advancedSpeakerNode = resAdvanced;
        manager.proficientTravelerNode = resProficient;
        manager.developingSpeakerNode = resDeveloping;
        manager.beginningSpeakerNode = resBeginning;

        // Find Kalaw and add quest objective
        GameObject kalawGo = GameObject.Find("RiggedKalaw") ?? GameObject.Find("Kalaw");
        if (kalawGo != null)
        {
            var npc = kalawGo.GetComponent<InteractableNPC>();
            if (npc != null)
            {
                // Remove old "Return to Kalaw for Final Proficiency Test" if exists
                npc.questDialogues.RemoveAll(q => q.requiredObjective == "Return to Kalaw for Final Proficiency Test");
                
                npc.questDialogues.Add(new InteractableNPC.QuestDialogue {
                    requiredObjective = "Return to Kalaw for Final Proficiency Test",
                    dialogueNode = p1
                });
                EditorUtility.SetDirty(npc);
            }
        }

        Debug.Log("Final Evaluation Integration Complete!");
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

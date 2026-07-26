using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class NarrativeNodeGenerator2
{
    [MenuItem("Luminang/Fix Kalaw Intro")]
    public static void Run()
    {
        string outFolder = "Assets/Dialogues/MassiveImport";
        
        // Load Kalaw Intro
        DialogueNode kalawIntro = AssetDatabase.LoadAssetAtPath<DialogueNode>($"{outFolder}/00_Kalaw_Intro.asset");
        DialogueNode kalawPost = AssetDatabase.LoadAssetAtPath<DialogueNode>($"{outFolder}/00_Kalaw_PostQuest.asset");

        if (kalawIntro != null && kalawPost != null)
        {
            // Remove the "SetObjective: Find Fruit" end event
            kalawIntro.endEventName = "";

            // Link it directly to Post-Quest via a Choice!
            kalawIntro.choices.Clear();
            kalawIntro.choices.Add(new DialogueChoice { 
                choiceText = "Hand over Fruit", 
                nextNode = kalawPost 
            });

            EditorUtility.SetDirty(kalawIntro);
            AssetDatabase.SaveAssets();
            Debug.Log("Fixed Kalaw's Intro to flow directly into the Post-Quest without setting an objective!");
        }

        // Update Scene Objects just in case
        GameObject[] sceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GameObject go in sceneObjects)
        {
            InteractableNPC npc = go.GetComponent<InteractableNPC>();
            if (npc != null && go.name.ToLower().Contains("kalaw"))
            {
                Undo.RecordObject(npc, "Fix Kalaw Dialogue");
                
                // Clear all quest dialogues
                npc.questDialogues.Clear();

                // Make Kalaw's default dialogue the intro
                npc.defaultDialogue = kalawIntro;

                // Add the final proficiency test objective
                DialogueNode finalOutro = AssetDatabase.LoadAssetAtPath<DialogueNode>($"{outFolder}/82_Kalaw_FinalEvaluation.asset");
                npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Return to Kalaw for Final Proficiency Test", dialogueNode = finalOutro });
                
                EditorUtility.SetDirty(npc);
            }
        }
        
        EditorSceneManager.SaveOpenScenes();
    }
}

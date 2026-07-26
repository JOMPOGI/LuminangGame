using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class NarrativeNodeGenerator
{
    [MenuItem("Luminang/Generate Narrative Nodes")]
    public static void Run()
    {
        string outFolder = "Assets/Dialogues/MassiveImport";
        
        // 1. Kalaw Intro
        DialogueNode kalawIntro = CreateNode(outFolder, "00_Kalaw_Intro", "Kalaw", 
            "Squawk! Oh... is that a fruit in your hand? May I have some? Flying under this hot Vigan sun has completely worn me out...");
        kalawIntro.choices.Clear();
        kalawIntro.endEventName = "SetObjective: Find Fruit";
        EditorUtility.SetDirty(kalawIntro);

        // 2. Kalaw Post-Quest
        DialogueNode kalawPost = CreateNode(outFolder, "00_Kalaw_PostQuest", "Kalaw", 
            "Mmm, sweet and juicy! Thank you so much, traveler!\n\nAh, where are my manners—I am Kalaw, your companion and guide through these lands!\n\nWait a second... look at that ancient anting-anting pendant resting on your chest! Feel that faint hum?\n\nThe Ilocos Language Crystal inside it is sleeping.\n\nTo charge it up and awaken its power, you've got to explore Calle Crisologo and speak with the locals in their native tongue.\n\nAs you practice and learn how Ilocanos talk in their daily lives, your voice will help restore the crystal's power.\n\nSo, let's begin with something simple.\n\nBefore you can truly connect with the people of Ilocos, you must first learn how to greet them.");
        
        DialogueNode word1 = AssetDatabase.LoadAssetAtPath<DialogueNode>($"{outFolder}/01_Kalaw_Teach.asset");
        kalawPost.choices.Clear();
        kalawPost.choices.Add(new DialogueChoice { choiceText = "Next", nextNode = word1 });
        EditorUtility.SetDirty(kalawPost);

        // 3. Mang Lance Intro
        DialogueNode mangLanceIntro = CreateNode(outFolder, "44b_MangLance_Intro", "Mang Lance", 
            "Whoa, hold on! My carriage wheel pin popped out!\n\nPlease find the wheel pin so my horse Barnaby and I can ride safely!");
        mangLanceIntro.choices.Clear();
        mangLanceIntro.endEventName = "SetObjective: Find Wheel Pin";
        EditorUtility.SetDirty(mangLanceIntro);

        // 4. Aling Rosa Intro
        DialogueNode alingRosaIntro = CreateNode(outFolder, "52b_AlingRosa_Intro", "Aling Rosa", 
            "Ay, my thread broke!\n\nPlease find some thread so I can finish weaving these colorful souvenirs!");
        alingRosaIntro.choices.Clear();
        alingRosaIntro.endEventName = "SetObjective: Find Thread";
        EditorUtility.SetDirty(alingRosaIntro);

        // 5. Final Kalaw Outro
        DialogueNode finalOutro = CreateNode(outFolder, "82_Kalaw_FinalEvaluation", "Kalaw", 
            "SQUAWK!\n\nYOUR VOICE HAS BEEN HEARD!\n\nLook at how far you've come, traveler.\n\nYou didn't just memorize words.\n\nYou learned to understand. You learned to respond. You learned to build your own sentences.\n\nYou learned to ask for help, navigate the streets, introduce yourself, and communicate with the people of Ilocos.\n\nMore importantly, you discovered that language lives through the people who speak it.\n\nEvery word you learned came from someone. Every phrase carried a meaning. Every conversation connected you to the people and culture of this place.\n\nThe language you practiced here is not simply something to collect.\n\nIt is something living.\n\nSomething carried by families, communities, traditions, and stories.\n\nYour journey through Vigan may be complete, but your journey with Ilokano does not have to end here.\n\nThe more you listen, the more you speak. The more you speak, the more you understand.\n\nAnd the more you understand, the closer you become to the people and culture that keep this language alive.\n\nGreat job, traveler.\n\nYou've taken another step in restoring the voices of the regions.");
        finalOutro.choices.Clear();
        finalOutro.endEventName = "SetObjective: Ilocos Chapter Complete";
        EditorUtility.SetDirty(finalOutro);

        AssetDatabase.SaveAssets();

        // Update Scene Objects
        GameObject[] sceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GameObject go in sceneObjects)
        {
            InteractableNPC npc = go.GetComponent<InteractableNPC>();
            if (npc != null)
            {
                string nName = go.name.ToLower();
                Undo.RecordObject(npc, "Narrative Nodes Update");

                if (nName.Contains("kalaw"))
                {
                    // Kalaw's default becomes Intro
                    npc.defaultDialogue = kalawIntro;
                    // Kalaw triggers PostQuest if objective is "Find Fruit Completed" or similar
                    // But in the user's quest system, what is the exact string when Fruit is given?
                    // Let's add a couple variants.
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Find Fruit", dialogueNode = kalawIntro });
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Return to Kalaw", dialogueNode = kalawPost });
                    
                    // Final assessment objective
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Return to Kalaw for Final Proficiency Test", dialogueNode = finalOutro });
                }
                else if (nName.Contains("mang lance"))
                {
                    npc.defaultDialogue = mangLanceIntro;
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Talk to Mang Lance", dialogueNode = mangLanceIntro });
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Find Wheel Pin", dialogueNode = mangLanceIntro });
                    
                    DialogueNode word45 = AssetDatabase.LoadAssetAtPath<DialogueNode>($"{outFolder}/45_Mang Lance_Teach.asset");
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Return to Mang Lance", dialogueNode = word45 });
                }
                else if (nName.Contains("aling rosa"))
                {
                    npc.defaultDialogue = alingRosaIntro;
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Talk to Aling Rosa", dialogueNode = alingRosaIntro });
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Find Thread", dialogueNode = alingRosaIntro });
                    
                    DialogueNode word53 = AssetDatabase.LoadAssetAtPath<DialogueNode>($"{outFolder}/53_Aling Rosa_Teach.asset");
                    npc.questDialogues.Add(new InteractableNPC.QuestDialogue { requiredObjective = "Return to Aling Rosa", dialogueNode = word53 });
                }
                
                EditorUtility.SetDirty(npc);
            }
        }
        
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Narrative Nodes successfully created and injected into the scene!");
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

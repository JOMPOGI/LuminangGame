using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class MassiveDialogueImporter : EditorWindow
{
    [System.Serializable]
    public class ScriptWord
    {
        public int id;
        public string title;
        public string speaker;
        public string teachText;
        public string expectedSTT;
        public string successText;
    }

    [System.Serializable]
    public class ScriptData
    {
        public ScriptWord[] words;
    }

    [MenuItem("Luminang/Import Massive Dialogue Script")]
    public static void RunImport()
    {
        string jsonPath = "Assets/Editor/MassiveScript.json";
        if (!File.Exists(jsonPath))
        {
            Debug.LogError("MassiveScript.json not found!");
            return;
        }

        string json = File.ReadAllText(jsonPath);
        ScriptData data = JsonUtility.FromJson<ScriptData>(json);

        string outFolder = "Assets/Dialogues/MassiveImport";
        if (!AssetDatabase.IsValidFolder("Assets/Dialogues"))
            AssetDatabase.CreateFolder("Assets", "Dialogues");
        if (!AssetDatabase.IsValidFolder(outFolder))
            AssetDatabase.CreateFolder("Assets/Dialogues", "MassiveImport");

        Dictionary<string, DialogueNode> firstNodes = new Dictionary<string, DialogueNode>();
        DialogueNode previousSuccessNode = null;
        string previousSpeaker = "";

        foreach (var word in data.words)
        {
            string speaker = word.speaker.Trim();
            
            // 1. Create Teach Node
            DialogueNode teachNode = ScriptableObject.CreateInstance<DialogueNode>();
            teachNode.speakerName = speaker;
            teachNode.dialogueText = word.teachText;
            teachNode.choices = new List<DialogueChoice>();
            
            // 2. Create Success Node
            DialogueNode successNode = ScriptableObject.CreateInstance<DialogueNode>();
            successNode.speakerName = speaker;
            successNode.dialogueText = word.successText;
            
            // 3. Setup STT Choice on Teach Node
            DialogueChoice sttChoice = new DialogueChoice();
            sttChoice.choiceText = "Speak";
            
            string sttWord = word.expectedSTT.ToLower().Trim('?', '!', '.', ' ');
            // Some expectedSTT might have [Player Name] which should be allowed as wildcards
            if (sttWord.Contains("[")) sttWord = sttWord.Substring(0, sttWord.IndexOf("[")).Trim();
            
            sttChoice.expectedSTTWord = sttWord;
            sttChoice.nextNode = successNode;
            sttChoice.choiceEvent = "StartSTT";
            teachNode.choices.Add(sttChoice);

            // Save Assets
            string sttSafe = string.Join("_", sttWord.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");
            string teachName = $"{word.id:00}_{speaker}_{sttSafe}_Teach";
            string successName = $"{word.id:00}_{speaker}_{sttSafe}_Success";

            AssetDatabase.CreateAsset(teachNode, $"{outFolder}/{teachName}.asset");
            AssetDatabase.CreateAsset(successNode, $"{outFolder}/{successName}.asset");

            if (!firstNodes.ContainsKey(speaker))
            {
                firstNodes[speaker] = teachNode;
            }

            // Wire previous success to this teach IF same speaker
            if (previousSpeaker == speaker && previousSuccessNode != null)
            {
                previousSuccessNode.choices = new List<DialogueChoice>();
                previousSuccessNode.choices.Add(new DialogueChoice { choiceText = "Next", nextNode = teachNode });
                EditorUtility.SetDirty(previousSuccessNode);
            }

            previousSpeaker = speaker;
            previousSuccessNode = successNode;
        }

        // --- Generate Kalaw Intro & Post Quest ---
        DialogueNode kalawIntro = ScriptableObject.CreateInstance<DialogueNode>();
        kalawIntro.speakerName = "Kalaw";
        kalawIntro.dialogueText = "Squawk! Oh... is that a fruit in your hand? May I have some? Flying under this hot Vigan sun has completely worn me out...";
        kalawIntro.choices = new List<DialogueChoice>();
        
        DialogueNode kalawPost = ScriptableObject.CreateInstance<DialogueNode>();
        kalawPost.speakerName = "Kalaw";
        kalawPost.dialogueText = "Mmm, sweet and juicy! Thank you so much, traveler!\nAh, where are my manners—I am Kalaw, your companion and guide through these lands!\nWait a second... look at that ancient anting-anting pendant resting on your chest! Feel that faint hum?\nThe Ilocos Language Crystal inside it is sleeping.\nTo charge it up and awaken its power, you've got to explore Calle Crisologo and speak with the locals in their native tongue.\nAs you practice and learn how Ilocanos talk in their daily lives, your voice will help restore the crystal's power.\nSo, let's begin with something simple.\nBefore you can truly connect with the people of Ilocos, you must first learn how to greet them.";
        kalawPost.choices = new List<DialogueChoice>();
        
        // Link Intro to Post
        kalawIntro.choices.Add(new DialogueChoice { choiceText = "Hand over Fruit", nextNode = kalawPost, choiceEvent = "CompleteObjective" });
        if (firstNodes.ContainsKey("Kalaw"))
        {
            kalawPost.choices.Add(new DialogueChoice { choiceText = "Learn to Greet", nextNode = firstNodes["Kalaw"] });
        }
        
        AssetDatabase.CreateAsset(kalawIntro, $"{outFolder}/00_Kalaw_Intro.asset");
        AssetDatabase.CreateAsset(kalawPost, $"{outFolder}/00_Kalaw_PostQuest.asset");
        firstNodes["Kalaw"] = kalawIntro; // Override Kalaw's first node

        // Assign to NPCs
        var allNPCs = FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int matched = 0;
        
        foreach (var npc in allNPCs)
        {
            string goName = npc.gameObject.name.ToLower();
            string bestMatch = null;
            
            // Match heuristics
            if (goName.Contains("kalaw")) bestMatch = "Kalaw";
            else if (goName.Contains("kyros")) bestMatch = "Kyros";
            else if (goName.Contains("irah")) bestMatch = "Irah";
            else if (goName.Contains("jom")) bestMatch = "Jom";
            else if (goName.Contains("ronnie")) bestMatch = "Ronnie";
            else if (goName.Contains("sally")) bestMatch = "Sally";
            else if (goName.Contains("lito")) bestMatch = "Lito";
            else if (goName.Contains("lakay")) bestMatch = "Apo Lakay";
            else if (goName.Contains("tomas")) bestMatch = "Tomas";
            else if (goName.Contains("klara")) bestMatch = "Klara";
            else if (goName.Contains("tala")) bestMatch = "Tala";
            else if (goName.Contains("lance")) bestMatch = "Mang Lance";
            else if (goName.Contains("rayo")) bestMatch = "Rayo";
            else if (goName.Contains("rosa")) bestMatch = "Aling Rosa";
            else if (goName.Contains("nida")) bestMatch = "Lola Nida";
            else if (goName.Contains("neneng")) bestMatch = "Neneng";
            else if (goName.Contains("riza")) bestMatch = "Aling Riza";
            else if (goName.Contains("bebang")) bestMatch = "Lola Bebang";
            
            if (bestMatch != null && firstNodes.ContainsKey(bestMatch))
            {
                Undo.RecordObject(npc, "Assign Massive Script");
                npc.defaultDialogue = firstNodes[bestMatch];
                npc.interactionEnabled = true;
                npc.questDialogues.Clear(); // Clear overrides
                EditorUtility.SetDirty(npc);
                matched++;
            }
        }
        
        // Kalaw Final Evaluation
        UltimateScriptImporter.RunImport();
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        
        AssetDatabase.SaveAssets();
        Debug.Log($"Massive Import Complete! Created {data.words.Length * 2} nodes. Assigned to {matched} NPCs.");
    }
}

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class DialogueImporter : EditorWindow
{
    [MenuItem("Tools/Import Calle Crisologo Dialogues")]
    public static void ImportDialogues()
    {
        string filePath = "Assets/Dialogues/new_calle_crisologo_dialogues.txt";
        string outputFolder = "Assets/Dialogues/CalleCrisologo";
        
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            AssetDatabase.CreateFolder("Assets/Dialogues", "CalleCrisologo");
        }

        string[] lines = File.ReadAllLines(filePath);
        
        List<NPCBlock> npcBlocks = ParseBlocks(lines);
        
        foreach (var block in npcBlocks)
        {
            ProcessNPCBlock(block, outputFolder);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Successfully imported " + npcBlocks.Count + " NPC dialogue sequences!");
    }

    class NPCBlock
    {
        public string Name;
        public string StartDialogue;
        public List<WordBlock> Words = new List<WordBlock>();
    }

    class WordBlock
    {
        public string TargetWord;
        public string TeachDialogue;
        public string SuccessDialogue;
    }

    static List<NPCBlock> ParseBlocks(string[] lines)
    {
        List<NPCBlock> blocks = new List<NPCBlock>();
        NPCBlock currentBlock = null;
        WordBlock currentWord = null;
        
        string currentMode = ""; // "start", "teach", "success"
        bool inQuote = false;
        
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (!inQuote)
            {
                if (Regex.IsMatch(line, @"^\d+\.\s+([A-Z\s]+)\s*—"))
                {
                    Match m = Regex.Match(line, @"^\d+\.\s+([A-Z\s]+)\s*—");
                    currentBlock = new NPCBlock();
                    currentBlock.Name = m.Groups[1].Value.Trim();
                    blocks.Add(currentBlock);
                    currentMode = "start";
                    continue;
                }

                if (line.StartsWith("Word ") && line.Contains("?"))
                {
                    currentWord = new WordBlock();
                    currentWord.TargetWord = line.Split('?')[1].Trim().ToLower();
                    if (currentBlock != null)
                    {
                        currentBlock.Words.Add(currentWord);
                    }
                    currentMode = "";
                    continue;
                }

                if (currentBlock != null)
                {
                    if (line.Contains("Initial Dialogue") || line.Contains("Post-Quest") || (line.Contains("Teach:") && currentWord == null))
                    {
                        currentMode = "start";
                        continue;
                    }
                    
                    if (currentWord != null && line.Contains("— Teach:"))
                    {
                        currentMode = "teach";
                        continue;
                    }
                    if (currentWord != null && line.Contains("— Success:"))
                    {
                        currentMode = "success";
                        continue;
                    }
                }
            }

            if (currentBlock != null)
            {
                if (line.StartsWith("\""))
                {
                    inQuote = true;
                }
                
                if (inQuote)
                {
                    string text = line.Replace("\"", "").Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (currentMode == "start")
                        {
                            currentBlock.StartDialogue = string.IsNullOrEmpty(currentBlock.StartDialogue) ? text : currentBlock.StartDialogue + "\n" + text;
                        }
                        else if (currentMode == "teach" && currentWord != null)
                        {
                            currentWord.TeachDialogue = string.IsNullOrEmpty(currentWord.TeachDialogue) ? text : currentWord.TeachDialogue + "\n" + text;
                        }
                        else if (currentMode == "success" && currentWord != null)
                        {
                            currentWord.SuccessDialogue = string.IsNullOrEmpty(currentWord.SuccessDialogue) ? text : currentWord.SuccessDialogue + "\n" + text;
                        }
                    }
                    
                    if (line.EndsWith("\""))
                    {
                        inQuote = false;
                    }
                }
            }
        }
        
        return blocks;
    }

    static void ProcessNPCBlock(NPCBlock block, string outputFolder)
    {
        string safeName = block.Name.Replace(" ", "").Replace("VENDOR", "Vendor");
        if (safeName == "KALAW") safeName = "Kalaw";
        if (safeName == "RONNIE") safeName = "Ronnie";
        if (safeName == "SALLY") safeName = "Sally";
        if (safeName == "LITO") safeName = "Lito";
        if (safeName == "APOLAKAY") safeName = "ApoLakay";
        if (safeName == "TOMAS") safeName = "Tomas";
        if (safeName == "KLARA") safeName = "Klara";
        if (safeName == "TALA") safeName = "Tala";
        if (safeName == "MANGLANCE") safeName = "MangLance";
        if (safeName == "RAYO") safeName = "Rayo";
        if (safeName == "ALINGROSA") safeName = "AlingRosa";
        if (safeName == "LOLANIDA") safeName = "LolaNida";
        if (safeName == "NENENG") safeName = "Neneng";
        if (safeName == "ALINGRIZA") safeName = "AlingRiza";
        if (safeName == "LOLABEBANG") safeName = "LolaBebang";

        DialogueNode startNode = GetOrCreateNode($"{outputFolder}/{safeName}_Start.asset");
        startNode.speakerName = block.Name.Contains("VENDOR") ? block.Name : safeName;
        if (!string.IsNullOrEmpty(block.StartDialogue))
        {
            startNode.dialogueText = block.StartDialogue;
        }
        else
        {
            startNode.dialogueText = "Hello!";
        }

        DialogueNode previousNode = startNode;

        for (int i = 0; i < block.Words.Count; i++)
        {
            var word = block.Words[i];
            
            DialogueNode modelNode = GetOrCreateNode($"{outputFolder}/{safeName}_Word_{i}_Model.asset");
            modelNode.speakerName = startNode.speakerName;
            modelNode.dialogueText = word.TeachDialogue;

            DialogueNode successNode = GetOrCreateNode($"{outputFolder}/{safeName}_Word_{i}_Success.asset");
            successNode.speakerName = startNode.speakerName;
            successNode.dialogueText = word.SuccessDialogue;

            if (previousNode.choices == null) previousNode.choices = new List<DialogueChoice>();
            previousNode.choices.Clear();
            DialogueChoice nextChoice = new DialogueChoice();
            nextChoice.nextNode = modelNode;
            previousNode.choices.Add(nextChoice);

            if (modelNode.choices == null) modelNode.choices = new List<DialogueChoice>();
            modelNode.choices.Clear();
            DialogueChoice sttChoice = new DialogueChoice();
            sttChoice.choiceEvent = "StartSTT";
            sttChoice.expectedSTTWord = word.TargetWord;
            sttChoice.nextNode = successNode;
            modelNode.choices.Add(sttChoice);

            previousNode = successNode;
            EditorUtility.SetDirty(modelNode);
            EditorUtility.SetDirty(successNode);
        }

        if (previousNode.choices != null)
        {
            previousNode.choices.Clear();
        }

        EditorUtility.SetDirty(startNode);
    }

    static DialogueNode GetOrCreateNode(string path)
    {
        DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        if (node == null)
        {
            node = ScriptableObject.CreateInstance<DialogueNode>();
            AssetDatabase.CreateAsset(node, path);
        }
        if (node.choices == null) node.choices = new List<DialogueChoice>();
        return node;
    }
}

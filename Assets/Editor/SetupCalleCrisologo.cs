using UnityEngine;
using UnityEditor;
using System.Linq;

public class SetupCalleCrisologo : EditorWindow
{
    [MenuItem("Luminang/Auto-Setup Calle Crisologo")]
    public static void RunSetup()
    {
        int catsSetup = 0;
        int npcsFixed = 0;
        int portraitsAssigned = 0;

        // 1. Assign Speaker Portraits to all DialogueNodes
        string[] dialogueGuids = AssetDatabase.FindAssets("t:DialogueNode");
        foreach (string guid in dialogueGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(assetPath);
            if (node != null && !string.IsNullOrEmpty(node.speakerName) && node.speakerPortrait == null)
            {
                // Clean the speaker name (remove spaces)
                string cleanName = node.speakerName.Replace(" ", "");
                
                // Try finding the sprite with or without "Image"
                string spritePath1 = $"Assets/Sprites/NPCs/{cleanName}.png";
                string spritePath2 = $"Assets/Sprites/NPCs/{cleanName}Image.png";
                
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath1);
                if (sprite == null) sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath2);

                if (sprite != null)
                {
                    Undo.RecordObject(node, "Assign Speaker Portrait");
                    node.speakerPortrait = sprite;
                    EditorUtility.SetDirty(node);
                    portraitsAssigned++;
                }
            }
        }
        AssetDatabase.SaveAssets();

        // 2. Create the Cat Dialogue Asset if it doesn't exist
        string path = "Assets/Dialogues/Cat_Meow.asset";
        DialogueNode catDialogue = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        
        if (catDialogue == null)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Dialogues"))
            {
                AssetDatabase.CreateFolder("Assets", "Dialogues");
            }

            catDialogue = ScriptableObject.CreateInstance<DialogueNode>();
            catDialogue.speakerName = "Cat";
            catDialogue.dialogueText = "Meow... Meow...";
            AssetDatabase.CreateAsset(catDialogue, path);
            AssetDatabase.SaveAssets();
            Debug.Log("Created new Cat Dialogue Node at " + path);
        }

        // 3. Find all GameObjects in the active scene
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (GameObject go in allObjects)
        {
            // Setup Cats
            if (go.name.Contains("Cat", System.StringComparison.OrdinalIgnoreCase))
            {
                // Must have a renderer/animator to be an NPC
                if (go.GetComponent<Animator>() != null)
                {
                    InteractableNPC interactable = go.GetComponent<InteractableNPC>();
                    if (interactable == null)
                    {
                        interactable = Undo.AddComponent<InteractableNPC>(go);
                    }
                    
                    if (interactable.defaultDialogue == null || interactable.defaultDialogue.speakerName != "Cat")
                    {
                        interactable.defaultDialogue = catDialogue;
                        interactable.npcAnimator = go.GetComponent<Animator>();
                        interactable.promptText = "Interact"; // <--- NEW
                        EditorUtility.SetDirty(interactable);
                        catsSetup++;
                    }
                }
            }

            // Fix NPCs to hide button/indicator when objective is done
            InteractableNPC npc = go.GetComponent<InteractableNPC>();
            if (npc != null && !go.name.Contains("Cat", System.StringComparison.OrdinalIgnoreCase))
            {
                // Make sure they have a Talk button by default
                if (npc.promptText != "Talk")
                {
                    Undo.RecordObject(npc, "Set Prompt Text");
                    npc.promptText = "Talk";
                    EditorUtility.SetDirty(npc);
                }

                if (npc.questDialogues != null && npc.questDialogues.Count > 0 && npc.defaultDialogue != null)
                {
                    Undo.RecordObject(npc, "Clear Default Dialogue");
                    npc.defaultDialogue = null;
                    EditorUtility.SetDirty(npc);
                    npcsFixed++;
                }
            }
        }

        Debug.Log($"Auto-Setup Complete: Assigned {portraitsAssigned} portraits, setup {catsSetup} Cats, and fixed interaction logic for {npcsFixed} quest NPCs.");
        EditorUtility.DisplayDialog("Setup Complete", 
            $"Successfully matched and assigned {portraitsAssigned} portraits!\n\nSuccessfully setup {catsSetup} Cats to meow!\n\nFixed {npcsFixed} NPCs to hide their indicators/buttons when their quest is done.", "Awesome!");
    }
}

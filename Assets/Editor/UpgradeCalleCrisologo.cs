using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class UpgradeCalleCrisologo : EditorWindow
{
    [MenuItem("Tools/Upgrade Calle Crisologo (Port Magellan Features)")]
    public static void DoWork()
    {
        // 1. Open Magellan's Cross additively to copy systems
        string magellanPath = "Assets/Scenes/Environments/Magellan's_Cross.unity";
        Scene magellanScene = EditorSceneManager.OpenScene(magellanPath, OpenSceneMode.Additive);
        
        var activeScene = SceneManager.GetActiveScene(); // Should be Calle Crisologo
        if (!activeScene.name.Contains("Calle_Crisologo"))
        {
            Debug.LogError("Please open Calle_Crisologo scene before running this tool!");
            EditorSceneManager.CloseScene(magellanScene, true);
            return;
        }

        var magellanRoots = magellanScene.GetRootGameObjects();
        List<GameObject> objectsToCopy = new List<GameObject>();
        
        foreach (var root in magellanRoots)
        {
            string lowerName = root.name.ToLower();
            if (lowerName == "ui" || 
                lowerName.Contains("minigame") || 
                lowerName.Contains("quiz") || 
                lowerName.Contains("word rush") || 
                lowerName.Contains("matching") || 
                lowerName.Contains("fishing"))
            {
                objectsToCopy.Add(root);
            }
        }
        
        // 2. Prepare Calle Crisologo
        var calleRoots = activeScene.GetRootGameObjects();
        foreach (var root in calleRoots)
        {
            if (root.name == "UI")
            {
                root.name = "UI_OLD_BACKUP";
                root.SetActive(false); // Backup the old UI instead of deleting it, just in case
                break;
            }
        }
        
        // 3. Copy the systems over
        int copiedCount = 0;
        foreach (var obj in objectsToCopy)
        {
            GameObject copy = Instantiate(obj);
            copy.name = obj.name; // Remove "(Clone)"
            SceneManager.MoveGameObjectToScene(copy, activeScene);
            EditorUtility.SetDirty(copy);
            copiedCount++;
        }
        
        EditorSceneManager.CloseScene(magellanScene, true);
        
        // 4. Upgrade Dialogues with Portraits
        string[] dialogueGuids = AssetDatabase.FindAssets("t:DialogueNode", new[] { "Assets/Dialogues/CalleCrisologo" });
        int portraitCount = 0;
        foreach (string guid in dialogueGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
            if (node != null && string.IsNullOrEmpty(node.speakerName) == false && node.speakerPortrait == null)
            {
                // Try to find a portrait matching their name in the project
                string searchName = node.speakerName.Replace(" ", "");
                string[] portraitGuids = AssetDatabase.FindAssets(searchName + " t:Sprite");
                if (portraitGuids.Length > 0)
                {
                    string portraitPath = AssetDatabase.GUIDToAssetPath(portraitGuids[0]);
                    Sprite portrait = AssetDatabase.LoadAssetAtPath<Sprite>(portraitPath);
                    if (portrait != null)
                    {
                        node.speakerPortrait = portrait;
                        EditorUtility.SetDirty(node);
                        portraitCount++;
                    }
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveScene(activeScene);
        
        Debug.Log($"<color=green>SUCCESS: Upgraded Calle Crisologo! Copied {copiedCount} systems (UI/Minigames) from Magellan's Cross and auto-assigned {portraitCount} speaking portraits!</color>");
    }
}

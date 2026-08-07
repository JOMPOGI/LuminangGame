using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine.Events;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ReplaceWithFishingGame : EditorWindow
{
    [MenuItem("Tools/Luminang/Replace Minigames with Fishing Game")]
    public static void ReplaceAll()
    {
        string scenePath = "Assets/Scenes/Environments/Calle_Crisologo.unity";
        Scene currentScene = EditorSceneManager.GetActiveScene();
        
        bool wasOpen = currentScene.path == scenePath;
        if (!wasOpen)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                currentScene = EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                Debug.LogWarning("Aborted because unsaved scenes could not be saved.");
                return;
            }
        }

        // 1. Update Dialogue Assets
        string[] dialogueGuids = AssetDatabase.FindAssets("t:DialogueNode", new[] { "Assets/Dialogues/CalleCrisologo" });
        int dialogueMods = 0;
        foreach (string guid in dialogueGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
            
            bool changed = false;
            if (node.choices != null)
            {
                foreach (var choice in node.choices)
                {
                    if (choice.choiceEvent != null && 
                       (choice.choiceEvent.Contains("WordRush") || 
                        choice.choiceEvent.Contains("TwoTruths") || 
                        choice.choiceEvent.Contains("Matching")))
                    {
                        choice.choiceEvent = "StartMinigame:FishingGame";
                        choice.choiceText = "Play Fishing Game!";
                        changed = true;
                    }
                }
            }
            
            if (changed)
            {
                EditorUtility.SetDirty(node);
                dialogueMods++;
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log($"Updated {dialogueMods} dialogue nodes to point to FishingGame.");

        // 2. Update NPCs
        InteractableNPC[] allNPCs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int npcMods = 0;
        
        Dictionary<string, string> npcToCategory = new Dictionary<string, string>()
        {
            { "kyros", "Greetings" },
            { "jom", "Greetings" }, // Fallback for early quests
            { "sally", "Identity" },
            { "lito", "Requests" },
            { "klara", "Directions" },
            { "manglance", "Count" },
            { "alingrosa", "Action Verbs" },
            { "alingriza", "Linking Verbs" }, // Also pronouns, but setting one is fine
            { "lolabebang", "Interrogatives" }
        };

        foreach (var npc in allNPCs)
        {
            string lowerName = npc.name.ToLower();
            if (lowerName.Contains("kalaw")) continue; // Skip bird quiz
            
            // Check if NPC has dialogue events mapped to Minigames
            SerializedObject so = new SerializedObject(npc);
            SerializedProperty dialogueEvents = so.FindProperty("dialogueEvents");
            bool modifiedNPC = false;

            if (dialogueEvents != null && dialogueEvents.isArray)
            {
                for (int i = 0; i < dialogueEvents.arraySize; i++)
                {
                    SerializedProperty element = dialogueEvents.GetArrayElementAtIndex(i);
                    SerializedProperty eventName = element.FindPropertyRelative("eventName");
                    
                    if (eventName != null && eventName.stringValue.StartsWith("StartMinigame") && !eventName.stringValue.Contains("Tiptip"))
                    {
                        // Update string to match new Dialogue mapping
                        eventName.stringValue = "StartMinigame:FishingGame";
                        
                        // Add or get SceneMinigameTrigger
                        SceneMinigameTrigger trigger = npc.GetComponent<SceneMinigameTrigger>();
                        if (trigger == null) trigger = npc.gameObject.AddComponent<SceneMinigameTrigger>();
                        
                        trigger.minigameSceneName = "FishingGameScene";
                        trigger.targetLanguage = "ilokano";
                        trigger.useLoadingScreen = true;
                        
                        // Infer category
                        trigger.categoryFilter = "Greetings"; // default
                        foreach (var kvp in npcToCategory)
                        {
                            if (lowerName.Contains(kvp.Key))
                            {
                                trigger.categoryFilter = kvp.Value;
                                break;
                            }
                        }

                        // Update the UnityEvent directly
                        SerializedProperty onEventTriggered = element.FindPropertyRelative("onEventTriggered");
                        SerializedProperty persistentCalls = onEventTriggered.FindPropertyRelative("m_PersistentCalls.m_Calls");
                        
                        // Clear old calls (like MinigameManager.StartMinigame)
                        persistentCalls.ClearArray();
                        so.ApplyModifiedProperties(); // Apply before adding via UnityEventTools
                        
                        // Add new call to SceneMinigameTrigger.StartMinigameScene()
                        UnityAction methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction), trigger, "StartMinigameScene") as UnityAction;
                        UnityEventTools.AddPersistentListener(npc.dialogueEvents[i].onEventTriggered, methodDelegate);
                        
                        so.Update(); // refresh serialized object
                        modifiedNPC = true;
                    }
                }
            }

            if (modifiedNPC)
            {
                EditorUtility.SetDirty(npc);
                npcMods++;
            }
        }

        if (npcMods > 0)
        {
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            Debug.Log($"<color=green>SUCCESS: Converted {npcMods} NPCs to use the Fishing Game Scene transition!</color>");
        }
        else
        {
            Debug.LogWarning("No NPCs found with StartMinigame mapping (excluding Kalaw).");
        }
    }
}

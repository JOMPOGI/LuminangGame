using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class ReplaceKalawQuizPrefab : EditorWindow
{
    [MenuItem("Tools/Luminang/Replace Kalaw Quiz Prefab")]
    public static void ReplacePrefab()
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

        string kalawPrefabPath = "Assets/Prefabs/Mini Games/KalawQuizBubble.prefab";
        GameObject kalawPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(kalawPrefabPath);
        
        if (kalawPrefab == null)
        {
            Debug.LogError("Could not find KalawQuizBubble at " + kalawPrefabPath);
            return;
        }

        InteractableNPC[] allNPCs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int replacements = 0;

        foreach (var npc in allNPCs)
        {
            if (npc.name.Contains("Kalaw") || npc.name.Contains("NPC")) // Specifically target Kalaw or all NPCs just in case
            {
                // We actually need to modify the serialized UnityEvent.
                // UnityEvents are hard to modify via code safely, so we use SerializedObject.
                SerializedObject so = new SerializedObject(npc);
                SerializedProperty dialogueEvents = so.FindProperty("dialogueEvents");

                if (dialogueEvents != null && dialogueEvents.isArray)
                {
                    for (int i = 0; i < dialogueEvents.arraySize; i++)
                    {
                        SerializedProperty element = dialogueEvents.GetArrayElementAtIndex(i);
                        SerializedProperty eventName = element.FindPropertyRelative("eventName");
                        
                        if (eventName != null && eventName.stringValue.StartsWith("StartTiptipQuiz"))
                        {
                            SerializedProperty onDialogueEvent = element.FindPropertyRelative("onEventTriggered");
                            if (onDialogueEvent != null)
                            {
                                SerializedProperty persistentCalls = onDialogueEvent.FindPropertyRelative("m_PersistentCalls.m_Calls");
                                
                                if (persistentCalls != null)
                                {
                                    for (int j = 0; j < persistentCalls.arraySize; j++)
                                    {
                                SerializedProperty call = persistentCalls.GetArrayElementAtIndex(j);
                                SerializedProperty methodName = call.FindPropertyRelative("m_MethodName");
                                
                                if (methodName != null && methodName.stringValue == "StartMinigame")
                                {
                                    SerializedProperty args = call.FindPropertyRelative("m_Arguments");
                                    SerializedProperty objectArg = args.FindPropertyRelative("m_ObjectArgument");
                                    
                                            if (objectArg != null)
                                            {
                                                objectArg.objectReferenceValue = kalawPrefab;
                                                replacements++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (so.hasModifiedProperties)
                {
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(npc);
                }
            }
        }

        if (replacements > 0)
        {
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            Debug.Log($"<color=green>SUCCESS: Fixed {replacements} references to point to KalawQuizBubble!</color>");
        }
        else
        {
            Debug.LogWarning("No replacements were made. Make sure the NPC Kalaw has dialogueEvents for StartTiptipQuiz.");
        }
    }
}

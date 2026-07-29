using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class AutomateCalleSetup
{
    [MenuItem("Tools/Automate Calle Crisologo Setup")]
    public static void RunSetup()
    {
        Scene activeScene = EditorSceneManager.GetActiveScene();
        if (activeScene.name != "Calle_Crisologo")
        {
            Debug.LogError("Please open the Calle_Crisologo scene first!");
            return;
        }

        // 1. InSceneLessonController STT Region
        var lessonController = Object.FindObjectOfType<InSceneLessonController>(true);
        if (lessonController != null)
        {
            Undo.RecordObject(lessonController, "Set STT Region");
            lessonController.sttRegion = RegionMode.Ilokano;
            EditorUtility.SetDirty(lessonController);
            Debug.Log("Set InSceneLessonController sttRegion to Ilokano.");
        }
        else
        {
            Debug.LogWarning("InSceneLessonController not found in scene!");
        }

        // 2. Setup NPCs
        var interactables = Object.FindObjectsOfType<InteractableNPC>(true);
        
        string[] npcNames = { "Kalaw", "VendorKyros", "VendorIrah", "VendorJom", "Ronnie", "Sally", 
                              "Lito", "ApoLakay", "Tomas", "Klara", "Tala", "MangLance", 
                              "Rayo", "AlingRosa", "LolaNida", "Neneng", "AlingRiza", "LolaBebang" };
        
        GameObject closeUpParent = GameObject.Find("CloseUpCameras");
        if (closeUpParent == null)
        {
            closeUpParent = new GameObject("CloseUpCameras");
            Undo.RegisterCreatedObjectUndo(closeUpParent, "Create CloseUpCameras");
        }

        foreach (var npcName in npcNames)
        {
            InteractableNPC npcComponent = null;
            
            // Try to find the matching NPC GameObject based on name
            foreach (var i in interactables)
            {
                string objName = i.gameObject.name.Replace(" ", "").Replace("_", "").ToLower();
                string targetName = npcName.ToLower();
                
                if (objName.Contains(targetName) || 
                   (targetName == "vendorkyros" && objName.Contains("kyros")) ||
                   (targetName == "vendorirah" && objName.Contains("irah")) ||
                   (targetName == "vendorjom" && objName.Contains("jom")))
                {
                    npcComponent = i;
                    break;
                }
            }

            if (npcComponent != null)
            {
                string assetName = (npcName == "Kalaw") ? "Kalaw_QuestIntro" : $"{npcName}_Node_1";
                string assetPath = $"Assets/Dialogues/CalleCrisologo_New/{assetName}.asset";
                DialogueNode startingNode = AssetDatabase.LoadAssetAtPath<DialogueNode>(assetPath);
                
                if (startingNode != null)
                {
                    Undo.RecordObject(npcComponent, "Set Default Dialogue");
                    npcComponent.defaultDialogue = startingNode;
                    
                    // Clear out old quest dialogues since we handle quests directly in the nodes now
                    if (npcComponent.questDialogues != null)
                    {
                        npcComponent.questDialogues.Clear();
                        
                        if (npcName == "Kalaw")
                        {
                            DialogueNode postQuestNode = AssetDatabase.LoadAssetAtPath<DialogueNode>("Assets/Dialogues/CalleCrisologo_New/Kalaw_Node_0.asset");
                            if (postQuestNode != null)
                            {
                                npcComponent.questDialogues.Add(new InteractableNPC.QuestDialogue { 
                                    requiredObjective = "Talk to Kalaw", 
                                    dialogueNode = postQuestNode 
                                });
                            }
                        }
                    }
                    
                    EditorUtility.SetDirty(npcComponent);
                    Debug.Log($"Assigned {assetName} to {npcComponent.gameObject.name}");
                }
                else
                {
                    Debug.LogError($"Could not find asset {assetPath}");
                }

                // 3. Create CloseUp Camera Target
                string camName = npcName == "VendorKyros" ? "KyrosCloseUp" : 
                                 npcName == "VendorIrah" ? "IrahCloseUp" : 
                                 npcName == "VendorJom" ? "JomCloseUp" : 
                                 $"{npcName}CloseUp";
                
                Transform existingCam = closeUpParent.transform.Find(camName);
                GameObject camObj;
                if (existingCam == null)
                {
                    camObj = new GameObject(camName);
                    camObj.transform.SetParent(closeUpParent.transform);
                    Undo.RegisterCreatedObjectUndo(camObj, "Create CloseUp Target");
                }
                else
                {
                    camObj = existingCam.gameObject;
                }
                
                // Position cam relative to NPC (slightly above and in front)
                Undo.RecordObject(camObj.transform, "Position CloseUp Target");
                Vector3 forward = npcComponent.transform.forward;
                camObj.transform.position = npcComponent.transform.position + (forward * 1.5f) + (Vector3.up * 1.5f);
                camObj.transform.rotation = Quaternion.LookRotation(-forward);
            }
            else
            {
                Debug.LogWarning($"Could not find NPC named {npcName} in the scene.");
            }
        }

        EditorSceneManager.MarkSceneDirty(activeScene);
        Debug.Log("Calle Crisologo setup complete! Don't forget to save the scene.");
    }
}

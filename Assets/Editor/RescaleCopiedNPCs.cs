using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class RescaleCopiedNPCs : EditorWindow
{
    [MenuItem("Tools/Luminang/Rescale Copied NPCs")]
    public static void RescaleNPCs()
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

        // Find a reference native Calle Crisologo NPC
        GameObject referenceNPC = GameObject.Find("Jen_Rigged") ?? GameObject.Find("Wayne_Rigged");
        if (referenceNPC == null)
        {
            Debug.LogError("Could not find a native Calle Crisologo NPC (e.g., Jen_Rigged or Wayne_Rigged) to use as a reference.");
            return;
        }

        Vector3 targetScale = referenceNPC.transform.localScale;
        CapsuleCollider refCollider = referenceNPC.GetComponent<CapsuleCollider>();
        UnityEngine.AI.NavMeshAgent refAgent = referenceNPC.GetComponent<UnityEngine.AI.NavMeshAgent>();

        InteractableNPC[] allNPCs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int modifications = 0;

        foreach (var npc in allNPCs)
        {
            string name = npc.name.ToLower();
            
            // Check if this is a copied NPC (excluding Kalaw the bird)
            if ((name.Contains("ronnie") || name.Contains("sally") || name.Contains("manglance") || name.Contains("aling")) 
                && !name.Contains("closeup")) // Ignore close-up models, just do the world models
            {
                // Update Scale
                npc.transform.localScale = targetScale;
                
                // Update Collider
                CapsuleCollider col = npc.GetComponent<CapsuleCollider>();
                if (col != null && refCollider != null)
                {
                    col.height = refCollider.height;
                    col.radius = refCollider.radius;
                    col.center = refCollider.center;
                }

                // Update NavMeshAgent
                UnityEngine.AI.NavMeshAgent agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null && refAgent != null)
                {
                    agent.height = refAgent.height;
                    agent.radius = refAgent.radius;
                    agent.speed = refAgent.speed;
                }
                
                // Update SphereCollider for interaction triggers if any
                SphereCollider[] sphereCols = npc.GetComponents<SphereCollider>();
                SphereCollider[] refSphereCols = referenceNPC.GetComponents<SphereCollider>();
                if (sphereCols.Length > 0 && refSphereCols.Length > 0)
                {
                    // Find the trigger collider
                    foreach (var sc in sphereCols)
                    {
                        if (sc.isTrigger)
                        {
                            foreach(var rsc in refSphereCols)
                            {
                                if (rsc.isTrigger)
                                {
                                    sc.radius = rsc.radius;
                                    sc.center = rsc.center;
                                    break;
                                }
                            }
                        }
                    }
                }
                
                // Update Animator Speed / Settings if needed
                Animator anim = npc.GetComponent<Animator>();
                Animator refAnim = referenceNPC.GetComponent<Animator>();
                // Only if necessary to keep animations consistent (but typically not required for scaling, just mentioning it as a possibility).

                EditorUtility.SetDirty(npc);
                modifications++;
                Debug.Log($"[RescaleCopiedNPCs] Successfully rescaled: {npc.name}");
            }
        }

        if (modifications > 0)
        {
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            Debug.Log($"<color=green>SUCCESS: Rescaled {modifications} copied NPCs to match native Calle Crisologo proportions.</color>");
        }
        else
        {
            Debug.LogWarning("No copied NPCs found to rescale. Make sure Ronnie, Sally, MangLance, or Aling are in the scene.");
        }
    }
}

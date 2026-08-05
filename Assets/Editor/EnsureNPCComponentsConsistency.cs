using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class EnsureNPCComponentsConsistency : EditorWindow
{
    [MenuItem("Tools/Luminang/Ensure NPC Components Consistency")]
    public static void FixComponents()
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

        GameObject jom = GameObject.Find("PlayerArmature");
        if (jom == null)
        {
            Debug.LogError("Could not find Jom (PlayerArmature) in the scene.");
            return;
        }

        // Get Jom's reference values (or sensible defaults for a human)
        float targetWorldHeight = 1.8f;
        float targetWorldRadius = 0.3f;
        float targetNavSpeed = 3.5f;

        CharacterController jomCC = jom.GetComponent<CharacterController>();
        if (jomCC != null)
        {
            targetWorldHeight = jomCC.height * jom.transform.lossyScale.y;
            targetWorldRadius = jomCC.radius * Mathf.Max(jom.transform.lossyScale.x, jom.transform.lossyScale.z);
        }

        InteractableNPC[] allNPCs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int modifications = 0;

        foreach (var npc in allNPCs)
        {
            string name = npc.name.ToLower();
            if (name.Contains("kalaw") || name.Contains("neneng") || name.Contains("closeup"))
                continue;

            bool modified = false;

            // 1. Capsule Collider (Local Space, scales with lossyScale)
            CapsuleCollider col = npc.GetComponent<CapsuleCollider>();
            if (col != null)
            {
                Vector3 scale = npc.transform.lossyScale;
                float expectedLocalHeight = targetWorldHeight / scale.y;
                float expectedLocalRadius = targetWorldRadius / Mathf.Max(scale.x, scale.z);
                float expectedLocalCenterY = expectedLocalHeight / 2f; // Assuming origin is at feet

                if (Mathf.Abs(col.height - expectedLocalHeight) > 0.01f || Mathf.Abs(col.radius - expectedLocalRadius) > 0.01f)
                {
                    col.height = expectedLocalHeight;
                    col.radius = expectedLocalRadius;
                    col.center = new Vector3(0, expectedLocalCenterY, 0);
                    modified = true;
                }
            }

            // 2. NavMeshAgent (World Space)
            UnityEngine.AI.NavMeshAgent agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                if (Mathf.Abs(agent.height - targetWorldHeight) > 0.01f || Mathf.Abs(agent.radius - targetWorldRadius) > 0.01f)
                {
                    agent.height = targetWorldHeight;
                    agent.radius = targetWorldRadius;
                    agent.speed = targetNavSpeed;
                    modified = true;
                }
            }

            // 3. Interaction Triggers (SphereCollider on the NPC)
            // A typical interaction radius in world space is around 2.5 units
            float targetWorldInteractionRadius = 2.5f;
            SphereCollider[] sphereCols = npc.GetComponents<SphereCollider>();
            foreach (var sc in sphereCols)
            {
                if (sc.isTrigger)
                {
                    float maxScale = Mathf.Max(npc.transform.lossyScale.x, npc.transform.lossyScale.y, npc.transform.lossyScale.z);
                    float expectedLocalInteractionRadius = targetWorldInteractionRadius / maxScale;

                    if (Mathf.Abs(sc.radius - expectedLocalInteractionRadius) > 0.01f)
                    {
                        sc.radius = expectedLocalInteractionRadius;
                        sc.center = new Vector3(0, targetWorldHeight / (2f * npc.transform.lossyScale.y), 0);
                        modified = true;
                    }
                }
            }

            if (modified)
            {
                EditorUtility.SetDirty(npc);
                modifications++;
                Debug.Log($"[FixComponents] Normalized colliders and NavMesh for {npc.name}");
            }
        }

        if (modifications > 0)
        {
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            Debug.Log($"<color=green>SUCCESS: Normalized Colliders, NavMeshAgents, and Triggers for {modifications} NPCs!</color>");
        }
        else
        {
            Debug.Log("All NPCs already have perfectly consistent components.");
        }
    }
}

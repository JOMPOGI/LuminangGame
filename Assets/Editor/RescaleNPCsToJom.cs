using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class RescaleNPCsToJom : EditorWindow
{
    [MenuItem("Tools/Luminang/Rescale ALL NPCs to Jom")]
    public static void FixScales()
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

        // Find Jom (PlayerArmature)
        GameObject jom = GameObject.Find("PlayerArmature");
        if (jom == null)
        {
            Debug.LogError("Could not find Jom (PlayerArmature) in the scene.");
            return;
        }

        // Calculate Jom's physical height
        float jomHeight = GetVisualHeight(jom);
        if (jomHeight <= 0)
        {
            // Fallback to collider if no renderers
            CharacterController cc = jom.GetComponent<CharacterController>();
            if (cc != null) jomHeight = cc.height * jom.transform.lossyScale.y;
            else jomHeight = 1.8f; // Reasonable fallback
        }
        
        Debug.Log($"Jom's calculated height is: {jomHeight} units");

        InteractableNPC[] allNPCs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int modifications = 0;

        foreach (var npc in allNPCs)
        {
            string name = npc.name.ToLower();
            
            // Exclude Kalaw and Neneng
            if (name.Contains("kalaw") || name.Contains("neneng"))
            {
                continue;
            }
            
            // Ignore close up models
            if (name.Contains("closeup"))
                continue;

            float npcHeight = GetVisualHeight(npc.gameObject);
            
            if (npcHeight > 0)
            {
                // Calculate scale factor needed to match Jom's height
                float scaleFactor = jomHeight / npcHeight;
                
                // We must apply the scale factor to the current localScale
                npc.transform.localScale *= scaleFactor;
                
                EditorUtility.SetDirty(npc);
                modifications++;
                Debug.Log($"[RescaleNPCsToJom] Rescaled {npc.name} by factor {scaleFactor}. New scale: {npc.transform.localScale}");
            }
            else
            {
                Debug.LogWarning($"Could not determine visual height for {npc.name}, skipping.");
            }
        }

        if (modifications > 0)
        {
            EditorSceneManager.MarkSceneDirty(currentScene);
            EditorSceneManager.SaveScene(currentScene);
            Debug.Log($"<color=green>SUCCESS: Fixed {modifications} NPCs to physically match Jom's height!</color>");
        }
        else
        {
            Debug.LogWarning("No NPCs found to rescale.");
        }
    }

    private static float GetVisualHeight(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0f;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds.size.y;
    }
}

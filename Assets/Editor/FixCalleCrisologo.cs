using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public class FixCalleCrisologo
{
    static FixCalleCrisologo()
    {
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void OnHierarchyChanged()
    {
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.name != "Calle_Crisologo") return;

        // Fix Terrain Collider spam
        TerrainCollider[] terrains = Object.FindObjectsByType<TerrainCollider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in terrains)
        {
            MeshCollider mc = t.GetComponent<MeshCollider>();
            if (mc != null)
            {
                Object.DestroyImmediate(mc);
                Debug.Log($"<color=green>[AutoFix] Removed incompatible MeshCollider from Terrain: {t.name}</color>");
            }
        }

        // Ensure TeachingOverlayPanel is under a Canvas
        TeachingOverlayPanel overlay = Object.FindFirstObjectByType<TeachingOverlayPanel>(FindObjectsInactive.Include);
        if (overlay != null)
        {
            Canvas parentCanvas = overlay.GetComponentInParent<Canvas>();
            if (parentCanvas == null)
            {
                // Find DialogueUIController's Canvas
                DialogueUIController diagUI = Object.FindFirstObjectByType<DialogueUIController>(FindObjectsInactive.Include);
                if (diagUI != null)
                {
                    Canvas targetCanvas = diagUI.GetComponentInParent<Canvas>();
                    if (targetCanvas != null)
                    {
                        overlay.transform.SetParent(targetCanvas.transform, false);
                        Debug.Log($"<color=green>[AutoFix] Moved TeachingOverlayPanel to Canvas: {targetCanvas.name}</color>");
                    }
                }
            }
        }

        // Ensure PopupPanel is under a Canvas
        PopupManager popup = Object.FindFirstObjectByType<PopupManager>(FindObjectsInactive.Include);
        if (popup != null && popup.popupPanel != null)
        {
            Canvas popupCanvas = popup.popupPanel.GetComponentInParent<Canvas>(true);
            if (popupCanvas == null)
            {
                 popup.popupPanel.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                 popup.popupPanel.AddComponent<CanvasScaler>();
                 popup.popupPanel.AddComponent<GraphicRaycaster>();
                 Debug.Log($"<color=green>[AutoFix] Added Canvas to PopupPanel.</color>");
            }
        }

        // Auto-assign Dialogues to NPCs
        InteractableNPC[] npcs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        string[] allGuids = AssetDatabase.FindAssets("t:DialogueNode");
        
        foreach (var npc in npcs)
        {
            // Only assign if they don't have a default dialogue
            if (npc.defaultDialogue == null)
            {
                string rawName = npc.gameObject.name;
                // Remove prefixes/suffixes to get clean name
                string cleanName = rawName.Replace("vendor", "").Replace("Vendor", "")
                                          .Replace("_Rigged", "").Replace("_rigged", "")
                                          .Replace("NPC_", "").Replace("NPC", "").Trim();
                
                // For Apo Lakay
                if (cleanName.Equals("Apo_Lakay", System.StringComparison.OrdinalIgnoreCase)) cleanName = "ApoLakay";

                // Try to find [CleanName]_Intro.asset or [CleanName]_0.asset
                DialogueNode matchedNode = null;
                foreach (string guid in allGuids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    // Prioritize existing level folders (Intro) over generated ones
                    if (path.Contains("/" + cleanName + "_Intro.asset", System.StringComparison.OrdinalIgnoreCase))
                    {
                        matchedNode = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
                        break;
                    }
                    else if (path.Contains("/" + cleanName + "_0.asset", System.StringComparison.OrdinalIgnoreCase))
                    {
                        // Fallback to script-generated dialogues
                        matchedNode = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
                    }
                }

                if (matchedNode != null)
                {
                    npc.defaultDialogue = matchedNode;
                    
                    EditorUtility.SetDirty(npc);
                    Debug.Log($"<color=green>[AutoFix] Auto-assigned dialogue '{matchedNode.name}' to NPC '{rawName}'</color>");
                }
            }
        }

        // Run smartly to guarantee quest lines are ALWAYS working and bulletproof
        AutomateCalleSetup.RunSetup();
    }
}

using UnityEngine;
using UnityEditor;
using System.Linq;

public class AutoFixer : EditorWindow
{
    [MenuItem("Tools/Fix Project Errors")]
    public static void FixErrors()
    {
        int missingScriptsRemoved = 0;
        int meshCollidersRemoved = 0;

        // Find all GameObjects in the active scene
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (GameObject go in allObjects)
        {
            // 1. Remove Missing Scripts
            int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            missingScriptsRemoved += count;

            // 2. Remove MeshCollider from Terrains
            if (go.GetComponent<Terrain>() != null)
            {
                MeshCollider mc = go.GetComponent<MeshCollider>();
                if (mc != null)
                {
                    Undo.DestroyObjectImmediate(mc);
                    meshCollidersRemoved++;
                    Debug.Log($"Removed MeshCollider from Terrain: {go.name}");
                }
            }
        }

        // 3. Fix UI Raycasts
        int raycastsFixed = 0;
        UnityEngine.UI.Graphic[] graphics = Object.FindObjectsByType<UnityEngine.UI.Graphic>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var graphic in graphics)
        {
            // If it's not a Selectable (Button, Toggle, etc) and has no Selectable in parent/itself that needs it
            if (graphic.raycastTarget)
            {
                bool needsRaycast = false;
                if (graphic.GetComponent<UnityEngine.UI.Selectable>() != null) needsRaycast = true;
                if (graphic.GetComponent<UnityEngine.EventSystems.IEventSystemHandler>() != null) needsRaycast = true;
                
                // If the graphic is part of a button, it might be the background or text of the button.
                // We should keep raycast on the Button itself, but often people put raycast on everything.
                // Actually, if it has a Button component, it needs it. If its parent has a button, does the text need it? No, only the graphic receiving the raycast needs it (usually the Button's own Graphic).
                // But to be safe, if there's any Selectable on the same GameObject, keep it.
                if (!needsRaycast)
                {
                    Undo.RecordObject(graphic, "Fix RaycastTarget");
                    graphic.raycastTarget = false;
                    raycastsFixed++;
                    PrefabUtility.RecordPrefabInstancePropertyModifications(graphic);
                }
            }
        }

        Debug.Log($"AutoFixer Complete: Removed {missingScriptsRemoved} missing scripts and {meshCollidersRemoved} invalid MeshColliders. Fixed {raycastsFixed} UI RaycastTargets.");
        EditorUtility.DisplayDialog("Fixes Complete", $"Removed {missingScriptsRemoved} missing scripts.\nRemoved {meshCollidersRemoved} invalid MeshColliders from Terrains.\nFixed {raycastsFixed} UI RaycastTargets.\nIf your buttons are now clickable, the issue was an invisible UI panel blocking clicks!", "OK");
    }
}

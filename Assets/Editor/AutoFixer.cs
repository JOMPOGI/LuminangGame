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

        Debug.Log($"AutoFixer Complete: Removed {missingScriptsRemoved} missing scripts and {meshCollidersRemoved} invalid MeshColliders.");
        EditorUtility.DisplayDialog("Fixes Complete", $"Removed {missingScriptsRemoved} missing scripts.\nRemoved {meshCollidersRemoved} invalid MeshColliders from Terrains.", "OK");
    }
}

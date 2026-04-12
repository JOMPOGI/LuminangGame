#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Editor tool that scans the active scene and automatically adds LOD Groups
/// to any 3D object that has a MeshRenderer but no existing LOD setup.
///
/// Since we don't have multiple mesh LODs, we set up:
///   LOD0 (100% → 5% screen height): Fully rendered
///   Culled  (< 5% screen height)  : Not rendered at all
///
/// This stops Unity from drawing objects that are tiny on screen — a free
/// performance win especially on low-end devices.
///
/// Usage: Unity menu → Tools → Luminang → Setup LOD Groups In Scene
/// </summary>
public static class LODGroupSetupTool
{
    // An object must be smaller than this % of screen height to be culled.
    // 5% is a good default — a 1m object at ~20m distance on a phone.
    private const float CULL_SCREEN_PERCENT = 0.05f;

    [MenuItem("Tools/Luminang/Setup LOD Groups In Scene")]
    public static void SetupLODGroups()
    {
        int added    = 0;
        int skipped  = 0;
        int existing = 0;

        // Collect all root GameObjects first so we handle hierarchy correctly
        var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        var candidates = new List<GameObject>();

        foreach (var root in roots)
        {
            // Walk every GameObject in the hierarchy
            var allInHierarchy = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in allInHierarchy)
            {
                candidates.Add(t.gameObject);
            }
        }

        foreach (var go in candidates)
        {
            // Skip if already has an LOD Group
            if (go.GetComponent<LODGroup>() != null)
            {
                existing++;
                continue;
            }

            // Only care about objects that have a visual mesh
            var mr = go.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                skipped++;
                continue;
            }

            // Skip Canvas/UI objects
            if (go.GetComponentInParent<Canvas>() != null)
            {
                skipped++;
                continue;
            }

            // Skip if a PARENT already has an LOD Group (don't double-up)
            if (go.GetComponentInParent<LODGroup>() != null)
            {
                skipped++;
                continue;
            }

            // ---- Add LOD Group ----
            Undo.RecordObject(go, "Add LOD Group");
            var lodGroup = Undo.AddComponent<LODGroup>(go);

            // One LOD level: the existing renderer renders until CULL_SCREEN_PERCENT
            var lod0 = new LOD(CULL_SCREEN_PERCENT, new Renderer[] { mr });
            lodGroup.SetLODs(new LOD[] { lod0 });
            lodGroup.RecalculateBounds();

            EditorUtility.SetDirty(go);
            added++;
        }

        // Mark the scene dirty so it prompts to save
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log($"[LODGroupSetupTool] Done! " +
                  $"Added: {added} | Already had LOD: {existing} | Skipped (no mesh/UI): {skipped}");

        EditorUtility.DisplayDialog(
            "LOD Group Setup Complete",
            $"✅ LOD Groups Added: {added}\n" +
            $"⏭ Already had LOD:  {existing}\n" +
            $"⏭ Skipped (no mesh): {skipped}\n\n" +
            $"Objects will now be culled (not rendered) when they are smaller than {CULL_SCREEN_PERCENT * 100f}% of screen height.\n\n" +
            $"💾 Don't forget to save the scene!",
            "OK"
        );
    }

    [MenuItem("Tools/Luminang/Remove All Auto-Added LOD Groups")]
    public static void RemoveLODGroups()
    {
        int removed = 0;
        var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var root in roots)
        {
            var lodGroups = root.GetComponentsInChildren<LODGroup>(true);
            foreach (var lg in lodGroups)
            {
                Undo.DestroyObjectImmediate(lg);
                removed++;
            }
        }

        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"[LODGroupSetupTool] Removed {removed} LOD Groups.");
        EditorUtility.DisplayDialog("Removed LOD Groups", $"Removed {removed} LOD Groups from the scene.", "OK");
    }
}
#endif

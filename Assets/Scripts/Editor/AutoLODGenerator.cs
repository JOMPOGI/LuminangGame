using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityMeshSimplifier;
using System.IO;
using UnityEditor.SceneManagement;

public class AutoLODGenerator : EditorWindow
{
    [MenuItem("Tools/Luminang/Generate LODs for Selected (Including Children)")]
    public static void GenerateLODsSelected()
    {
        GameObject[] selections = Selection.gameObjects;
        if (selections.Length == 0)
        {
            EditorUtility.DisplayDialog("Auto LOD", "Please select objects in Hierarchy first.", "OK");
            return;
        }
        ProcessObjects(selections);
    }

    [MenuItem("Tools/Luminang/Remove ALL Generated LODs (Nuclear Cleanup)")]
    public static void RemoveLODsSelected()
    {
        GameObject[] selections = Selection.gameObjects;
        if (selections.Length == 0) return;

        int deletedCount = 0;
        foreach (var sel in selections)
        {
            Transform[] allChildren = sel.GetComponentsInChildren<Transform>(true);
            List<GameObject> toDelete = new List<GameObject>();

            foreach (var child in allChildren)
            {
                if (child == null || child == sel.transform) continue;
                string n = child.name;
                if (n.StartsWith("LOD1") || n.StartsWith("LOD2") || n.StartsWith("LOD3") || n.Contains("LOD_Mesh"))
                    toDelete.Add(child.gameObject);
            }

            foreach (var go in toDelete) { if (go != null) { DestroyImmediate(go); deletedCount++; } }
            LODGroup[] groups = sel.GetComponentsInChildren<LODGroup>(true);
            foreach (var group in groups) DestroyImmediate(group);
        }
        Debug.Log($"[AutoLOD] Cleanup Finished. Deleted {deletedCount} objects.");
        EditorUtility.DisplayDialog("Auto LOD", $"Cleanup complete!", "OK");
    }

    [MenuItem("Tools/Luminang/Generate LODs for EVERYTHING in Scene")]
    public static void GenerateLODsAll()
    {
        if (!EditorUtility.DisplayDialog("Warning", "Generate LODs for everything? This might take a while.", "Yes", "Cancel"))
            return;
        ProcessObjects(null); 
    }

    private static void ProcessObjects(GameObject[] scopes)
    {
        // 1. Identify roots to process
        List<GameObject> roots = new List<GameObject>();
        if (scopes == null)
        {
            // For "Everything", we find all top-level objects in the scene that have renderers
            Renderer[] allSceneRenderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            foreach (var r in allSceneRenderers)
            {
                if (r == null) continue;
                GameObject root = r.transform.root.gameObject;
                if (!roots.Contains(root)) roots.Add(root);
            }
        }
        else
        {
            roots.AddRange(scopes);
        }

        int processedCount = 0;
        if (!AssetDatabase.IsValidFolder("Assets/GeneratedLODs"))
            AssetDatabase.CreateFolder("Assets", "GeneratedLODs");

        foreach (GameObject root in roots)
        {
            if (root == null) continue;
            // Skip already generated LOD children or objects with existing LODGroups we don't want to double-process
            if (root.name.Contains("LOD") || root.GetComponent<LODGroup>() != null) continue;

            Renderer[] childRenderers = root.GetComponentsInChildren<Renderer>(true);
            if (childRenderers.Length == 0) continue;

            // Prepare LOD levels for this group
            List<Renderer> lod0Renderers = new List<Renderer>();
            List<Renderer> lod1Renderers = new List<Renderer>();
            List<Renderer> lod2Renderers = new List<Renderer>();
            List<Renderer> lod3Renderers = new List<Renderer>();

            string rootSafeName = System.Text.RegularExpressions.Regex.Replace(root.name, @"[^a-zA-Z0-9_\-]", "_");
            string rootFolder = "Assets/GeneratedLODs/" + rootSafeName;
            if (!AssetDatabase.IsValidFolder(rootFolder))
                AssetDatabase.CreateFolder("Assets/GeneratedLODs", rootSafeName);

            foreach (Renderer renderer in childRenderers)
            {
                GameObject go = renderer.gameObject;
                // Skip if this renderer is actually a previously generated LOD mesh
                if (go.name.Contains("LOD")) continue;

                Mesh originalMesh = null;
                if (renderer is MeshRenderer)
                {
                    var mf = go.GetComponent<MeshFilter>();
                    if (mf != null) originalMesh = mf.sharedMesh;
                }
                else if (renderer is SkinnedMeshRenderer smr)
                {
                    originalMesh = smr.sharedMesh;
                }

                lod0Renderers.Add(renderer);

                if (originalMesh == null) continue;

                string meshSafeName = System.Text.RegularExpressions.Regex.Replace(go.name, @"[^a-zA-Z0-9_\-]", "_");
                string meshFolder = rootFolder + "/" + meshSafeName;
                if (!AssetDatabase.IsValidFolder(meshFolder))
                    AssetDatabase.CreateFolder(rootFolder, meshSafeName);

                // Generate Simplified Meshes (High-Quality preservation)
                Mesh meshLOD1 = SimplifyAndSave(originalMesh, 0.70f, meshFolder + "/LOD1.asset"); // Med: 70%
                Mesh meshLOD2 = SimplifyAndSave(originalMesh, 0.40f, meshFolder + "/LOD2.asset"); // Low: 40%
                Mesh meshLOD3 = SimplifyAndSave(originalMesh, 0.15f, meshFolder + "/LOD3.asset"); // Very Low: 15%

                GameObject lod1Obj = GetOrCreateChild(go, "LOD1_Mesh");
                GameObject lod2Obj = GetOrCreateChild(go, "LOD2_Mesh");
                GameObject lod3Obj = GetOrCreateChild(go, "LOD3_Mesh");

                SetupLODChild(go, lod1Obj, meshLOD1, renderer.sharedMaterials);
                SetupLODChild(go, lod2Obj, meshLOD2, renderer.sharedMaterials);
                SetupLODChild(go, lod3Obj, meshLOD3, renderer.sharedMaterials);

                lod1Renderers.Add(lod1Obj.GetComponent<Renderer>());
                lod2Renderers.Add(lod2Obj.GetComponent<Renderer>());
                lod3Renderers.Add(lod3Obj.GetComponent<Renderer>());
            }

            if (lod0Renderers.Count > 0)
            {
                LODGroup lodGroup = root.GetComponent<LODGroup>();
                if (lodGroup == null) lodGroup = Undo.AddComponent<LODGroup>(root);

                LOD[] lods = new LOD[4];
                // Drastically lower thresholds for better visibility distance in Unity 6
                lods[0] = new LOD(0.40f, lod0Renderers.ToArray()); 
                lods[1] = new LOD(0.15f, lod1Renderers.ToArray());
                lods[2] = new LOD(0.05f, lod2Renderers.ToArray());
                lods[3] = new LOD(0.01f, lod3Renderers.ToArray());

                lodGroup.SetLODs(lods);
                lodGroup.RecalculateBounds();
                EditorSceneManager.MarkSceneDirty(root.scene);
                processedCount++;
                Debug.Log($"[AutoLOD] Optimized Group: {root.name} with {lod0Renderers.Count} renderers.");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Auto LOD", $"Successfully optimized {processedCount} object groups!", "OK");
    }

    private static Mesh SimplifyAndSave(Mesh sourceMesh, float quality, string assetPath)
    {
        // 1. Safeguard: Don't simplify very low-poly meshes (like walls/grounds) further
        if (sourceMesh.triangles.Length < 600) return sourceMesh;

        var meshSimplifier = new MeshSimplifier();
        
        // 2. High-Quality Preservation Settings (Simplygon-style)
        // Note: Using SimplificationOptions to avoid obsolete warnings
        var options = meshSimplifier.SimplificationOptions;
        options.PreserveBorderEdges = true;
        options.PreserveUVSeamEdges = true;
        options.PreserveUVFoldoverEdges = true;
        meshSimplifier.SimplificationOptions = options;

        // Initialize with bone weight support for characters
        meshSimplifier.Initialize(sourceMesh);
        meshSimplifier.SimplifyMesh(quality);
        
        Mesh destMesh = meshSimplifier.ToMesh();
        
        // Re-assign bindposes from original if they were lost
        if (sourceMesh.bindposes != null && sourceMesh.bindposes.Length > 0)
        {
            destMesh.bindposes = sourceMesh.bindposes;
        }

        AssetDatabase.CreateAsset(destMesh, assetPath);
        return destMesh;
    }

    private static GameObject GetOrCreateChild(GameObject parent, string name)
    {
        Transform t = parent.transform.Find(name);
        if (t != null) return t.gameObject;
        GameObject child = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(child, "Create LOD Mesh");
        child.transform.SetParent(parent.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;
        child.transform.localScale = Vector3.one;
        return child;
    }

    private static void SetupLODChild(GameObject parent, GameObject child, Mesh mesh, Material[] materials)
    {
        var parentRenderer = parent.GetComponent<Renderer>();

        if (parentRenderer is SkinnedMeshRenderer parentSMR)
        {
            var childSMR = child.GetComponent<SkinnedMeshRenderer>();
            if (childSMR == null) childSMR = Undo.AddComponent<SkinnedMeshRenderer>(child);
            
            var oldMF = child.GetComponent<MeshFilter>(); if (oldMF) DestroyImmediate(oldMF);
            var oldMR = child.GetComponent<MeshRenderer>(); if (oldMR) DestroyImmediate(oldMR);

            childSMR.sharedMesh = mesh;
            childSMR.sharedMaterials = materials;
            childSMR.bones = parentSMR.bones;
            childSMR.rootBone = parentSMR.rootBone;
            childSMR.quality = parentSMR.quality;
            childSMR.updateWhenOffscreen = true; // Force true for LOD children to prevent culling issues
            childSMR.skinnedMotionVectors = parentSMR.skinnedMotionVectors;
        }
        else
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf == null) mf = Undo.AddComponent<MeshFilter>(child);
            mf.sharedMesh = mesh;

            MeshRenderer mr = child.GetComponent<MeshRenderer>();
            if (mr == null) mr = Undo.AddComponent<MeshRenderer>(child);
            mr.sharedMaterials = materials;
        }
    }
}

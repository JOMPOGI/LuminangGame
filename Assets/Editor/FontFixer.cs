using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class FontFixer : EditorWindow
{
    // The WRONG font asset GUID (the one Unity assigned when fonts were broken)
    private const string WRONG_FONT_GUID = "8f586378b4e144a9851e7b34d9b748ee";

    // The CORRECT font GUIDs
    private const string CINZEL_BOLD_GUID = "721a421f1884e754aa51d30324c4d1a9";
    private const string CORMORANT_SEMIBOLD_GUID = "3b187979c42084048898b9f75add0def";

    private TMP_FontAsset cinzelBold;
    private TMP_FontAsset cormorantSemiBold;
    private TMP_FontAsset replacementFont;
    private int fixedCount = 0;

    [MenuItem("Tools/Fix Broken Fonts")]
    public static void ShowWindow()
    {
        GetWindow<FontFixer>("Font Fixer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Font Fixer Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "This tool will find all TextMeshPro components using the wrong/fallback font " +
            "(GUID: 8f586378b4e144a9851e7b34d9b748ee) across all prefabs and scenes, " +
            "and replace them with the correct Cinzel-Bold font.",
            MessageType.Info);

        EditorGUILayout.Space();

        replacementFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "Replace with font:", replacementFont, typeof(TMP_FontAsset), false);

        EditorGUILayout.HelpBox(
            "Drag 'Cinzel-Bold SDF' from Assets/Fonts/ into the field above, then click Fix.",
            MessageType.None);

        EditorGUILayout.Space();

        if (GUILayout.Button("Fix All Prefabs & Scenes", GUILayout.Height(40)))
        {
            if (replacementFont == null)
            {
                // Try to auto-load by GUID
                string path = AssetDatabase.GUIDToAssetPath(CINZEL_BOLD_GUID);
                if (!string.IsNullOrEmpty(path))
                    replacementFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            }

            if (replacementFont == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a replacement font first!", "OK");
                return;
            }

            fixedCount = 0;
            FixAllPrefabs();
            FixAllScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Done!", $"Fixed {fixedCount} TextMeshPro components!", "OK");
        }

        if (fixedCount > 0)
        {
            EditorGUILayout.HelpBox($"Last run fixed {fixedCount} components.", MessageType.Info);
        }
    }

    private void FixAllPrefabs()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int total = prefabGuids.Length;
        int i = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            EditorUtility.DisplayProgressBar("Fixing Prefabs...", path, (float)i / total);

            // Check if file contains wrong GUID before loading
            string fileContent = File.ReadAllText(path);
            if (!fileContent.Contains(WRONG_FONT_GUID))
            {
                i++;
                continue;
            }

            GameObject prefab = PrefabUtility.LoadPrefabContents(path);
            bool changed = false;

            var tmps = prefab.GetComponentsInChildren<TMP_Text>(true);
            foreach (var tmp in tmps)
            {
                if (tmp.font != null)
                {
                    string fontGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(tmp.font));
                    if (fontGuid == WRONG_FONT_GUID)
                    {
                        tmp.font = replacementFont;
                        fixedCount++;
                        changed = true;
                    }
                }
            }

            if (changed)
                PrefabUtility.SaveAsPrefabAsset(prefab, path);

            PrefabUtility.UnloadPrefabContents(prefab);
            i++;
        }

        EditorUtility.ClearProgressBar();
    }

    private void FixAllScenes()
    {
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });
        int total = sceneGuids.Length;
        int i = 0;

        // Save current scene first
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        string activeScenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Skip recovery scenes
            if (path.Contains("_Recovery")) { i++; continue; }

            EditorUtility.DisplayProgressBar("Fixing Scenes...", path, (float)i / total);

            // Check if file contains wrong GUID before loading
            string fileContent = File.ReadAllText(path);
            if (!fileContent.Contains(WRONG_FONT_GUID)) { i++; continue; }

            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            bool changed = false;

            var rootObjects = scene.GetRootGameObjects();
            foreach (var root in rootObjects)
            {
                var tmps = root.GetComponentsInChildren<TMP_Text>(true);
                foreach (var tmp in tmps)
                {
                    if (tmp.font != null)
                    {
                        string fontGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(tmp.font));
                        if (fontGuid == WRONG_FONT_GUID)
                        {
                            tmp.font = replacementFont;
                            fixedCount++;
                            changed = true;
                        }
                    }
                }
            }

            if (changed)
                EditorSceneManager.SaveScene(scene);

            if (scene.path != activeScenePath)
                EditorSceneManager.CloseScene(scene, true);

            i++;
        }

        EditorUtility.ClearProgressBar();
    }
}

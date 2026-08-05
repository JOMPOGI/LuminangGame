using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CleanMagellanScene : EditorWindow
{
    [MenuItem("Tools/Clean Magellan's Cross Errors")]
    public static void DoWork()
    {
        string scenePath = "Assets/Scenes/Environments/Magellan's_Cross.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

        var roots = scene.GetRootGameObjects();
        int removedCount = 0;
        
        foreach (var root in roots)
        {
            // 1. Remove all dangling CanvasRenderers without RectTransforms
            var renderers = root.GetComponentsInChildren<CanvasRenderer>(true);
            foreach (var r in renderers)
            {
                if (r.GetComponent<RectTransform>() == null)
                {
                    DestroyImmediate(r);
                    removedCount++;
                }
            }
            
            // 2. Remove all placeholder objects that were created from missing prefabs
            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                if (t != null && t.name.Contains("Placeholder for referenced MonoBehaviour"))
                {
                    DestroyImmediate(t.gameObject);
                    removedCount++;
                }
            }
        }
        
        if (removedCount > 0)
        {
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"<color=green>SUCCESS: Magellan's Cross has been completely cleaned! Removed {removedCount} broken components and prefabs.</color>");
        }
        else
        {
            Debug.Log("<color=green>SUCCESS: Magellan's Cross is already completely clean!</color>");
        }
        
        EditorSceneManager.CloseScene(scene, true);
    }
}

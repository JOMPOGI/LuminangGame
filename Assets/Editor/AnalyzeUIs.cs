using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text;

public class AnalyzeUIs : EditorWindow
{
    [MenuItem("Tools/Analyze UIs")]
    public static void DoWork()
    {
        StringBuilder sb = new StringBuilder();
        
        string[] scenes = new string[] {
            "Assets/Scenes/Environments/Magellan's_Cross.unity",
            "Assets/Scenes/Environments/Calle_Crisologo.unity"
        };
        
        foreach (var scenePath in scenes)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            sb.AppendLine($"--- {scene.name} ---");
            var roots = scene.GetRootGameObjects();
            foreach (var root in roots)
            {
                if (root.name.Contains("UI") || root.GetComponentInChildren<Canvas>(true) != null)
                {
                    sb.AppendLine($"Found UI Root: {root.name}");
                    Canvas canvas = root.GetComponentInChildren<Canvas>(true);
                    if (canvas != null)
                    {
                        foreach (Transform child in canvas.transform)
                        {
                            sb.AppendLine($"  - {child.name} (Active: {child.gameObject.activeSelf})");
                        }
                    }
                }
            }
            EditorSceneManager.CloseScene(scene, true);
        }
        Debug.Log(sb.ToString());
    }
}

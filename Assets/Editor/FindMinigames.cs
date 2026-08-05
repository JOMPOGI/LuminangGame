using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text;

public class FindMinigames : EditorWindow
{
    [MenuItem("Tools/Find Minigames")]
    public static void DoWork()
    {
        string scenePath = "Assets/Scenes/Environments/Magellan's_Cross.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        StringBuilder sb = new StringBuilder();
        
        var roots = scene.GetRootGameObjects();
        bool found = false;
        foreach (var root in roots)
        {
            var allScripts = root.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var s in allScripts)
            {
                if (s != null && s.GetType().Name.ToLower().Contains("minigame"))
                {
                    sb.AppendLine($"Found Minigame script '{s.GetType().Name}' on GameObject '{s.gameObject.name}'");
                    found = true;
                }
            }
        }
        
        if (!found) sb.AppendLine("No minigame scripts found in Magellan's Cross.");
        
        Debug.Log(sb.ToString());
        EditorSceneManager.CloseScene(scene, true);
    }
}

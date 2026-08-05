using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FindUICanvas : EditorWindow
{
    [MenuItem("Tools/Find UI Canvas")]
    public static void DoWork()
    {
        string scenePath = "Assets/Scenes/Environments/Calle_Crisologo.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        
        var roots = scene.GetRootGameObjects();
        foreach (var root in roots)
        {
            if (root.GetComponentInChildren<Canvas>(true) != null)
            {
                Debug.Log($"Found Canvas in root object: {root.name}");
            }
        }
        
        EditorSceneManager.CloseScene(scene, true);
    }
}

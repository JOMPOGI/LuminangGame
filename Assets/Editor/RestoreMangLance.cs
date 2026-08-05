using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class RestoreMangLance : EditorWindow
{
    [MenuItem("Tools/Restore Mang Lance")]
    public static void DoWork()
    {
        string tempPath = "Assets/Temp_Calle_Crisologo.unity";
        Scene tempScene = EditorSceneManager.OpenScene(tempPath, OpenSceneMode.Additive);
        
        var tempRoots = tempScene.GetRootGameObjects();
        GameObject mangLanceToCopy = null;
        
        foreach (var root in tempRoots)
        {
            var npcs = root.GetComponentsInChildren<Transform>(true);
            foreach (var t in npcs)
            {
                if (t.name.Equals("Mang_Lance") || t.name.Equals("Mang_Lance_Rigged"))
                {
                    // Find the outermost root of Mang Lance
                    var rootObj = t;
                    while (rootObj.parent != null && rootObj.parent.name.Contains("Mang"))
                    {
                        rootObj = rootObj.parent;
                    }
                    mangLanceToCopy = rootObj.gameObject;
                    break;
                }
            }
            if (mangLanceToCopy != null) break;
        }

        if (mangLanceToCopy != null)
        {
            var activeScene = SceneManager.GetActiveScene();
            var newInstance = Instantiate(mangLanceToCopy);
            newInstance.name = mangLanceToCopy.name;
            SceneManager.MoveGameObjectToScene(newInstance, activeScene);
            EditorUtility.SetDirty(newInstance);
            Debug.Log($"<color=green>SUCCESS: Restored {newInstance.name} from backup!</color>");
        }
        else
        {
            Debug.LogError("Could not find Mang Lance in the backup scene!");
        }

        EditorSceneManager.CloseScene(tempScene, true);
    }
}

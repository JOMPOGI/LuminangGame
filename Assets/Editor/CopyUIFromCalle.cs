using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class CopyUIFromCalle : EditorWindow
{
    [MenuItem("Tools/Fix Magellan UI")]
    public static void DoWork()
    {
        // 1. Open Calle Crisologo to copy the good UI
        string callePath = "Assets/Scenes/Environments/Calle_Crisologo.unity";
        Scene calleScene = EditorSceneManager.OpenScene(callePath, OpenSceneMode.Additive);
        
        GameObject goodUI = null;
        var calleRoots = calleScene.GetRootGameObjects();
        foreach (var root in calleRoots)
        {
            if (root.name == "UI")
            {
                goodUI = root;
                break;
            }
        }
        
        if (goodUI != null)
        {
            // 2. We are in Magellan's Cross. Let's delete the broken UI and replace it!
            var activeScene = SceneManager.GetActiveScene();
            var activeRoots = activeScene.GetRootGameObjects();
            
            foreach (var root in activeRoots)
            {
                if (root.name == "UI")
                {
                    DestroyImmediate(root);
                    break;
                }
            }
            
            // 3. Instantiate the good UI into Magellan's Cross
            GameObject newUI = Instantiate(goodUI);
            newUI.name = "UI";
            SceneManager.MoveGameObjectToScene(newUI, activeScene);
            EditorUtility.SetDirty(newUI);
            
            Debug.Log("<color=green>SUCCESS: Copied perfectly working UI from Calle Crisologo into Magellan's Cross!</color>");
        }
        else
        {
            Debug.LogError("Could not find the UI object in Calle Crisologo!");
        }
        
        EditorSceneManager.CloseScene(calleScene, true);
    }
}

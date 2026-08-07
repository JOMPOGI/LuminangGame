using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FixCalleCrisologoQuizPrefab : EditorWindow
{
    [MenuItem("Tools/Luminang/Fix Quiz Prefab in Scene")]
    public static void FixPrefab()
    {
        string scenePath = "Assets/Scenes/Environments/Calle_Crisologo.unity";
        Scene currentScene = EditorSceneManager.GetActiveScene();
        
        bool wasOpen = currentScene.path == scenePath;
        if (!wasOpen)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                currentScene = EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                Debug.LogWarning("Aborted because unsaved scenes could not be saved.");
                return;
            }
        }

        ConversationTestManager ctm = Object.FindFirstObjectByType<ConversationTestManager>(FindObjectsInactive.Include);
        if (ctm != null)
        {
            string kalawPrefabPath = "Assets/Prefabs/Mini Games/KalawQuizBubble.prefab";
            GameObject kalawPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(kalawPrefabPath);
            
            if (kalawPrefab != null)
            {
                ctm.tiptipQuizPrefab = kalawPrefab;
                EditorUtility.SetDirty(ctm);
                EditorSceneManager.MarkSceneDirty(currentScene);
                EditorSceneManager.SaveScene(currentScene);
                Debug.Log("<color=green>SUCCESS: Fixed ConversationTestManager to use KalawQuizBubble prefab!</color>");
            }
            else
            {
                Debug.LogError("Could not find KalawQuizBubble at " + kalawPrefabPath);
            }
        }
        else
        {
            Debug.LogError("Could not find ConversationTestManager in Calle_Crisologo scene.");
        }
    }
}

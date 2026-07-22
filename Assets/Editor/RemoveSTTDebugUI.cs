using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
public class RemoveSTTDebugUI : MonoBehaviour
{
    [MenuItem("Luminang/Clean Up/Remove STT Debug UI")]
    public static void CleanUp()
    {
        // 1. Remove the floating STT_UI_Canvas
        GameObject canvas = GameObject.Find("STT_UI_Canvas");
        if (canvas != null)
        {
            DestroyImmediate(canvas);
            Debug.Log("Successfully removed the floating STT_UI_Canvas.");
        }

        // 2. Unload STT_TestScene if it's still open additively
        Scene testScene = SceneManager.GetSceneByName("STT_TestScene");
        if (testScene.isLoaded)
        {
            EditorSceneManager.CloseScene(testScene, true);
            Debug.Log("Successfully closed STT_TestScene from the Hierarchy.");
        }
        
        // Ensure STT_System is still intact
        GameObject sys = GameObject.Find("[STT_System]");
        if (sys == null)
        {
            Debug.LogWarning("Wait! [STT_System] is missing! The Mic button needs this to process the STT logic. Please re-run 'Integrate STT into Calle Crisologo'.");
        }
        else
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            Debug.Log("Calle Crisologo is clean! The STT logic is hidden in the background and will be triggered by your Dialogue Mic Button.");
        }
    }
}
#endif

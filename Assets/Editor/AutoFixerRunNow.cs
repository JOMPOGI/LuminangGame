using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class AutoFixerRunNow
{
    static AutoFixerRunNow()
    {
        EditorApplication.delayCall += RunIt;
    }

    static void RunIt()
    {
        if (!SessionState.GetBool("WipeAllRunOnce", false))
        {
            SessionState.SetBool("WipeAllRunOnce", true);
            Debug.Log("Automatically running Wipe All...");
            WipAllDialogues.Run();
        }
    }
}

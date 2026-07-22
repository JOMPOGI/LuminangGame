using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AutoSetupSTT
{
    static AutoSetupSTT()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Calle_Crisologo")
            {
                Debug.Log("[AutoSetupSTT] Automatically running Setup STT Flow before playing...");
                SetupSTTFlow.SetupFlow();
                AddMicToDialogue.InjectMicButton();
            }
        }
    }
}

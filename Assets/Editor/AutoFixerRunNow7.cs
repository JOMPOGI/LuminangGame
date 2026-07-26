using UnityEngine;
using UnityEditor;

public class AutoFixerRunNow7
{
    [InitializeOnLoadMethod]
    static void RunOnLoad()
    {
        if (SessionState.GetBool("AutoFixerRunNow7_Executed", false))
            return;

        SessionState.SetBool("AutoFixerRunNow7_Executed", true);

        EditorApplication.delayCall += () =>
        {
            Debug.Log("Executing StateDumperFile.Dump()...");
            StateDumperFile.Dump();
        };
    }
}

using UnityEngine;
using UnityEditor;

public class AutoFixerRunNow6
{
    [InitializeOnLoadMethod]
    static void RunOnLoad()
    {
        if (SessionState.GetBool("AutoFixerRunNow6_Executed", false))
            return;

        SessionState.SetBool("AutoFixerRunNow6_Executed", true);

        EditorApplication.delayCall += () =>
        {
            Debug.Log("Executing StateDumper.Dump()...");
            StateDumper.Dump();
        };
    }
}

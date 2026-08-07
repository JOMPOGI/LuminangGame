using UnityEngine;
using UnityEditor;

public class CheckNavMeshAndScale : EditorWindow
{
    [MenuItem("Tools/Check NavMesh And Scale")]
    public static void DoWork()
    {
        var dave = GameObject.Find("Dave") ?? GameObject.Find("Dave_Rigged");
        if (dave != null)
        {
            var agent = dave.GetComponent<UnityEngine.AI.NavMeshAgent>();
            Debug.Log($"Dave has NavMeshAgent: {(agent != null)}");
            Debug.Log($"Dave current scale: {dave.transform.localScale}");
        }
    }
}

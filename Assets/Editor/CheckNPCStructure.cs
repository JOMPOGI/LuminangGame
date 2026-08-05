using UnityEngine;
using UnityEditor;

public class CheckNPCStructure : EditorWindow
{
    [MenuItem("Tools/Check NPC Structure")]
    public static void CheckStruct()
    {
        var lito = GameObject.Find("Lito");
        if (lito != null)
        {
            var animRoot = lito.GetComponent<Animator>();
            var animChild = lito.GetComponentInChildren<Animator>();
            Debug.Log($"Lito Root Animator: {(animRoot != null)} | Child Animator: {(animChild != null && animChild != animRoot)}");
        }
    }
}

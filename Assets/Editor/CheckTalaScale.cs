using UnityEngine;
using UnityEditor;

public class CheckTalaScale : EditorWindow
{
    [MenuItem("Tools/Check Tala Scale")]
    public static void Check()
    {
        var tala = GameObject.Find("Tala") ?? GameObject.Find("Tala_Rigged");
        if (tala != null)
        {
            Debug.Log($"Tala Scale: {tala.transform.localScale}");
        }
        else
        {
            Debug.Log("Tala not found.");
        }
    }
}

using UnityEngine;
using UnityEditor;

public static class CheckKalawAsset
{
    [MenuItem("Tools/Check Kalaw Asset")]
    public static void Check()
    {
        string guid = "5482162456d9cd44db89af116b967a00";
        string path = AssetDatabase.GUIDToAssetPath(guid);
        Debug.Log("Asset path for Kalaw GUID: " + path);
        DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        if (node == null) Debug.Log("Failed to load DialogueNode!");
        else Debug.Log("Successfully loaded: " + node.name + " with text: " + node.dialogueText);
    }
}

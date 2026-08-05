using UnityEngine;
using UnityEditor;

public class ClearKalawTranslations : EditorWindow
{
    [MenuItem("Tools/Clear Kalaw Translations")]
    public static void ClearTranslations()
    {
        string[] guids = AssetDatabase.FindAssets("t:DialogueNode Kalaw_");
        int count = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
            if (node != null && node.speakerName == "Kalaw")
            {
                // Only swap if there is an English translation available
                if (!string.IsNullOrWhiteSpace(node.translatedText))
                {
                    node.dialogueText = node.translatedText;
                    node.translatedText = "";
                    EditorUtility.SetDirty(node);
                    count++;
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"[ClearKalawTranslations] Replaced Ilocano with English for {count} Kalaw dialogue nodes!");
    }
}

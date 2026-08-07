using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class TranslationEntry
{
    public string assetPath;
    public string speakerName;
    [TextArea(2, 5)]
    public string englishText;
    [TextArea(2, 5)]
    public string ilokanoText;
}

[System.Serializable]
public class LocalizationData
{
    public List<TranslationEntry> entries = new List<TranslationEntry>();
}

public class DialogueLocalizer : EditorWindow
{
    private const string FOLDER_PATH = "Assets/Dialogues/CalleCrisologo";
    private const string JSON_PATH = "Assets/Dialogues/CalleCrisologoTranslations.json";

    [MenuItem("Tools/Localization/1. Export Calle Crisologo Dialogues (JSON)")]
    public static void ExportDialogues()
    {
        string[] guids = AssetDatabase.FindAssets("t:DialogueNode", new[] { FOLDER_PATH });
        LocalizationData data = new LocalizationData();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);

            if (node != null)
            {
                // If translatedText is empty, it means we haven't imported yet, so dialogueText is English.
                // If it's NOT empty, it means we already imported, and translatedText holds the English fallback.
                string english = string.IsNullOrEmpty(node.translatedText) ? node.dialogueText : node.translatedText;
                string ilokano = string.IsNullOrEmpty(node.translatedText) ? "" : node.dialogueText;

                data.entries.Add(new TranslationEntry
                {
                    assetPath = path,
                    speakerName = node.speakerName,
                    englishText = english,
                    ilokanoText = ilokano
                });
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(JSON_PATH, json);
        AssetDatabase.Refresh();

        Debug.Log($"<color=green>[Localization] Exported {data.entries.Count} dialogue nodes to {JSON_PATH}</color>");
    }

    [MenuItem("Tools/Localization/2. Import Ilokano Translations (JSON)")]
    public static void ImportDialogues()
    {
        if (!File.Exists(JSON_PATH))
        {
            Debug.LogError($"[Localization] Cannot find {JSON_PATH}. Please Export first and fill in the translations.");
            return;
        }

        string json = File.ReadAllText(JSON_PATH);
        LocalizationData data = JsonUtility.FromJson<LocalizationData>(json);

        if (data == null || data.entries == null)
        {
            Debug.LogError("[Localization] Failed to parse JSON.");
            return;
        }

        int updatedCount = 0;
        foreach (var entry in data.entries)
        {
            // Only update if there is actually a translation provided
            if (!string.IsNullOrWhiteSpace(entry.ilokanoText))
            {
                DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(entry.assetPath);
                if (node != null)
                {
                    // Swap: English goes to translation, Ilokano becomes default
                    node.translatedText = entry.englishText;
                    node.dialogueText = entry.ilokanoText;
                    
                    EditorUtility.SetDirty(node);
                    updatedCount++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"<color=cyan>[Localization] Successfully imported translations and updated {updatedCount} assets.</color>");
    }
}

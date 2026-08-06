using UnityEngine;
using UnityEditor;
using System.IO;

public class DialogueCleaner : EditorWindow
{
    [MenuItem("Tools/Clean Old Calle Crisologo Dialogues")]
    public static void CleanDialogues()
    {
        string folder = "Assets/Dialogues/CalleCrisologo";
        if (Directory.Exists(folder))
        {
            string[] files = Directory.GetFiles(folder, "*.asset");
            foreach(string f in files)
            {
                AssetDatabase.DeleteAsset(f);
            }
            AssetDatabase.Refresh();
            Debug.Log($"[DialogueCleaner] Deleted {files.Length} old assets in CalleCrisologo folder!");
        }
    }
}

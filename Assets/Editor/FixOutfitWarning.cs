using UnityEngine;
using UnityEditor;
using System.IO;

public class FixOutfitWarning : EditorWindow
{
    [MenuItem("Tools/Fix Outfit Warning")]
    public static void DoWork()
    {
        string path = "Assets/Scripts/Player/OutfitInitializer.cs";
        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            if (!content.Contains("// Suppressed warning"))
            {
                content = content.Replace(
                    "Debug.LogWarning(\"[OutfitInitializer] No saved outfit found\"", 
                    "// Suppressed warning Debug.LogWarning(\"[OutfitInitializer] No saved outfit found\""
                );
                // In case it's on one line:
                content = content.Replace(
                    "Debug.LogWarning(\"[OutfitInitializer] No saved outfit found in profile or failed to parse. Check if data was saved correctly in Character Creation.\");", 
                    "// Suppressed warning Debug.LogWarning(\"[OutfitInitializer] No saved outfit found...\");"
                );
                File.WriteAllText(path, content);
            }
        }
        AssetDatabase.Refresh();
        Debug.Log("<color=green>SUCCESS: Suppressed OutfitInitializer warning!</color>");
    }
}

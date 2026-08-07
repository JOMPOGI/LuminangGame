using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Assigns the correct NPC speaker portrait to every DialogueNode in
/// Assets/Dialogues/CalleCrisologo that is currently missing one.
/// NPCs with no portrait image on disk are silently skipped.
/// Run via: Tools > Fix NPC Portraits (Assign Missing)
/// </summary>
public class FixNPCPortraits : EditorWindow
{
    [MenuItem("Tools/Fix NPC Portraits (Assign Missing)")]
    public static void Fix()
    {
        // -----------------------------------------------------------------------
        // Map: key = lowercase partial NPC name  →  value = sprite asset path
        // Add new entries here whenever a new portrait image is added to the project.
        // -----------------------------------------------------------------------
        var portraitPaths = new Dictionary<string, string>
        {
            // Level 1 – Greetings
            { "kalaw",      "Assets/Sprites/NPCs/KalawImage.png" },
            // Level 1 – Greetings / Level 2 (Kyros has no portrait image yet — will skip)
            // Level 1 – Gratitude
            { "irah",       "Assets/Sprites/NPCs/IrahImage.png" },    // may not exist
            { "jom",        "Assets/Sprites/NPCs/JomImage.png" },     // may not exist
            // Level 1 – Identity
            { "ronnie",     "Assets/Sprites/NPCs/RonnieImage.png" },  // may not exist
            { "sally",      "Assets/Sprites/NPCs/Sally.png" },
            // Level 2 – Requests
            { "lito",       "Assets/Sprites/NPCs/LitoImage.png" },
            // Level 2 – Directions
            { "apolakay",   "Assets/Sprites/NPCs/ApoLakayImage.png" },
            { "tomas",      "Assets/Sprites/NPCs/TomasImage.png" },
            { "klara",      "Assets/Sprites/NPCs/KlaraImage.png" },
            // Level 2 – Count
            { "tala",       "Assets/Sprites/NPCs/TalaImage.png" },
            { "manglance",  "Assets/Sprites/NPCs/MangLanceImage.png" },
            // Level 3 – Action Verbs
            { "rayo",       "Assets/Sprites/NPCs/RayoImage.png" },
            { "alingrosa",  "Assets/Sprites/NPCs/AlingRosaImage.png" },
            // Level 3 – Linking Verbs / Pronouns
            { "lolanida",   "Assets/Sprites/NPCs/LolaNidaImage.png" }, // may not exist
            { "neneng",     "Assets/Sprites/NPCs/NenengImage.png" },   // may not exist
            { "alingriza",  "Assets/Sprites/NPCs/AlingRizaImage.png" }, // may not exist
            // Level 3 – Interrogatives
            { "lolabebang", "Assets/Sprites/NPCs/lolaBebang.png" },
        };

        // Pre-load only sprites that actually exist on disk
        var portraitSprites = new Dictionary<string, Sprite>();
        foreach (var kv in portraitPaths)
        {
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(kv.Value);
            if (s != null)
                portraitSprites[kv.Key] = s;
            else
                Debug.LogWarning($"[FixNPCPortraits] No portrait file for '{kv.Key}' at: {kv.Value}  — skipping.");
        }

        // Find every DialogueNode in the CalleCrisologo folder
        string[] guids = AssetDatabase.FindAssets("t:DialogueNode",
            new[] { "Assets/Dialogues/CalleCrisologo" });

        int fixedCount = 0;
        int alreadySet = 0;
        int noMatch    = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
            if (node == null) continue;

            // Skip nodes that already have a portrait
            if (node.speakerPortrait != null) { alreadySet++; continue; }

            // Clean speaker name to match our keys
            string speakerKey = node.speakerName?.Replace(" ", "").Replace("_", "")
                                                  .Replace("Apo", "apo")
                                                  .ToLower() ?? "";

            Sprite matched = null;
            foreach (var kv in portraitSprites)
            {
                // Either the key is a substring of the speaker name, or vice-versa
                if (speakerKey.Contains(kv.Key) || kv.Key.Contains(speakerKey))
                {
                    matched = kv.Value;
                    break;
                }
            }

            if (matched != null)
            {
                node.speakerPortrait = matched;
                EditorUtility.SetDirty(node);
                fixedCount++;
                Debug.Log($"[FixNPCPortraits] ✔ {matched.name}  →  {node.name}  ({node.speakerName})");
            }
            else
            {
                noMatch++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"<color=green>[FixNPCPortraits] Done! " +
                  $"Fixed={fixedCount}  AlreadySet={alreadySet}  NoPortraitAvailable={noMatch}</color>");
    }
}

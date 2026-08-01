using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Patches all Calle Crisologo _Teach dialogue nodes so they:
///   1. Strip the "Now, try saying ___" prompt from dialogueText (panel handles it)
///   2. Set triggerEventName = "ShowTeachingPanel" so the panel auto-shows with the mic
///
/// Also patches Level-completion _Success nodes so the banner is shown on the
/// Teaching Overlay Panel instead of inside the NPC speech bubble.
/// </summary>
public static class PatchCalleDialoguesForTeachingPanel
{
    [MenuItem("Tools/Calle Crisologo/Patch Dialogues for Teaching Panel")]
    public static void PatchAll()
    {
        int patchedTeach = 0;
        int patchedLevel = 0;

        string root = "Assets/Dialogues/CalleCrisologo";
        string[] guids = AssetDatabase.FindAssets("t:DialogueNode", new[] { root });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
            if (node == null) continue;

            string fileName = Path.GetFileNameWithoutExtension(path);
            bool dirty = false;

            // ── PATCH _Teach nodes ────────────────────────────────────────────
            if (fileName.EndsWith("_Teach", System.StringComparison.OrdinalIgnoreCase))
            {
                // Only patch if this node has an STT choice with an expected word
                bool hasSTT = false;
                string sttWord = "";
                if (node.choices != null)
                {
                    foreach (var c in node.choices)
                    {
                        if (c != null && (c.choiceEvent == "StartSTT" || !string.IsNullOrEmpty(c.expectedSTTWord)))
                        {
                            hasSTT = true;
                            sttWord = c.expectedSTTWord;
                            break;
                        }
                    }
                }

                if (hasSTT)
                {
                    // Clear triggerEventName! We DO NOT want the panel to show immediately.
                    // It will show up automatically AT THE END of the text via the StartSTT choice event.
                    if (node.triggerEventName == "ShowTeachingPanel")
                    {
                        node.triggerEventName = "";
                        dirty = true;
                        patchedTeach++;
                    }

                    // The user requested that the NPC ACTUALLY SAYS "Now, try saying '___' yourself." in the dialogue box
                    // before the mic appears. So we must ensure it is present in the text!
                    if (!string.IsNullOrEmpty(node.dialogueText))
                    {
                        // Some original texts had single quotes, some might not. We'll use a generic check.
                        if (!node.dialogueText.Contains("Now, try saying") && !string.IsNullOrEmpty(sttWord))
                        {
                            string expectedLine = $"Now, try saying '{sttWord}' yourself.";
                            
                            // If the text ends with a quote, we want to put our line INSIDE the quote
                            if (node.dialogueText.EndsWith("\""))
                            {
                                node.dialogueText = node.dialogueText.Substring(0, node.dialogueText.Length - 1).TrimEnd() + "\n" + expectedLine + "\"";
                            }
                            else
                            {
                                node.dialogueText = node.dialogueText.TrimEnd() + "\n" + expectedLine;
                            }
                            dirty = true;
                        }
                    }

                    if (dirty) patchedTeach++;
                }
            }

            // ── PATCH Level-completion _Success nodes ─────────────────────────
            // These are the nodes whose endEventName sets "LEVEL X COMPLETE!"
            if (!string.IsNullOrEmpty(node.endEventName) &&
                node.endEventName.Contains("COMPLETE", System.StringComparison.OrdinalIgnoreCase))
            {
                // Move the level banner text into a ShowFloatingText trigger event
                // and simplify the NPC's spoken text to just the congratulation line.
                string rawText = node.dialogueText ?? "";
                
                // Build a clean short NPC line (first sentence only)
                string firstSentence = GetFirstSentence(rawText);
                
                // Build the trigger event that will show the level-complete banner on the Teaching Panel
                string levelBanner = ExtractLevelBanner(rawText);
                
                if (!string.IsNullOrEmpty(levelBanner) && string.IsNullOrEmpty(node.triggerEventName))
                {
                    node.triggerEventName = $"ShowFloatingText:{levelBanner}";
                    node.dialogueText = firstSentence;
                    dirty = true;
                    patchedLevel++;
                }
            }

            if (dirty)
            {
                EditorUtility.SetDirty(node);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[PatchCalleDialogues] Done! Patched {patchedTeach} _Teach nodes and {patchedLevel} level-completion nodes.");
        EditorUtility.DisplayDialog("Patch Complete",
            $"Patched {patchedTeach} teaching nodes and {patchedLevel} level-completion nodes for the Teaching Overlay Panel!", "OK");
    }

    private static string GetFirstSentence(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        // Return content before the first "-->" or "LEVEL" banner marker
        int idx = text.IndexOf("->->", System.StringComparison.OrdinalIgnoreCase);
        if (idx < 0) idx = text.IndexOf("LEVEL", System.StringComparison.OrdinalIgnoreCase);
        if (idx > 0) return text.Substring(0, idx).Trim();
        return text.Trim();
    }

    private static string ExtractLevelBanner(string text)
    {
        // Find the "LEVEL X COMPLETE!" line and return it as the banner text
        int idx = text.IndexOf("LEVEL", System.StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return "";
        int end = text.IndexOf('\n', idx);
        if (end < 0) end = text.Length;
        string banner = text.Substring(idx, end - idx).Trim();
        // Clean up arrow markers
        banner = banner.Replace("->->", "").Replace("-->", "").Trim();
        return banner;
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Replaces the "..." placeholder dialogueText in all NPC Intro nodes
/// with proper introductory text from each NPC.
/// </summary>
public class FixIntroNodes : EditorWindow
{
    [MenuItem("Tools/Fix NPC Intro Nodes (Replace ...)")]
    public static void Fix()
    {
        var intros = new Dictionary<string, string>
        {
            // Level 1 - Greetings
            { "Kyros_Intro",    "Naimbag nga aldaw, traveler! Kalaw told me you were exploring Vigan. I'm Kyros, the souvenir vendor. Let me teach you more greetings!" },
            // Level 1 - Gratitude
            { "Irah_Intro",     "Welcome, traveler! I heard you're learning Ilokano. I'm Irah, the weaver. I'll teach you how to express gratitude in Ilokano!" },
            { "Jom_Intro",      "Ay, a visitor! I'm Jom. Pull up a chair! I'll teach you the proper Ilokano responses when someone thanks you." },
            // Level 1 - Identity
            { "Ronnie_Intro",   "Hoy, traveler! The name's Ronnie. Good thing you stopped by — I'll teach you how to introduce yourself in Ilokano!" },
            { "Sally_Intro",    "Naimbag nga aldaw! I'm Sally! Come, sit with me. I'll teach you more Ilokano phrases for talking about yourself and others." },
            // Level 2 - Requests
            { "Lito_Intro",     "Ay, hello traveler! I'm Lito. I spend my days by the river. Let me teach you how to make polite requests in Ilokano!" },
            // Level 2 - Directions
            { "ApoLakay_Intro", "Naimbag a bigat, anak! I am Apo Lakay, an elder of this street. Come, let me show you how we talk about directions in Ilokano!" },
            { "Tomas_Intro",    "Hey, biyahero! I'm Tomas, the potter. Let me teach you the directional commands we use every day!" },
            { "Klara_Intro",    "Naimbag nga aldaw! I'm Klara! You've come to the right place. Let me teach you the last few direction phrases and we'll start counting too!" },
            // Level 2 - Count
            { "Tala_Intro",     "Naimbag nga aldaw! I'm Tala! My bagnet is freshly cooked today. Come learn how to count in Ilokano with me!" },
            // Level 3 - Action Verbs (Quest 8)
            { "MangLance_Intro","Whoa, young traveler! I'm Mang Lance, the kalesa driver. Let me teach you some Ilokano action verbs while we wait for Barnaby!" },
            { "Rayo_Intro",     "Hoy! I'm Rayo. I work at the stables. Action verbs are the heart of Ilokano — let me show you how to use them!" },
            // Level 3 - Linking Verbs (Quest 9)
            { "LolaNida_Intro", "Anak, come here! I'm Lola Nida. Sit with me and I'll teach you linking verbs — the words that connect your sentences in Ilokano." },
            { "Neneng_Intro",   "Hello! I'm Neneng! Linking verbs make your Ilokano sound so natural. Let me teach you a few more before you see Aling Riza!" },
            // Level 3 - Linking Verbs (Quest 9) - AlingRiza
            { "AlingRiza_Intro","Naimbag nga aldaw! I'm Aling Riza, the seamstress. Let me finish teaching you the linking verbs you'll need every day!" },
            // Level 3 - Interrogatives (Quest 11)
            { "LolaBebang_Intro","Halika, anak! I'm Lola Bebang. Sit, sit! I'll teach you the question words — the most important tools for any curious traveler!" },
        };

        int count = 0;
        foreach (var kv in intros)
        {
            string[] guids = AssetDatabase.FindAssets(kv.Key + " t:DialogueNode",
                new[] { "Assets/Dialogues/CalleCrisologo" });

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // Make sure it's the exact Intro node (not a different node with similar name)
                if (!path.EndsWith(kv.Key + ".asset")) continue;

                DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
                if (node == null) continue;

                if (node.dialogueText == "..." || string.IsNullOrWhiteSpace(node.dialogueText))
                {
                    node.dialogueText = kv.Value;
                    // Also set translatedText to the English version so language switch works
                    if (string.IsNullOrWhiteSpace(node.translatedText))
                        node.translatedText = kv.Value;

                    EditorUtility.SetDirty(node);
                    count++;
                    Debug.Log($"[FixIntroNodes] Fixed: {kv.Key} → {path}");
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"<color=green>[FixIntroNodes] Done! Fixed {count} intro nodes.</color>");
    }
}

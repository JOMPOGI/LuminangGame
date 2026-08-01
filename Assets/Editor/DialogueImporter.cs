using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

// ============================================================
//  DialogueImporter  -  Calle Crisologo structured importer
//
//  What this generates per NPC:
//    NpcName_Intro.asset         triggerEventName = "Talk to [NPC]"
//    NpcName_W##_Teach.asset     STT setup
//    NpcName_W##_Success.asset   endEventName = "SetObjective_..." on key words
//
//  Output structure:
//    Assets/Dialogues/CalleCrisologo/
//      Level1_ConversationalSocial/
//        Quest1_Greetings/  Kalaw/  Kyros/
//        Quest2_Gratitude/  Irah/   Jom/
//        Quest4_Identity/   Ronnie/ Sally/
//      Level2_FunctionalNavigational/
//        Quest5_Requests/   Sally/  Lito/
//        Quest6_Directions/ ApoLakay/ Tomas/ Klara/
//        Quest7_Count/      Tala/ MangLance/
//      Level3_GrammaticalFoundations/
//        Quest8_ActionVerbs/   MangLance/ Rayo/ AlingRosa/
//        Quest9_LinkingVerbs/  LolaNida/ Neneng/ AlingRiza/
//        Quest11_Interrogatives/ LolaBebang/
// ============================================================

public class DialogueImporter : EditorWindow
{
    // ------------------------------------------------------------------ menu
    [MenuItem("Tools/Import Calle Crisologo Dialogues (Structured)")]
    public static void ImportDialogues()
    {
        string filePath   = "Assets/Dialogues/new_calle_crisologo_dialogues_ascii.txt";
        string rootFolder = "Assets/Dialogues/CalleCrisologo";

        if (!File.Exists(filePath))
        {
            Debug.LogError("[DialogueImporter] File not found: " + filePath);
            return;
        }

        EnsureFolder("Assets/Dialogues", "CalleCrisologo");

        string[] lines        = File.ReadAllLines(filePath);
        List<NPCBlock> blocks = ParseBlocks(lines);

        foreach (var block in blocks)
            ProcessNPCBlock(block, rootFolder);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"[DialogueImporter] Done! {blocks.Count} NPC blocks imported into '{rootFolder}'.");
    }

    // ================================================================== Data Model

    class NPCBlock
    {
        public string NpcName;
        public string DisplayName;
        public string LevelFolder;
        public string QuestFolder;
        public string IntroDialogue;
        public List<WordBlock> Words = new List<WordBlock>();
    }

    class WordBlock
    {
        public int    WordNumber;
        public string IlokanoTarget;
        public string TeachDialogue;
        public string SuccessDialogue;
    }

    // ================================================================== NPC Meta Table
    // key = lowercase raw npc name (after stripping role suffix)
    //       with _l2 appended for 2nd appearance of same NPC
    static readonly Dictionary<string, (string lv, string q, string safe, string display)> NpcMeta
        = new Dictionary<string, (string, string, string, string)>
    {
        // Level 1
        { "kalaw",        ("Level1_ConversationalSocial", "Quest1_Greetings",       "Kalaw",     "Kalaw") },
        { "vendor kyros", ("Level1_ConversationalSocial", "Quest1_Greetings",       "Kyros",     "Kyros") },
        { "vendor irah",  ("Level1_ConversationalSocial", "Quest2_Gratitude",       "Irah",      "Irah") },
        { "vendor jom",   ("Level1_ConversationalSocial", "Quest2_Gratitude",       "Jom",       "Jom") },
        { "ronnie",       ("Level1_ConversationalSocial", "Quest4_Identity",        "Ronnie",    "Ronnie") },
        { "sally",        ("Level1_ConversationalSocial", "Quest4_Identity",        "Sally",     "Sally") },
        // Level 2
        { "sally_l2",     ("Level2_FunctionalNavigational", "Quest5_Requests",      "Sally",     "Sally") },
        { "lito",         ("Level2_FunctionalNavigational", "Quest5_Requests",      "Lito",      "Lito") },
        { "apo lakay",    ("Level2_FunctionalNavigational", "Quest6_Directions",    "ApoLakay",  "Apo Lakay") },
        { "tomas",        ("Level2_FunctionalNavigational", "Quest6_Directions",    "Tomas",     "Tomas") },
        { "klara",        ("Level2_FunctionalNavigational", "Quest6_Directions",    "Klara",     "Klara") },
        { "tala",         ("Level2_FunctionalNavigational", "Quest7_Count",         "Tala",      "Tala") },
        { "mang lance",   ("Level2_FunctionalNavigational", "Quest7_Count",         "MangLance", "Mang Lance") },
        // Level 3
        { "mang lance_l2",("Level3_GrammaticalFoundations","Quest8_ActionVerbs",   "MangLance", "Mang Lance") },
        { "rayo",         ("Level3_GrammaticalFoundations","Quest8_ActionVerbs",    "Rayo",      "Rayo") },
        { "aling rosa",   ("Level3_GrammaticalFoundations","Quest8_ActionVerbs",    "AlingRosa", "Aling Rosa") },
        { "lola nida",    ("Level3_GrammaticalFoundations","Quest9_LinkingVerbs",   "LolaNida",  "Lola Nida") },
        { "neneng",       ("Level3_GrammaticalFoundations","Quest9_LinkingVerbs",   "Neneng",    "Neneng") },
        { "aling riza",   ("Level3_GrammaticalFoundations","Quest9_LinkingVerbs",   "AlingRiza", "Aling Riza") },
        { "lola bebang",  ("Level3_GrammaticalFoundations","Quest11_Interrogatives","LolaBebang","Lola Bebang") },
    };

    // ================================================================== Objective Wiring

    // triggerEventName on each NPC's Intro node  (key = "SafeName|QuestFolder")
    // Fires the moment the player opens that NPC's dialogue — updates the HUD objective.
    static readonly Dictionary<string, string> IntroTriggers
        = new Dictionary<string, string>
    {
        { "Kalaw|Quest1_Greetings",           "SetObjective_Learn greetings with Kalaw" },
        { "Kyros|Quest1_Greetings",           "SetObjective_Learn greetings from Kyros" },
        { "Irah|Quest2_Gratitude",            "SetObjective_Learn gratitude from Irah" },
        { "Jom|Quest2_Gratitude",             "SetObjective_Learn gratitude and responses from Jom" },
        { "Ronnie|Quest4_Identity",           "SetObjective_Learn identity phrases from Ronnie" },
        { "Sally|Quest4_Identity",            "SetObjective_Learn identity phrases from Sally" },
        { "Sally|Quest5_Requests",            "SetObjective_Learn requests from Sally" },
        { "Lito|Quest5_Requests",             "SetObjective_Learn requests from Lito" },
        { "ApoLakay|Quest6_Directions",       "SetObjective_Learn directions from Apo Lakay" },
        { "Tomas|Quest6_Directions",          "SetObjective_Learn directions from Tomas" },
        { "Klara|Quest6_Directions",          "SetObjective_Learn directions and numbers from Klara" },
        { "Tala|Quest7_Count",                "SetObjective_Learn counting from Tala" },
        { "MangLance|Quest7_Count",           "SetObjective_Learn counting from Mang Lance" },
        { "MangLance|Quest8_ActionVerbs",     "SetObjective_Learn action verbs from Mang Lance" },
        { "Rayo|Quest8_ActionVerbs",          "SetObjective_Learn action verbs from Rayo" },
        { "AlingRosa|Quest8_ActionVerbs",     "SetObjective_Learn action verbs from Aling Rosa" },
        { "LolaNida|Quest9_LinkingVerbs",     "SetObjective_Learn linking verbs from Lola Nida" },
        { "Neneng|Quest9_LinkingVerbs",       "SetObjective_Learn linking verbs from Neneng" },
        { "AlingRiza|Quest9_LinkingVerbs",    "SetObjective_Learn pronouns from Aling Riza" },
        { "LolaBebang|Quest11_Interrogatives","SetObjective_Learn question words from Lola Bebang" },
    };

    // endEventName on a specific word's Success node  (key = global word number)
    // Fires after the player successfully says the word — tells them who to find next.
    static readonly Dictionary<int, string> WordEndEvents
        = new Dictionary<int, string>
    {
        // Quest 1: Greetings ------------------------------------------------
        {  4, "SetObjective_Find Kyros" },
        {  8, "SetObjective_Find Irah" },

        // Quest 2: Gratitude + Quest 3: Responses (both Jom) ----------------
        { 12, "SetObjective_Find Jom" },
        { 13, "SetObjective_Learn responses from Jom" },        // stays with Jom
        { 18, "SetObjective_Find Ronnie" },

        // Quest 4: Identity --------------------------------------------------
        { 20, "SetObjective_Find Sally" },
        { 22, "SetObjective_LEVEL I COMPLETE! Head to Level II" },

        // Quest 5: Requests --------------------------------------------------
        { 24, "SetObjective_Find Lito" },
        { 27, "SetObjective_Learn directions from Lito" },      // stays with Lito
        { 28, "SetObjective_Find Apo Lakay" },

        // Quest 6: Directions ------------------------------------------------
        { 32, "SetObjective_Find Tomas" },
        { 36, "SetObjective_Find Klara" },
        { 37, "SetObjective_Learn counting with Klara" },       // stays with Klara
        { 40, "SetObjective_Find Tala" },

        // Quest 7: Count -----------------------------------------------------
        { 44, "SetObjective_Find Mang Lance" },
        { 47, "SetObjective_LEVEL II COMPLETE! Head to Level III" },

        // Quest 8: Action Verbs ----------------------------------------------
        { 48, "SetObjective_Find Rayo" },
        { 52, "SetObjective_Find Aling Rosa" },
        { 55, "SetObjective_Learn linking verbs with Aling Rosa" }, // stays with AlingRosa
        { 56, "SetObjective_Find Lola Nida" },

        // Quest 9: Linking Verbs ---------------------------------------------
        { 60, "SetObjective_Find Neneng" },
        { 64, "SetObjective_Find Aling Riza" },
        { 65, "SetObjective_Learn pronouns with Aling Riza" },  // stays with AlingRiza

        // Quest 10: Pronouns (AlingRiza continues) ---------------------------
        { 74, "SetObjective_Find Lola Bebang" },

        // Quest 11: Interrogatives -------------------------------------------
        { 81, "SetObjective_Find Kalaw" },
    };

    // ================================================================== Parser

    static List<NPCBlock> ParseBlocks(string[] lines)
    {
        var blocks         = new List<NPCBlock>();
        NPCBlock cur       = null;
        WordBlock curWord  = null;
        string mode        = "";   // "intro" | "teach" | "success"
        bool inQuote       = false;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (!inQuote)
            {
                // ---- NPC header: "1. KALAW - GUIDE: BASICS & GREETINGS"
                var npcM = Regex.Match(line, @"^\d+\.\s+(.+?)\s*-\s*");
                if (npcM.Success)
                {
                    string rawName   = npcM.Groups[1].Value.Trim().ToLower();
                    string lookupKey = ResolveKey(rawName, blocks);

                    if (NpcMeta.TryGetValue(lookupKey, out var meta))
                    {
                        cur             = new NPCBlock();
                        cur.NpcName     = meta.safe;
                        cur.DisplayName = meta.display;
                        cur.LevelFolder = meta.lv;
                        cur.QuestFolder = meta.q;
                        blocks.Add(cur);
                        curWord = null;
                        mode    = "intro";
                    }
                    else
                    {
                        Debug.LogWarning($"[DialogueImporter] Unrecognized NPC '{lookupKey}' at line {i + 1}");
                    }
                    continue;
                }

                // ---- Word header: "Word 5: GOOD AFTERNOON -> NAIMBAG A MALEM"
                var wordM = Regex.Match(line, @"^Word\s+(\d+)[.:]\s+.+?->\s*(.+)$");
                if (wordM.Success && cur != null)
                {
                    curWord               = new WordBlock();
                    curWord.WordNumber    = int.TryParse(wordM.Groups[1].Value, out int wn) ? wn : cur.Words.Count + 1;
                    curWord.IlokanoTarget = wordM.Groups[2].Value.Trim().ToLower();
                    cur.Words.Add(curWord);
                    mode = "";
                    continue;
                }

                // ---- Mode switch lines
                if (cur != null)
                {
                    if (line.Contains("Initial Dialogue") || line.Contains("Post-Quest") || line.Contains("Intro Script"))
                    { mode = "intro"; continue; }

                    if (curWord != null && line.Contains("- Teach:"))
                    { mode = "teach"; continue; }

                    if (curWord != null && line.Contains("- Success:"))
                    { mode = "success"; continue; }

                    if (line.StartsWith("Player") || line.StartsWith("Pre-requisite") ||
                        line.StartsWith("Approach")|| line.StartsWith("Quest") ||
                        line.StartsWith("QUEST")   || line.StartsWith("LEVEL") ||
                        line.StartsWith("Words "))
                        continue;
                }
            }

            // ---- Accumulate quoted text
            if (cur != null)
            {
                if (!inQuote && line.StartsWith("\""))
                    inQuote = true;

                if (inQuote)
                {
                    string text = line.Replace("\"", "").Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (mode == "intro")
                            cur.IntroDialogue = Append(cur.IntroDialogue, text);
                        else if (mode == "teach" && curWord != null)
                            curWord.TeachDialogue = Append(curWord.TeachDialogue, text);
                        else if (mode == "success" && curWord != null)
                            curWord.SuccessDialogue = Append(curWord.SuccessDialogue, text);
                    }

                    if (line.EndsWith("\""))
                        inQuote = false;
                }
            }
        }

        return blocks;
    }

    // Handles repeat NPC appearances (Sally x2, MangLance x2) by appending _l2 suffix
    static string ResolveKey(string rawName, List<NPCBlock> existing)
    {
        string key = rawName
            .Replace("guide: basics & greetings", "").TrimEnd()
            .Replace("souvenir vendor", "").TrimEnd()
            .Replace("inabel weaver", "").TrimEnd()
            .Replace("empanada vendor", "").TrimEnd()
            .Replace("local resident", "").TrimEnd()
            .Replace("tour guide", "").TrimEnd()
            .Replace("community elder", "").TrimEnd()
            .Replace("pottery maker", "").TrimEnd()
            .Replace("antique shop owner", "").TrimEnd()
            .Replace("bagnet seller", "").TrimEnd()
            .Replace("kutsero / kalesa driver", "").TrimEnd()
            .Replace("photographer", "").TrimEnd()
            .Replace("souvenir auntie", "").TrimEnd()
            .Replace("elderly resident", "").TrimEnd()
            .Replace("student", "").TrimEnd()
            .Replace("restaurant owner", "").TrimEnd()
            .TrimEnd('-', ' ');

        string keyCompact = key.Replace(" ", "");
        int count = 0;
        foreach (var b in existing)
        {
            if (b.NpcName.ToLower().Replace(" ", "") == keyCompact) count++;
        }

        if (count == 1) key = key + "_l2";
        else if (count >= 2) key = key + "_l3";

        return key;
    }

    static string Append(string existing, string newText)
        => string.IsNullOrEmpty(existing) ? newText : existing + "\n" + newText;

    // ================================================================== Asset Builder

    static void ProcessNPCBlock(NPCBlock block, string rootFolder)
    {
        string levelPath = EnsureFolder(rootFolder,  block.LevelFolder);
        string questPath = EnsureFolder(levelPath,   block.QuestFolder);
        string npcPath   = EnsureFolder(questPath,   block.NpcName);

        // ── Intro node ──────────────────────────────────────────────────
        var introNode          = GetOrCreateNode($"{npcPath}/{block.NpcName}_Intro.asset");
        introNode.speakerName  = block.DisplayName;
        introNode.dialogueText = string.IsNullOrEmpty(block.IntroDialogue) ? "..." : block.IntroDialogue;

        // Wire intro trigger so HUD objective updates when player talks to this NPC
        string introKey = $"{block.NpcName}|{block.QuestFolder}";
        if (IntroTriggers.TryGetValue(introKey, out string introTrigger))
            introNode.triggerEventName = introTrigger;

        EditorUtility.SetDirty(introNode);

        DialogueNode prevNode = introNode;

        // ── Word nodes ──────────────────────────────────────────────────
        foreach (var word in block.Words)
        {
            string tag = $"W{word.WordNumber:D2}";

            // Teach node
            var teachNode          = GetOrCreateNode($"{npcPath}/{block.NpcName}_{tag}_Teach.asset");
            teachNode.speakerName  = block.DisplayName;
            teachNode.dialogueText = string.IsNullOrEmpty(word.TeachDialogue)
                ? "(teach dialogue missing)" : word.TeachDialogue;

            // Success node
            var successNode          = GetOrCreateNode($"{npcPath}/{block.NpcName}_{tag}_Success.asset");
            successNode.speakerName  = block.DisplayName;
            successNode.dialogueText = string.IsNullOrEmpty(word.SuccessDialogue)
                ? "(success dialogue missing)" : word.SuccessDialogue;

            // Wire endEventName → objective redirect on key transition words
            if (WordEndEvents.TryGetValue(word.WordNumber, out string endEvt))
                successNode.endEventName = endEvt;

            // Chain: previous → Teach (Continue button)
            prevNode.choices = prevNode.choices ?? new List<DialogueChoice>();
            prevNode.choices.Clear();
            prevNode.choices.Add(new DialogueChoice { nextNode = teachNode });

            // Chain: Teach → Success (STT)
            teachNode.choices = teachNode.choices ?? new List<DialogueChoice>();
            teachNode.choices.Clear();
            teachNode.choices.Add(new DialogueChoice
            {
                choiceEvent     = "StartSTT",
                expectedSTTWord = word.IlokanoTarget,
                nextNode        = successNode
            });

            EditorUtility.SetDirty(teachNode);
            EditorUtility.SetDirty(successNode);

            prevNode = successNode;
        }

        // Last node — no outgoing choices (end of NPC session)
        prevNode.choices = prevNode.choices ?? new List<DialogueChoice>();
        prevNode.choices.Clear();
        EditorUtility.SetDirty(prevNode);

        Debug.Log($"[DialogueImporter] {block.NpcName} | {block.QuestFolder} | {block.Words.Count} words → {npcPath}");
    }

    // ================================================================== Helpers

    static string EnsureFolder(string parent, string child)
    {
        string full = $"{parent}/{child}";
        if (!AssetDatabase.IsValidFolder(full))
            AssetDatabase.CreateFolder(parent, child);
        return full;
    }

    static DialogueNode GetOrCreateNode(string path)
    {
        var node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
        if (node == null)
        {
            node = ScriptableObject.CreateInstance<DialogueNode>();
            AssetDatabase.CreateAsset(node, path);
        }
        node.choices = node.choices ?? new List<DialogueChoice>();
        return node;
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Editor tool: Tools > Setup Calle Crisologo NPCs
///
/// Wires every InteractableNPC in the scene to their correct _Intro dialogue
/// asset using the structured folder layout:
///   Assets/Dialogues/CalleCrisologo/<Level>/<Quest>/<NPCName>/<NPCName>_Intro.asset
///
/// NPC Walk Order (all Levels 1-3, all 9 quests):
///  L1 Q1 Greetings     : Kalaw > Kyros
///  L1 Q2 Gratitude     : Irah  > Jom
///  L1 Q4 Identity      : Ronnie > Sally
///  L2 Q5 Requests      : Sally (level transition) > Lito
///  L2 Q6 Directions    : Tomas > ApoLakay > Klara
///  L2 Q7 Count         : MangLance > Tala
///  L3 Q8 ActionVerbs   : AlingRosa > MangLance (level transition) > Rayo
///  L3 Q9 LinkingVerbs  : LolaNida > AlingRiza > Neneng
///  L3 Q11 Interrogat.  : LolaBebang
/// </summary>
public class AutomateCalleSetup
{
    // -------------------------------------------------------------------------
    // NPC Table - each row: (goNameContains, requiredObjective, assetName, questFolder)
    // When two entries share the same goNameContains, both will be added to the
    // same NPC's questDialogues list (one per required-objective).
    // -------------------------------------------------------------------------
    private struct NPCEntry
    {
        public string GoContains;     // fragment matched (normalised) against GO name
        public string Objective;      // ObjectiveManager text that enables this NPC
        public string AssetName;      // asset/folder name inside CalleCrisologo/
        public string QuestFolder;    // disambiguates duplicate NPC names across quests

        public NPCEntry(string g, string o, string a, string q)
        {
            GoContains = g; Objective = o; AssetName = a; QuestFolder = q;
        }
    }

    private static readonly List<NPCEntry> TABLE = new List<NPCEntry>
    {
        // ── LEVEL 1 ───────────────────────────────────────────────────────────
        new NPCEntry("Kalaw",     "Talk to Kalaw",                        "Kalaw",     "Quest1_Greetings"),
        new NPCEntry("Kyros",     "Find Kyros",                           "Kyros",     "Quest1_Greetings"),

        new NPCEntry("Irah",      "Find Irah",                            "Irah",      "Quest2_Gratitude"),
        new NPCEntry("Jom",       "Find Jom",                             "Jom",       "Quest2_Gratitude"),

        new NPCEntry("Ronnie",    "Find Ronnie",                          "Ronnie",    "Quest4_Identity"),
        new NPCEntry("Sally",     "Find Sally",                           "Sally",     "Quest4_Identity"),

        // ── LEVEL 2 ───────────────────────────────────────────────────────────
        // Sally also triggers the Level-I-complete cutscene (Quest5 version)
        new NPCEntry("Sally",     "LEVEL I COMPLETE! Head to Level II",   "Sally",     "Quest5_Requests"),
        new NPCEntry("Lito",      "Find Lito",                            "Lito",      "Quest5_Requests"),

        new NPCEntry("Tomas",     "Find Tomas",                           "Tomas",     "Quest6_Directions"),
        new NPCEntry("ApoLakay",  "Find Apo Lakay",                       "ApoLakay",  "Quest6_Directions"),
        new NPCEntry("Klara",     "Find Klara",                           "Klara",     "Quest6_Directions"),

        new NPCEntry("MangLance", "Find Mang Lance",                      "MangLance", "Quest7_Count"),
        new NPCEntry("Tala",      "Find Tala",                            "Tala",      "Quest7_Count"),

        // ── LEVEL 3 ───────────────────────────────────────────────────────────
        new NPCEntry("AlingRosa", "Find Aling Rosa",                      "AlingRosa", "Quest8_ActionVerbs"),
        // MangLance also triggers the Level-II-complete cutscene (Quest8 version)
        new NPCEntry("MangLance", "LEVEL II COMPLETE! Head to Level III", "MangLance", "Quest8_ActionVerbs"),
        new NPCEntry("Rayo",      "Find Rayo",                            "Rayo",      "Quest8_ActionVerbs"),

        new NPCEntry("LolaNida",  "Find Lola Nida",                       "LolaNida",  "Quest9_LinkingVerbs"),
        new NPCEntry("AlingRiza", "Find Aling Riza",                      "AlingRiza", "Quest9_LinkingVerbs"),
        new NPCEntry("Neneng",    "Find Neneng",                          "Neneng",    "Quest9_LinkingVerbs"),

        new NPCEntry("LolaBebang","Find Lola Bebang",                     "LolaBebang","Quest11_Interrogatives"),

        // ✨ FINAL CHALLENGE / OUTRO ✨
        new NPCEntry("Kalaw",     "LEVEL III COMPLETE! Head to Plaza: Talk to Kalaw", "Kalaw", "Quest12_Finale"),
    };

    // =========================================================================

    [MenuItem("Tools/Setup Calle Crisologo NPCs")]
    public static void RunSetup()
    {
        var interactables = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int assignedCount = 0;
        int npcsTouched   = 0;

        foreach (var npc in interactables)
        {
            string rawName   = npc.gameObject.name;
            string cleanGoName = Normalize(rawName);

            // Build the expected quest dialogues list
            List<InteractableNPC.QuestDialogue> expectedDialogues = new List<InteractableNPC.QuestDialogue>();

            foreach (var entry in TABLE)
            {
                if (!cleanGoName.Contains(Normalize(entry.GoContains))) continue;

                // Find _Intro asset restricted to the matching quest folder
                string[] guids = AssetDatabase.FindAssets(
                    entry.AssetName + "_Intro t:DialogueNode",
                    new[] { "Assets/Dialogues/CalleCrisologo" });

                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!path.Contains(entry.QuestFolder)) continue;   // wrong quest
                    if (!path.EndsWith(entry.AssetName + "_Intro.asset", System.StringComparison.OrdinalIgnoreCase)) continue; // strict exact match

                    var node = AssetDatabase.LoadAssetAtPath<DialogueNode>(path);
                    if (node == null) continue;

                    expectedDialogues.Add(new InteractableNPC.QuestDialogue
                    {
                        requiredObjective = entry.Objective,
                        dialogueNode      = node
                    });
                }
            }

            // Compare expected with actual
            bool needsUpdate = false;
            if (npc.questDialogues == null || npc.questDialogues.Count != expectedDialogues.Count)
            {
                needsUpdate = true;
            }
            else
            {
                for (int i = 0; i < expectedDialogues.Count; i++)
                {
                    if (npc.questDialogues[i].requiredObjective != expectedDialogues[i].requiredObjective ||
                        npc.questDialogues[i].dialogueNode != expectedDialogues[i].dialogueNode)
                    {
                        needsUpdate = true;
                        break;
                    }
                }
            }

            if (needsUpdate && expectedDialogues.Count > 0)
            {
                Undo.RecordObject(npc, "Assign Quest Dialogue");
                npc.questDialogues = new List<InteractableNPC.QuestDialogue>(expectedDialogues);
                EditorUtility.SetDirty(npc);
                assignedCount += expectedDialogues.Count;
                npcsTouched++;
                Debug.Log($"<color=green>[Setup]</color> Updated {npc.name} with {expectedDialogues.Count} quest dialogues.");
            }
            else if (expectedDialogues.Count == 0)
            {
                //Debug.Log($"<color=grey>[Setup] No Intro dialogue found for: {rawName}</color>");
            }
        }

        if (npcsTouched > 0)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
        }

        if (npcsTouched > 0)
        {
            Debug.Log($"<color=cyan><b>[Setup Complete]</b></color> " +
                      $"Touched {npcsTouched} NPCs, assigned {assignedCount} entries. " +
                      $"<b>Please save the scene!</b>");
        }
    }

    // Strip whitespace, underscores, dashes, and common GO suffixes for matching
    private static string Normalize(string s)
    {
        if (s == null) return "";
        return s.Replace(" ","").Replace("_","").Replace("-","")
                .ToLowerInvariant()
                .Replace("rigged","").Replace("vendor","")
                .Replace("npc","").Replace("position2","")
                .Replace("closeup","").Replace("position","");
    }
}

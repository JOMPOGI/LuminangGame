using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class DumpSallyConfig
{
    static DumpSallyConfig()
    {
        EditorApplication.delayCall += Dump;
    }

    [MenuItem("Tools/Dump Sally Config")]
    public static void Dump()
    {
        string logPath = "SallyDump.txt";
        using (StreamWriter writer = new StreamWriter(logPath, false))
        {
            InteractableNPC[] npcs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var npc in npcs)
            {
                if (npc.gameObject.name.Contains("Sally", System.StringComparison.OrdinalIgnoreCase))
                {
                    writer.WriteLine($"--- SALLY NPC FOUND: {npc.gameObject.name} ---");
                    writer.WriteLine($"Default Dialogue: {(npc.defaultDialogue != null ? npc.defaultDialogue.name : "NULL")}");
                    if (npc.questDialogues != null)
                    {
                        for (int i = 0; i < npc.questDialogues.Count; i++)
                        {
                            var qd = npc.questDialogues[i];
                            string dName = qd.dialogueNode != null ? qd.dialogueNode.name : "NULL";
                            writer.WriteLine($"QuestDialogue [{i}]: RequiredObj='{qd.requiredObjective}', Dialogue='{dName}'");
                        }
                    }
                    else
                    {
                        writer.WriteLine("QuestDialogues array is NULL");
                    }
                }
            }
        }
        Debug.Log("Dumped Sally Config to SallyDump.txt");
    }
}

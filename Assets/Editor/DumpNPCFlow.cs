using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class DumpNPCFlow : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Luminang/Debug Dump NPC Flow")]
    public static void Dump()
    {
        var npcs = FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        string output = "";
        foreach (var npc in npcs)
        {
            output += "NPC: " + npc.gameObject.name + "\n";
            output += "  Enabled: " + npc.interactionEnabled + "\n";
            output += "  Default: " + (npc.defaultDialogue != null ? npc.defaultDialogue.name : "NULL") + "\n";
            output += "  QuestDialogues: " + npc.questDialogues.Count + "\n";
            foreach (var qd in npc.questDialogues)
            {
                output += "    - " + qd.requiredObjective + " -> " + (qd.dialogueNode != null ? qd.dialogueNode.name : "NULL") + "\n";
            }
            output += "---\n";
        }
        File.WriteAllText("npc_dump.txt", output);
        Debug.Log("Dumped to npc_dump.txt");
    }
#endif
}

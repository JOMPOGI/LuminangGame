using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Dumps all InteractableNPC GameObjects in the Calle_Crisologo scene
/// and reports which ones have no dialogue assigned.
/// Run via: Tools > Audit NPC Dialogues
/// </summary>
public class AuditNPCDialogues : EditorWindow
{
    [MenuItem("Tools/Audit NPC Dialogues (Calle Crisologo)")]
    public static void Audit()
    {
        var allNPCs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        var withDialogue    = new List<string>();
        var withoutDialogue = new List<string>();

        foreach (var npc in allNPCs)
        {
            bool hasDefault = npc.defaultDialogue != null;
            bool hasQuest   = npc.questDialogues != null && npc.questDialogues.Count > 0 &&
                              npc.questDialogues.Any(q => q.dialogueNode != null);

            string label = $"{npc.gameObject.name}  [enabled={npc.interactionEnabled}]";

            if (hasDefault || hasQuest)
                withDialogue.Add(label);
            else
                withoutDialogue.Add(label);
        }

        withDialogue.Sort();
        withoutDialogue.Sort();

        Debug.Log($"<color=green>=== NPCs WITH Dialogue ({withDialogue.Count}) ===</color>\n" +
                  string.Join("\n", withDialogue));

        Debug.Log($"<color=red>=== NPCs WITHOUT Dialogue ({withoutDialogue.Count}) ===</color>\n" +
                  string.Join("\n", withoutDialogue));

        Debug.Log($"<color=cyan>TOTAL InteractableNPC in scene: {allNPCs.Length}</color>");
    }
}

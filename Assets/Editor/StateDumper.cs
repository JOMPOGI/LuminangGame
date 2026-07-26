using UnityEngine;
using UnityEditor;
using System.IO;

public class StateDumper
{
    [MenuItem("Luminang/Debug/Dump Kalaw State")]
    public static void Dump()
    {
        GameObject go = GameObject.Find("RiggedKalaw") ?? GameObject.Find("Kalaw");
        if (go == null)
        {
            Debug.LogError("RiggedKalaw not found in scene!");
            return;
        }

        InteractableNPC npc = go.GetComponent<InteractableNPC>();
        if (npc == null)
        {
            Debug.LogError("RiggedKalaw has no InteractableNPC!");
            return;
        }

        string log = $"Kalaw State Dump:\n";
        log += $"interactionEnabled: {npc.interactionEnabled}\n";
        log += $"defaultDialogue: {(npc.defaultDialogue == null ? "NULL" : npc.defaultDialogue.name)}\n";
        log += $"questDialogues count: {npc.questDialogues.Count}\n";
        foreach (var q in npc.questDialogues)
        {
            log += $"  - Obj: {q.requiredObjective}, Node: {(q.dialogueNode == null ? "NULL" : q.dialogueNode.name)}\n";
        }

        Debug.Log(log);
    }
}

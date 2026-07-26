using UnityEngine;
using UnityEditor;
using System.IO;

public class StateDumperFile
{
    [MenuItem("Luminang/Debug/Dump Kalaw State To File")]
    public static void Dump()
    {
        string log = "DUMP START\n";
        GameObject[] gos = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (GameObject go in gos)
        {
            if (go.name.Contains("Kalaw"))
            {
                InteractableNPC npc = go.GetComponent<InteractableNPC>();
                if (npc != null)
                {
                    log += $"Kalaw GameObject: {go.name}\n";
                    log += $"interactionEnabled: {npc.interactionEnabled}\n";
                    log += $"defaultDialogue: {(npc.defaultDialogue == null ? "NULL" : npc.defaultDialogue.name)}\n";
                    log += $"questDialogues count: {npc.questDialogues.Count}\n";
                    foreach (var q in npc.questDialogues)
                    {
                        log += $"  - Obj: {q.requiredObjective}, Node: {(q.dialogueNode == null ? "NULL" : q.dialogueNode.name)}\n";
                    }
                }
            }
        }
        
        File.WriteAllText("Assets/kalaw_dump.txt", log);
        Debug.Log("Dump written to Assets/kalaw_dump.txt");
    }
}

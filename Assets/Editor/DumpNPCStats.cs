using UnityEngine;
using UnityEditor;
using System.IO;

public class DumpNPCStats : EditorWindow
{
    [MenuItem("Tools/Luminang/Dump NPC Stats")]
    public static void DumpStats()
    {
        InteractableNPC[] allNPCs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        string report = "NPC Stats Report:\n\n";
        foreach(var npc in allNPCs)
        {
            report += $"Name: {npc.name}\n";
            report += $"Scale: {npc.transform.localScale}\n";
            
            CapsuleCollider col = npc.GetComponent<CapsuleCollider>();
            if (col != null)
            {
                report += $"Collider: height={col.height}, radius={col.radius}, center={col.center}\n";
            }
            else
            {
                report += "Collider: NONE\n";
            }
            
            UnityEngine.AI.NavMeshAgent agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null)
            {
                report += $"NavMeshAgent: radius={agent.radius}, height={agent.height}, speed={agent.speed}\n";
            }
            else
            {
                report += "NavMeshAgent: NONE\n";
            }
            
            report += "-------------------------\n";
        }
        
        File.WriteAllText("NPC_Stats_Dump.txt", report);
        Debug.Log("Dumped NPC stats to NPC_Stats_Dump.txt");
    }
}

using UnityEngine;
using UnityEditor;

public class DumpNPCs : MonoBehaviour
{
    [MenuItem("Luminang/Dump NPCs")]
    public static void Dump()
    {
        var npcs = Object.FindObjectsByType<InteractableNPC>(FindObjectsSortMode.None);
        Debug.Log($"Found {npcs.Length} NPCs");
        foreach (var npc in npcs)
        {
            Debug.Log($"NPC Name: {npc.gameObject.name}");
        }
    }
}

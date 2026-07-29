using UnityEngine;
using UnityEditor;

public static class DebugKalaw
{
    [MenuItem("Tools/Debug Kalaw In Play Mode")]
    public static void Check()
    {
        InteractableNPC[] npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int kalawCount = 0;
        foreach (var npc in npcs)
        {
            if (npc.gameObject.name.Contains("Kalaw", System.StringComparison.OrdinalIgnoreCase))
            {
                kalawCount++;
                Debug.Log($"[DebugKalaw] Found Kalaw: {npc.gameObject.name} (Instance ID: {npc.gameObject.GetInstanceID()})");
                if (npc.defaultDialogue == null)
                    Debug.Log($"[DebugKalaw] -> defaultDialogue is NULL on {npc.gameObject.name}!");
                else
                    Debug.Log($"[DebugKalaw] -> defaultDialogue is Assigned: {npc.defaultDialogue.name} on {npc.gameObject.name}");
                
                if (npc.questDialogues != null)
                    Debug.Log($"[DebugKalaw] -> questDialogues Count: {npc.questDialogues.Count} on {npc.gameObject.name}");
            }
        }
        if (kalawCount == 0) Debug.Log("[DebugKalaw] Could not find any object with 'Kalaw' in its name!");
    }
}

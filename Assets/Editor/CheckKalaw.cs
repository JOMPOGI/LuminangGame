using UnityEngine;
using UnityEditor;

public static class CheckKalaw
{
    [MenuItem("Tools/Check Kalaw Default Dialogue")]
    public static void Check()
    {
        InteractableNPC[] npcs = GameObject.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in npcs)
        {
            if (npc.gameObject.name == "Kalaw")
            {
                Debug.Log("Kalaw InteractableNPC found!");
                if (npc.defaultDialogue == null)
                    Debug.Log("defaultDialogue is literally NULL!");
                else
                    Debug.Log("defaultDialogue is: " + npc.defaultDialogue.name);
            }
        }
    }
}

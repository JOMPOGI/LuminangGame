using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class WipAllDialogues
{
    [MenuItem("Luminang/WIPE ALL DIALOGUES")]
    public static void Run()
    {
        GameObject[] sceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int count = 0;
        foreach (GameObject go in sceneObjects)
        {
            InteractableNPC npc = go.GetComponent<InteractableNPC>();
            if (npc != null)
            {
                Undo.RecordObject(npc, "Wipe All Dialogues");
                
                // Clear all dialogues
                npc.defaultDialogue = null;
                npc.questDialogues.Clear();
                
                // Disable interaction
                npc.interactionEnabled = false;
                
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        
        EditorSceneManager.SaveOpenScenes();
        Debug.Log($"Successfully wiped dialogues and disabled interactions for {count} NPCs in the scene.");
    }
}

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class InteractionEnabler : EditorWindow
{
    [MenuItem("Luminang/Emergency Enable All Interactions")]
    public static void Run()
    {
        GameObject[] sceneObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int count = 0;
        foreach (GameObject go in sceneObjects)
        {
            InteractableNPC npc = go.GetComponent<InteractableNPC>();
            if (npc != null)
            {
                Undo.RecordObject(npc, "Enable Interaction");
                npc.interactionEnabled = true;
                EditorUtility.SetDirty(npc);
                count++;
            }
        }
        
        EditorSceneManager.SaveOpenScenes();
        Debug.Log($"Successfully force-enabled interaction for {count} NPCs in the scene!");
    }
}

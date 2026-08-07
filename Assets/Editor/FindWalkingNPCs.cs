using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class FindWalkingNPCs : EditorWindow
{
    [MenuItem("Tools/Find Walking NPCs")]
    public static void DoWork()
    {
        Scene magellan = EditorSceneManager.OpenScene("Assets/Scenes/Environments/Magellan's_Cross.unity", OpenSceneMode.Additive);
        
        var npcs = new List<InteractableNPC>();
        foreach (var root in magellan.GetRootGameObjects())
        {
            npcs.AddRange(root.GetComponentsInChildren<InteractableNPC>(true));
        }

        foreach (var npc in npcs)
        {
            var patrol = npc.GetComponent<NPCPatrol>();
            if (patrol != null && patrol.waypoints != null && patrol.waypoints.Length > 0)
            {
                Debug.Log($"Walking NPC in Magellan's Cross: {npc.name}");
                for (int i=0; i<patrol.waypoints.Length; i++) {
                    if (patrol.waypoints[i].point != null)
                        Debug.Log($"  - WP {i}: {patrol.waypoints[i].point.position}");
                }
            }
        }
        
        EditorSceneManager.CloseScene(magellan, true);
    }
}

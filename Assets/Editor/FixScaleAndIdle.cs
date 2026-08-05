using UnityEngine;
using UnityEditor;

public class FixScaleAndIdle : EditorWindow
{
    [MenuItem("Tools/Fix NPC Scale & Idle Animations")]
    public static void DoWork()
    {
        // 1. Find Tala to get her exact size
        Vector3 targetScale = Vector3.one;
        var tala = GameObject.Find("Tala") ?? GameObject.Find("Tala_Rigged");
        if (tala != null)
        {
            targetScale = tala.transform.localScale;
            Debug.Log($"[Fix] Found Tala's scale: {targetScale}");
        }
        else
        {
            Debug.LogWarning("[Fix] Could not find Tala! Defaulting to scale 0.5.");
            targetScale = new Vector3(0.5f, 0.5f, 0.5f); // fallback
        }

        // 2. Fix the New NPCs
        string[] newNPCs = { "Dave", "Wayne", "Jen", "Lina", "Mishang", "Pedro" };
        foreach (var npc in newNPCs)
        {
            var obj = GameObject.Find(npc) ?? GameObject.Find(npc + "_Rigged");
            if (obj != null)
            {
                // Fix Scale!
                obj.transform.localScale = targetScale;

                // Strip away any weird animations in Patrol
                var patrol = obj.GetComponent<NPCPatrol>();
                if (patrol != null && patrol.waypoints != null)
                {
                    for (int i = 0; i < patrol.waypoints.Length; i++)
                    {
                        patrol.waypoints[i].idleStateName = "Breathing Idle"; // Force breathing only
                    }
                }

                // Strip away any weird animations in Random Idle
                var anim = obj.GetComponentInChildren<Animator>();
                if (anim != null)
                {
                    var randomIdle = anim.gameObject.GetComponent<NPCRandomIdle>();
                    if (randomIdle != null)
                    {
                        randomIdle.defaultIdleState = "Breathing Idle";
                        // Only let them do Breathing Idle, no looking down or bending over!
                        randomIdle.randomIdleStates = new string[] { "Breathing Idle" }; 
                    }
                }
                
                EditorUtility.SetDirty(obj);
            }
        }

        // Also fix the shared NPCs just in case they are bending over too
        var allNPCs = FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in allNPCs)
        {
            var anim = npc.GetComponentInChildren<Animator>();
            if (anim != null)
            {
                var randomIdle = anim.gameObject.GetComponent<NPCRandomIdle>();
                if (randomIdle != null)
                {
                    randomIdle.defaultIdleState = "Breathing Idle";
                    randomIdle.randomIdleStates = new string[] { "Breathing Idle" }; 
                }
            }

            var patrol = npc.GetComponent<NPCPatrol>();
            if (patrol != null && patrol.waypoints != null)
            {
                for (int i = 0; i < patrol.waypoints.Length; i++)
                {
                    patrol.waypoints[i].idleStateName = "Breathing Idle";
                }
            }
            EditorUtility.SetDirty(npc.gameObject);
        }

        Debug.Log("<color=green>[Fix] COMPLETE! Resized new NPCs to match Tala and locked all idle animations to Breathing Idle.</color>");
    }
}

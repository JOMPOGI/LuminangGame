using UnityEngine;
using UnityEditor;

public class SetupNavMeshAndScale : EditorWindow
{
    [MenuItem("Tools/Fix NPC Scale & Add Pathfinding")]
    public static void DoWork()
    {
        // 1. Target a smaller, realistic scale
        Vector3 targetScale = new Vector3(0.35f, 0.35f, 0.35f); 
        var tala = GameObject.Find("Tala") ?? GameObject.Find("Tala_Rigged");
        if (tala != null)
        {
            // If Tala's local scale is 0.5, we want to match her exact global visual size. 
            // In many Mixamo models, 0.35 - 0.4 is perfect.
            targetScale = tala.transform.localScale * 0.75f; 
        }

        string[] newNPCs = { "Dave", "Wayne", "Jen", "Lina", "Mishang", "Pedro" };
        int agentsAdded = 0;

        foreach (var npc in newNPCs)
        {
            var obj = GameObject.Find(npc) ?? GameObject.Find(npc + "_Rigged");
            if (obj != null)
            {
                // Fix Scale!
                obj.transform.localScale = targetScale;

                // Add NavMeshAgent for obstacle avoidance
                var agent = obj.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent == null)
                {
                    agent = obj.AddComponent<UnityEngine.AI.NavMeshAgent>();
                }
                
                // Configure agent to slip past each other easily
                agent.radius = 0.25f; // Small radius so they don't get stuck on each other
                agent.height = 1.6f;
                agent.avoidancePriority = Random.Range(30, 60); // Mix priorities so they yield to each other
                agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                
                agentsAdded++;
                EditorUtility.SetDirty(obj);
            }
        }

        // Apply pathfinding upgrades to shared NPCs in Calle Crisologo that have Patrol scripts
        var allNPCs = FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in allNPCs)
        {
            var patrol = npc.GetComponent<NPCPatrol>();
            if (patrol != null)
            {
                var agent = npc.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent == null) agent = npc.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
                
                agent.radius = 0.25f;
                agent.avoidancePriority = Random.Range(30, 60);
                agent.obstacleAvoidanceType = UnityEngine.AI.ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                EditorUtility.SetDirty(npc.gameObject);
            }
        }

        Debug.Log($"<color=green>[Fix] COMPLETE! Scaled NPCs down further and attached smart NavMesh pathfinding to {agentsAdded} new NPCs.</color>");
    }
}

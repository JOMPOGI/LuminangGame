using UnityEngine;
using UnityEditor;

public class CleanupNavMeshAndScale : EditorWindow
{
    [MenuItem("Tools/Revert NavMesh & Shrink NPCs More")]
    public static void DoWork()
    {
        // Remove NavMeshAgent from all NPCs to stop the freezing!
        var allAgents = FindObjectsByType<UnityEngine.AI.NavMeshAgent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int agentsRemoved = 0;
        foreach (var agent in allAgents)
        {
            DestroyImmediate(agent);
            agentsRemoved++;
        }

        // Force a much smaller scale
        Vector3 tinyScale = new Vector3(0.25f, 0.25f, 0.25f);

        string[] newNPCs = { "Dave", "Wayne", "Jen", "Lina", "Mishang", "Pedro" };
        foreach (var npc in newNPCs)
        {
            var obj = GameObject.Find(npc) ?? GameObject.Find(npc + "_Rigged");
            if (obj != null)
            {
                obj.transform.localScale = tinyScale;
                EditorUtility.SetDirty(obj);
            }
        }

        Debug.Log($"<color=green>[Fix] COMPLETE! Removed {agentsRemoved} crashing NavMesh components. Shrunk new NPCs to 0.25 scale.</color>");
    }
}

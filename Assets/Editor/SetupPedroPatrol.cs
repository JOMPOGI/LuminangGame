using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetupPedroPatrol : EditorWindow
{
    [MenuItem("Tools/Luminang/Setup Pedro Patrol")]
    public static void SetupPedro()
    {
        string scenePath = "Assets/Scenes/Environments/Calle_Crisologo.unity";
        Scene currentScene = EditorSceneManager.GetActiveScene();
        
        bool wasOpen = currentScene.path == scenePath;
        if (!wasOpen)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                currentScene = EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                Debug.LogWarning("Aborted because unsaved scenes could not be saved.");
                return;
            }
        }

        GameObject neneng = GameObject.Find("Neneng_Rigged");
        GameObject pedro = GameObject.Find("Pedro_Rigged");

        if (neneng == null || pedro == null)
        {
            Debug.LogError("Could not find Neneng_Rigged or Pedro_Rigged in the scene.");
            return;
        }

        // Get Neneng's patrol
        NPCPatrol nenengPatrol = neneng.GetComponent<NPCPatrol>();
        if (nenengPatrol == null)
        {
            Debug.LogError("Neneng does not have an NPCPatrol script attached.");
            return;
        }

        // Setup Pedro's patrol
        NPCPatrol pedroPatrol = pedro.GetComponent<NPCPatrol>();
        if (pedroPatrol == null)
        {
            pedroPatrol = pedro.AddComponent<NPCPatrol>();
            Debug.Log("Added NPCPatrol to Pedro.");
        }

        // Copy waypoints
        pedroPatrol.waypoints = new PatrolWaypoint[nenengPatrol.waypoints.Length];
        for (int i = 0; i < nenengPatrol.waypoints.Length; i++)
        {
            pedroPatrol.waypoints[i] = new PatrolWaypoint
            {
                point = nenengPatrol.waypoints[i].point,
                waitTime = nenengPatrol.waypoints[i].waitTime,
                idleStateName = nenengPatrol.waypoints[i].idleStateName
            };
        }

        // Make Pedro run! (Increase speed for both so they look like they are playing)
        float runSpeed = 4f; // A light jog / run
        nenengPatrol.speed = runSpeed;
        pedroPatrol.speed = runSpeed;

        UnityEngine.AI.NavMeshAgent nenengAgent = neneng.GetComponent<UnityEngine.AI.NavMeshAgent>();
        UnityEngine.AI.NavMeshAgent pedroAgent = pedro.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nenengAgent != null) nenengAgent.speed = runSpeed;
        if (pedroAgent != null) pedroAgent.speed = runSpeed;

        // Position Pedro slightly behind Neneng so they don't overlap and he follows her
        // Let's place him 1.5 units behind her current facing direction
        Vector3 newPedroPos = neneng.transform.position - (neneng.transform.forward * 1.5f);
        
        // Ensure Pedro is on the ground / same height as Neneng
        newPedroPos.y = neneng.transform.position.y;
        
        pedro.transform.position = newPedroPos;
        pedro.transform.rotation = neneng.transform.rotation;
        
        // Also ensure Pedro's NavMeshAgent is synced with this new position
        if (pedroAgent != null)
        {
            pedroAgent.Warp(newPedroPos);
        }

        EditorUtility.SetDirty(neneng);
        EditorUtility.SetDirty(pedro);
        
        EditorSceneManager.MarkSceneDirty(currentScene);
        EditorSceneManager.SaveScene(currentScene);
        
        Debug.Log("<color=green>SUCCESS: Pedro is now configured to chase Neneng on the same patrol path!</color>");
    }
}

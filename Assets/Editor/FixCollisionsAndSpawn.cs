using UnityEngine;
using UnityEditor;

public class FixCollisionsAndSpawn
{
    [MenuItem("Tools/Fix Player Spawn and Collisions")]
    public static void Fix()
    {
        var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (activeScene.name != "Calle_Crisologo")
        {
            Debug.LogWarning("Please open Calle_Crisologo scene first.");
            return;
        }

        // 1. Move Player
        GameObject player = GameObject.Find("PlayerArmature");
        if (player != null)
        {
            GameObject church = GameObject.Find("Cathedral");
            if (church != null)
            {
                // Move player to front of church
                player.transform.position = church.transform.position + new Vector3(0, 1f, -15f);
                player.transform.rotation = Quaternion.Euler(0, 0, 0);
                Debug.Log($"<color=green>[Spawn Fix]</color> Moved player to church: {player.transform.position}");
            }
            else
            {
                Debug.LogWarning("Could not find 'Cathedral' object.");
            }
            
            // Ensure player has CharacterController or Collider enabled
            var cc = player.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = true;
        }
        else
        {
            Debug.LogError("PlayerArmature not found!");
        }

        // 2. Add MeshColliders to environment objects
        MeshRenderer[] renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int collidersAdded = 0;
        foreach (var r in renderers)
        {
            if (r.gameObject.CompareTag("Player") || 
                r.gameObject.GetComponentInParent<InteractableNPC>() != null ||
                r.gameObject.GetComponent<Animator>() != null) 
                continue;
            
            if (r.gameObject.GetComponent<Collider>() == null)
            {
                r.gameObject.AddComponent<MeshCollider>();
                collidersAdded++;
            }
        }

        // 3. Fix NPC Colliders
        InteractableNPC[] npcs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in npcs)
        {
            if (npc.gameObject.GetComponent<Collider>() == null)
            {
                var capsule = npc.gameObject.AddComponent<CapsuleCollider>();
                capsule.height = 2f;
                capsule.radius = 0.4f;
                capsule.center = new Vector3(0, 1f, 0);
            }
            else
            {
                // Ensure there is at least one non-trigger collider
                Collider[] cols = npc.GetComponents<Collider>();
                bool hasSolid = false;
                foreach(var c in cols)
                {
                    if (!c.isTrigger) hasSolid = true;
                }
                if (!hasSolid)
                {
                    var capsule = npc.gameObject.AddComponent<CapsuleCollider>();
                    capsule.height = 2f;
                    capsule.radius = 0.4f;
                    capsule.center = new Vector3(0, 1f, 0);
                }
            }
        }

        Debug.Log($"<color=green>[Collision Fix]</color> Added {collidersAdded} missing colliders to environment.");
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }
}

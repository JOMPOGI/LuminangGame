using UnityEngine;
using UnityEditor;

public class FixNPCsCalleCrisologo : EditorWindow
{
    [MenuItem("Tools/Fix Cloned NPCs & Animations")]
    public static void Fix()
    {
        // Find Player position
        Vector3 playerPos = Vector3.zero;
        var player = GameObject.Find("NestedParentArmature_Unpack") ?? GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPos = player.transform.position;
            Debug.Log($"[Fix] Found Player at {playerPos}");
        }

        // 1. Move the New NPCs so they are visible near the player
        string[] newNPCs = { "Dave", "Wayne", "Jen", "Lina", "Mishang", "Pedro" };
        int i = 0;
        foreach (string npcName in newNPCs)
        {
            GameObject obj = GameObject.Find(npcName) ?? GameObject.Find(npcName + "_Rigged");
            if (obj != null)
            {
                // Place them in a row slightly in front of the player
                obj.transform.position = playerPos + new Vector3(2 + (i * 1.5f), 0, 5);
                
                // Fix their animator too!
                FixAnimator(obj);
                
                Debug.Log($"[Fix] Moved {obj.name} near player!");
                i++;
                EditorUtility.SetDirty(obj);
            }
        }

        // 2. Fix the Shared NPCs Animations!
        var allNPCs = FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int animsFixed = 0;

        foreach (var npc in allNPCs)
        {
            if (FixAnimator(npc.gameObject))
            {
                animsFixed++;
            }
        }
        
        // Fix Kalaw explicitly
        var kalaw = GameObject.Find("Kalaw");
        if (kalaw != null) FixAnimator(kalaw);

        Debug.Log($"<color=green>[Fix] COMPLETE! Moved {i} new NPCs. Fixed Animators for {animsFixed} shared NPCs.</color>");
    }

    private static bool FixAnimator(GameObject rootObj)
    {
        string controllerPath = "Assets/Animations/NPC_Animations/Controllers/Universal_Humanoid_NPC.controller";
        var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);

        // Find the actual mesh child that holds the Animator
        var anim = rootObj.GetComponentInChildren<Animator>(true);
        if (anim != null)
        {
            // Force assign the new controller
            if (anim.runtimeAnimatorController != controller && controller != null)
            {
                anim.runtimeAnimatorController = controller;
            }
            anim.applyRootMotion = false; // Usually true causes sliding if animations don't have root motion
            
            // Force attach the idle script
            var idleScript = anim.gameObject.GetComponent<NPCRandomIdle>();
            if (idleScript == null)
            {
                idleScript = anim.gameObject.AddComponent<NPCRandomIdle>();
            }
            
            // Force the exact state names based on Mixamo FBXs we pulled
            idleScript.defaultIdleState = "Breathing Idle"; // The FBX is named "Breathing Idle"
            idleScript.randomIdleStates = new string[] { "Idling", "Looking Down", "ExtraLooking" }; 
            idleScript.minWaitTime = 4f;
            idleScript.maxWaitTime = 12f;
            
            EditorUtility.SetDirty(anim.gameObject);
            return true;
        }
        return false;
    }
}

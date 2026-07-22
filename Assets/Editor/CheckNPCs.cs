using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CheckNPCs : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Luminang/Check All NPCs")]
    public static void Check()
    {
        var npcNames = new string[]
        {
            "Kalaw", "Ronnie_Rigged", "Sally_Rigged", "Lito_Rigged", "NPC_Apo_Lakay",
            "Tomas_Rigged", "Klara_Rigged", "Tala_Rigged", "MangLance_Rigged",
            "Rayo_Rigged", "AlingRosa_Rigged", "LolaNida_Rigged", "Neneng_Rigged",
            "AlingRiza_Rigged", "Pedro_Rigged", "LolaBebang_Rigged"
        };

        Debug.Log("--- STARTING NPC CHECK ---");
        foreach (var name in npcNames)
        {
            // Find even inactive objects
            GameObject go = null;
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
                foreach (var t in transforms)
                {
                    if (t.name == name)
                    {
                        go = t.gameObject;
                        break;
                    }
                }
                if (go != null) break;
            }

            if (go == null)
            {
                Debug.LogError($"[MISSING] {name} is NOT IN THE SCENE!");
                continue;
            }

            bool isActive = go.activeInHierarchy;
            var npcScript = go.GetComponent<InteractableNPC>();
            var animator = go.GetComponent<Animator>();

            Debug.Log($"[FOUND] {name} - Active: {isActive}, Position: {go.transform.position}, HasInteractable: {npcScript != null}, HasAnimator: {animator != null}");
        }
        Debug.Log("--- FINISHED NPC CHECK ---");
    }
#endif
}

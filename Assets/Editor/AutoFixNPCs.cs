using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AutoFixNPCs : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Luminang/ULTIMATE FIX: Auto-Configure All NPCs")]
    public static void FixAll()
    {
        AssetDatabase.Refresh();
        string dir = "Assets/Dialogues/GDD_Flow/";
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Dialogue&Quest/Quest_Indicator.prefab");

        var npcMap = new Dictionary<string, string>()
        {
            {"Kalaw", "Kalaw_Start"},
            {"Ronnie_Rigged", "Ronnie_Start"},
            {"Sally_Rigged", "Sally_Start"},
            {"Lito_Rigged", "Lito_Start"},
            {"NPC_Apo_Lakay", "ApoLakay_Start"},
            {"Tomas_Rigged", "Tomas_Start"},
            {"Klara_Rigged", "Klara_Start"},
            {"Tala_Rigged", "Tala_Start"},
            {"MangLance_Rigged", "MangLance_Start"},
            {"Rayo_Rigged", "Rayo_Start"},
            {"AlingRosa_Rigged", "AlingRosa_Start"},
            {"Neneng_Rigged", "Neneng_Start"},
            {"AlingRiza_Rigged", "AlingRiza_Start"},
            {"Pedro_Rigged", "Pedro_Start"},
            {"LolaBebang_Rigged", "LolaBebang_Start"}
        };

        var questStages = new Dictionary<string, List<(string, string)>>()
        {
            {"Kalaw", new List<(string, string)> { ("Return to Kalaw for Final Test", "Kalaw_Final_Start") }},
            {"Ronnie_Rigged", new List<(string, string)> { ("Talk to Ronnie again", "Ronnie_Identity_Start") }},
            {"Sally_Rigged", new List<(string, string)> { ("Talk to Sally for Requests", "Sally_Requests_Start") }},
            {"Lito_Rigged", new List<(string, string)> { ("Talk to Lito for Directions", "Lito_Directions_Start") }},
            {"Klara_Rigged", new List<(string, string)> { ("Talk to Klara for Counting", "Klara_Counting_Start") }},
            {"MangLance_Rigged", new List<(string, string)> { ("Talk to Mang Lance for Verbs", "MangLance_Verbs_Start") }},
            {"AlingRosa_Rigged", new List<(string, string)> { ("Talk to Aling Rosa for Linking Verbs", "AlingRosa_Linking_Start") }},
            {"AlingRiza_Rigged", new List<(string, string)> { ("Talk to Aling Riza for Pronouns", "AlingRiza_Pronouns_Start") }},
            {"LolaBebang_Rigged", new List<(string, string)> { ("Talk to Lola Bebang for Interrogatives", "LolaBebang_Interrogatives_Start") }}
        };

        foreach (var kvp in npcMap)
        {
            // 1. Find the GameObject (even if it is disabled/inactive)
            GameObject go = null;
            string cleanTargetName = kvp.Key.Replace(" ", "").Replace("_", "").ToLower();
            
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
                foreach (var t in transforms)
                {
                    string cleanObjName = t.name.Replace(" ", "").Replace("_", "").ToLower();
                    if (cleanObjName == cleanTargetName)
                    {
                        go = t.gameObject;
                        break;
                    }
                }
                if (go != null) break;
            }

            if (go == null)
            {
                Debug.LogError($"[ULTIMATE FIX] MISSING FROM SCENE: {kvp.Key}. Did you accidentally delete or rename them?");
                continue;
            }

            // Force the NPC to be active in the scene so you can actually see and talk to them!
            if (!go.activeSelf)
            {
                go.SetActive(true);
                Debug.Log($"[ULTIMATE FIX] Force-enabled {go.name} because it was turned off in the Inspector!");
            }

            // 2. Ensure they have a Collider (otherwise you can't click them!)
            var col = go.GetComponent<Collider>();
            if (col == null)
            {
                var cap = go.AddComponent<CapsuleCollider>();
                cap.center = new Vector3(0, 1f, 0);
                cap.height = 2f;
                cap.radius = 0.5f;
                Debug.Log($"Added missing CapsuleCollider to {go.name}");
            }

            // 3. Ensure they have an Animator
            var anim = go.GetComponent<Animator>();
            if (anim == null)
            {
                anim = go.AddComponent<Animator>();
                Debug.Log($"Added missing Animator to {go.name}");
            }

            // 4. Ensure they have the InteractableNPC script
            var npc = go.GetComponent<InteractableNPC>();
            if (npc == null)
            {
                npc = go.AddComponent<InteractableNPC>();
                Debug.Log($"Added missing InteractableNPC script to {go.name}");
            }

            // 5. Wire up the scripts correctly
            npc.npcAnimator = anim;
            npc.interactionEnabled = (go.name == "Kalaw");
            npc.disableAfterInteraction = false;
            npc.dialogueEvents.Clear();
            npc.questDialogues.Clear();

            // 6. Assign Default Dialogue
            string assetPath = dir + kvp.Value + ".asset";
            DialogueNode node = AssetDatabase.LoadAssetAtPath<DialogueNode>(assetPath);
            if (node != null) npc.defaultDialogue = node;

            // 7. Assign Multi-stage Dialogues
            if (questStages.ContainsKey(kvp.Key))
            {
                foreach (var stage in questStages[kvp.Key])
                {
                    string stagePath = dir + stage.Item2 + ".asset";
                    DialogueNode stageNode = AssetDatabase.LoadAssetAtPath<DialogueNode>(stagePath);
                    if (stageNode != null)
                    {
                        var qd = new InteractableNPC.QuestDialogue();
                        qd.requiredObjective = stage.Item1;
                        qd.dialogueNode = stageNode;
                        npc.questDialogues.Add(qd);
                    }
                }
            }

            // 8. Add the Floating Quest Indicator
            if (prefab != null && npc.GetComponentInChildren<QuestIndicator>() == null)
            {
                var indicator = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                indicator.transform.SetParent(npc.transform);
                indicator.transform.localPosition = new Vector3(0, 2.5f, 0); // Hover above head
                indicator.transform.localRotation = Quaternion.identity;
            }

            EditorUtility.SetDirty(go);
        }

        AssetDatabase.SaveAssets();
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log("ALL NPCs SUCCESSFULLY FIXED, CONFIGURED, AND WIRED UP!");
    }
#endif
}

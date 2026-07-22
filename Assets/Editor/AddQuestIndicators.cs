using UnityEngine;
using UnityEditor;

public class AddQuestIndicators : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Luminang/Add Quest Indicators to all NPCs")]
    public static void AddIndicators()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Dialogue&Quest/Quest_Indicator.prefab");
        if (prefab == null)
        {
            Debug.LogError("Could not find Quest_Indicator.prefab!");
            return;
        }

        var npcs = Object.FindObjectsByType<InteractableNPC>(FindObjectsSortMode.None);
        int addedCount = 0;

        foreach (var npc in npcs)
        {
            // Only add if they don't already have one
            if (npc.GetComponentInChildren<QuestIndicator>() == null)
            {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                instance.transform.SetParent(npc.transform);
                // Position it above the NPC's head
                instance.transform.localPosition = new Vector3(0, 2.5f, 0);
                instance.transform.localRotation = Quaternion.identity;
                
                addedCount++;
            }
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"Successfully added Quest Indicators to {addedCount} NPCs!");
    }
#endif
}

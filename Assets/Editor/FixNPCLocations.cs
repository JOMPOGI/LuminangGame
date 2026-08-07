using UnityEngine;
using UnityEditor;

public class FixNPCLocations : EditorWindow
{
    [MenuItem("Tools/Fix Cloned NPC Locations")]
    public static void FixLocations()
    {
        string[] newNPCs = { "Dave", "Wayne", "Jen", "Lina", "Mishang", "Pedro" };
        int i = 0;
        foreach (string npcName in newNPCs)
        {
            // The names might have '_Rigged' or something in the scene
            var obj = GameObject.Find(npcName + "_Rigged") ?? GameObject.Find(npcName);
            if (obj != null)
            {
                // Place them along the street, near the origin
                obj.transform.position = new Vector3(10 + (i * 2), 0, 10);
                Debug.Log($"Moved {obj.name} to {obj.transform.position}");
                i++;
                EditorUtility.SetDirty(obj);
            }
        }
    }
}

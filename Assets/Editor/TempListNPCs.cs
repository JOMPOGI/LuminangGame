using UnityEngine;
using UnityEditor;
using System.IO;

public class TempListNPCs {
    [MenuItem("Luminang/List NPCs")]
    public static void DoList() {
        var npcs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        string output = "NPCs in Scene:\n";
        foreach(var n in npcs) {
            output += n.gameObject.name + "\n";
        }
        File.WriteAllText("temp_npcs.txt", output);
        Debug.Log("Wrote to temp_npcs.txt");
    }
}

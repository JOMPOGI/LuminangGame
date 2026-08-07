using UnityEngine;
using UnityEditor;

public class GetPlayerPos : EditorWindow
{
    [MenuItem("Tools/Get Player Pos")]
    public static void GetPos()
    {
        var player = GameObject.Find("NestedParentArmature_Unpack");
        if (player != null)
        {
            Debug.Log($"Player Position: {player.transform.position}");
        }
        else
        {
            Debug.Log("Player not found.");
        }
    }
}

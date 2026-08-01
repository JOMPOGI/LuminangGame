using UnityEngine;
using UnityEditor;

public static class FindMissingScripts
{
    [MenuItem("Tools/Find Missing Scripts In Scene")]
    public static void Find()
    {
        var gameObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int missingCount = 0;
        foreach (var go in gameObjects)
        {
            var components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.Log("Missing script on GameObject: " + go.name + " at path: " + GetPath(go), go);
                    missingCount++;
                }
            }
        }
        Debug.Log("Total missing scripts found: " + missingCount);
    }
    
    private static string GetPath(GameObject go)
    {
        string path = go.name;
        Transform curr = go.transform.parent;
        while (curr != null)
        {
            path = curr.name + "/" + path;
            curr = curr.parent;
        }
        return path;
    }
}

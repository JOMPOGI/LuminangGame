using UnityEngine;
using UnityEditor;

public static class RemoveMissingScripts
{
    [MenuItem("Tools/Remove Missing Scripts In Scene")]
    public static void Remove()
    {
        var gameObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        int totalRemoved = 0;
        foreach (var go in gameObjects)
        {
            int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (count > 0)
            {
                Debug.Log("Removed " + count + " missing scripts from GameObject: " + go.name);
                totalRemoved += count;
                EditorUtility.SetDirty(go);
            }
        }
        
        if (totalRemoved > 0)
        {
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            Debug.Log("Successfully removed " + totalRemoved + " missing scripts and saved the scene! Please restart Play Mode.");
        }
        else
        {
            Debug.Log("No missing scripts found.");
        }
    }
}

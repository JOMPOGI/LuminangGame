using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class FixCatAndPedro : EditorWindow
{
    [MenuItem("Tools/Luminang/Fix Cat and Pedro")]
    public static void FixThem()
    {
        string scenePath = "Assets/Scenes/Environments/Calle_Crisologo.unity";
        Scene currentScene = EditorSceneManager.GetActiveScene();
        
        bool wasOpen = currentScene.path == scenePath;
        if (!wasOpen)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                currentScene = EditorSceneManager.OpenScene(scenePath);
            }
            else
            {
                Debug.LogWarning("Aborted because unsaved scenes could not be saved.");
                return;
            }
        }

        // 1. Fix the Cat's scale
        GameObject cat = GameObject.Find("Cat");
        if (cat != null)
        {
            cat.transform.localScale = new Vector3(8f, 8f, 8f);
            EditorUtility.SetDirty(cat);
            Debug.Log("Set Cat's scale to 8, 8, 8.");
        }
        else
        {
            Debug.LogWarning("Could not find GameObject named 'Cat'.");
        }

        // 2. Find Pedro and Neneng
        GameObject neneng = null;
        GameObject pedro = null;

        // Pedro might be a prefab instance named "Pedro_Rigged" or something
        InteractableNPC[] npcs = Object.FindObjectsByType<InteractableNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var npc in npcs)
        {
            if (npc.name.Contains("Neneng")) neneng = npc.gameObject;
            if (npc.name.Contains("Pedro")) pedro = npc.gameObject;
        }

        // If Pedro didn't have InteractableNPC, search all objects
        if (pedro == null)
        {
            GameObject[] allGo = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var go in allGo)
            {
                if (go.name.Contains("Pedro")) pedro = go;
                if (neneng == null && go.name.Contains("Neneng") && !go.name.Contains("Waypoints") && !go.name.Contains("WP") && !go.name.Contains("CloseUp")) neneng = go;
            }
        }

        if (neneng != null)
        {
            Debug.Log($"Found Neneng: {neneng.name}");
            Component[] comps = neneng.GetComponents<Component>();
            foreach (var c in comps)
            {
                Debug.Log($"Neneng Component: {c.GetType().Name}");
            }
        }

        if (pedro != null)
        {
            Debug.Log($"Found Pedro: {pedro.name}");
            Component[] comps = pedro.GetComponents<Component>();
            foreach (var c in comps)
            {
                Debug.Log($"Pedro Component: {c.GetType().Name}");
            }
        }

        EditorSceneManager.MarkSceneDirty(currentScene);
        EditorSceneManager.SaveScene(currentScene);
    }
}

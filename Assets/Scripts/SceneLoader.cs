using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static string previousScene;
    
    // The target scene that LoadingScene will eventually load
    public static string targetSceneForLoading;
    private static bool isSceneLoading = false; // Prevents double-triggering the loading screen

    [Header("Transition Settings")]
    public float transitionDelay = 0.4f;
    
    [Header("Loading Screen Setup")]
    public bool useLoadingScreenForSampleScene = true;
    public string loadingSceneName = "LoadingScene";

    public void LoadScene(string sceneName)
    {
        if (isSceneLoading)
        {
            Debug.Log("[SceneLoader] LoadScene ignored - already loading.");
            return;
        }

        Debug.Log("[SceneLoader] LoadScene called for: " + sceneName);
        isSceneLoading = true;
        if (useLoadingScreenForSampleScene && sceneName == "SampleScene")
        {
            Debug.Log("[SceneLoader] Using additive loading for LoadingScene");
            targetSceneForLoading = sceneName;
            
            // Check if LoadingScene is already pre-loaded
            if (IsSceneLoaded(loadingSceneName))
            {
                Debug.Log("[SceneLoader] LoadingScene is already pre-loaded. Activating it.");
                ActivateScene(loadingSceneName);
            }
            else
            {
                Debug.Log("[SceneLoader] LoadingScene not found in memory. Loading fresh.");
                SceneManager.LoadScene(loadingSceneName, LoadSceneMode.Additive);
            }
        }
        else
        {
            Debug.Log("[SceneLoader] Normal loading for: " + sceneName);
            StartCoroutine(LoadSceneWithDelay(sceneName));
        }
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name == sceneName) return true;
        }
        return false;
    }

    private void ActivateScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.IsValid())
        {
            bool triggered = false;
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                obj.SetActive(true);
                
                if (triggered) continue;

                // Try to find the controller and trigger it ONLY ONCE
                var controller = obj.GetComponentInChildren<LoadingSceneController>();
                if (controller != null)
                {
                    controller.PrepareAndShow(targetSceneForLoading);
                    triggered = true;
                    continue;
                }
                
                var mainLoading = obj.GetComponentInChildren<MainLoading>();
                if (mainLoading != null)
                {
                    mainLoading.PrepareAndShow(targetSceneForLoading);
                    triggered = true;
                    continue;
                }
            }
        }
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(transitionDelay);
        if (SceneManager.GetActiveScene().name != loadingSceneName)
        {
            previousScene = SceneManager.GetActiveScene().name;
        }
        isSceneLoading = false; // RESET THE FLAG
        SceneManager.LoadScene(sceneName);
    }

    public void GoBack()
    {
        if (!string.IsNullOrEmpty(previousScene))
        {
            StartCoroutine(GoBackWithDelay());
        }
    }

    private IEnumerator GoBackWithDelay()
    {
        yield return new WaitForSeconds(transitionDelay);
        isSceneLoading = false; // RESET THE FLAG
        SceneManager.LoadScene(previousScene);
    }

    public static void ResetLoadingFlag()
    {
        isSceneLoading = false;
        Debug.Log("[SceneLoader] Loading flag reset.");
    }
}
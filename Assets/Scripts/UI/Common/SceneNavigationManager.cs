using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneNavigationManager
{
    // This is the "Memory" of our navigator
    private static string lastSceneName = "MapSelectionScene"; // Default fallback

    /// <summary>
    /// Loads the customization scene and remembers the current scene.
    /// Uses the game's existing Additive LoadingScene system.
    /// </summary>
    public static void LoadCustomization()
    {
        // 1. Remember where we are right now for the back button
        lastSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("[SceneNavigator] Remembering scene for return: " + lastSceneName);

        // 2. Reset the loading flag in your SceneLoader so it doesn't block us
        SceneLoader.ResetLoadingFlag();

        // 3. Set the target for your existing LoadingScene system
        SceneLoader.targetSceneForLoading = "CharacterCustomizationScene";
        SceneLoader.keepBackgroundPersistent = true; // NEW: Keep the video background visible!
        
        // 4. Load your LoadingScene ADDITIVELY (it will overlay on your current scene)
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
    }

    public static void LoadSTTTest()
    {
        // 1. Remember where we are (MapSelectionScene)
        lastSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("[SceneNavigator] Saving scene: " + lastSceneName);

        // 2. Load the test scene
        SceneManager.LoadScene("STT_TestScene");
    }

    /// <summary>
    /// Returns to the scene we were in before customization.
    /// Uses the smooth Navy/Purple fade.
    /// </summary>
    public static void ReturnToPreviousScene()
    {
        Debug.Log("[SceneNavigator] Returning to: " + lastSceneName);
        
        // If we have a TransitionOverlay in the scene, use it!
        if (TransitionOverlay.Instance != null)
        {
            TransitionOverlay.Instance.StartTransition(lastSceneName);
        }
        else
        {
            // Fallback if no overlay exists
            SceneManager.LoadScene(lastSceneName);
        }
    }
}

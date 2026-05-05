using UnityEngine;

public class MainMenuRedirection : MonoBehaviour
{
    [Header("Scene Names")]
    public string createCharacterScene = "CreateCharacterScene";
    public string prologueScene = "PrologueScene";
    public string mapSelectionScene = "MapSelectionScene";

    public void StartGame()
    {
        string sceneToLoad = createCharacterScene; // Default

        if (UserProfileManager.Instance != null && UserProfileManager.Instance.CurrentProfile != null)
        {
            var profile = UserProfileManager.Instance.CurrentProfile;

            if (!profile.HasCreatedCharacter)
            {
                sceneToLoad = createCharacterScene;
                Debug.Log("[Main Menu] Player has no character. Loading CreateCharacter...");
            }
            else if (!profile.HasSeenPrologue)
            {
                sceneToLoad = prologueScene;
                Debug.Log("[Main Menu] Player hasn't seen prologue. Loading Prologue...");
            }
            else
            {
                sceneToLoad = mapSelectionScene;
                Debug.Log("[Main Menu] Player is ready. Loading Map Selection...");
            }
        }
        else
        {
            Debug.LogWarning("[Main Menu] UserProfileManager not found or profile not loaded. Using default.");
        }

        // Use the SceneLoader if available, otherwise use SceneManager
        var loader = Object.FindFirstObjectByType<SceneLoader>();
        if (loader != null)
        {
            loader.LoadScene(sceneToLoad);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}

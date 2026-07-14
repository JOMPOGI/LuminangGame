using UnityEngine;

namespace Luminang.UI.Common
{
    /// <summary>
    /// Attach this to any button in your scene to easily call the SceneNavigationManager.
    /// </summary>
    public class NavigationButtonHelper : MonoBehaviour
    {
        public void LoadSTTTest()
        {
            SceneNavigationManager.LoadSTTTest();
        }

        public void BackToPrevious()
        {
            SceneNavigationManager.ReturnToPreviousScene();
        }

        // Shortcut to just load any scene by name if needed
        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}

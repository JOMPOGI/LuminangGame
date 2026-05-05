using UnityEngine;
using System.Collections;

public class PrologueController : MonoBehaviour
{
    public float waitTime = 5.0f;
    public string nextSceneName = "MapSelectionScene";
    public SceneLoader sceneLoader;

    void Start()
    {
        StartCoroutine(StartPrologueTimer());
    }

    private IEnumerator StartPrologueTimer()
    {
        Debug.Log($"[Prologue] Starting {waitTime} second timer...");
        yield return new WaitForSeconds(waitTime);
        
        Debug.Log("[Prologue] Timer finished. Updating progress and loading next scene.");
        
        if (UserProfileManager.Instance != null)
        {
            var task = UserProfileManager.Instance.SetPrologueSeen(true);
            yield return new WaitUntil(() => task.IsCompleted);
        }

        if (sceneLoader != null)
        {
            sceneLoader.LoadScene(nextSceneName);
        }
        else
        {
            // Fallback if sceneLoader isn't assigned
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}

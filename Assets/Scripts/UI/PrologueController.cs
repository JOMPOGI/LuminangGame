using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class PrologueController : MonoBehaviour
{
    [Header("Video Settings")]
    public VideoPlayer videoPlayer;
    public float fallbackWaitTime = 5.0f;
    public string nextSceneName = "MapSelectionScene";
    public SceneLoader sceneLoader;

    void Start()
    {
        StartCoroutine(StartPrologueSequence());
    }

    private IEnumerator StartPrologueSequence()
    {
        if (videoPlayer != null)
        {
            Debug.Log("[Prologue] Waiting for Video Player...");
            
            // Wait for it to prepare
            if (!videoPlayer.isPrepared)
            {
                videoPlayer.Prepare();
                while (!videoPlayer.isPrepared) yield return null;
            }

            videoPlayer.Play();
            Debug.Log("[Prologue] Video playing...");

            // Wait until it reaches the end
            while (videoPlayer.isPlaying)
            {
                yield return null;
            }
            
            // Tiny buffer to ensure it's truly done
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            Debug.Log($"[Prologue] No VideoPlayer found. Falling back to {fallbackWaitTime} second timer...");
            yield return new WaitForSeconds(fallbackWaitTime);
        }
        
        Debug.Log("[Prologue] Sequence finished. Updating progress and loading next scene.");
        
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
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}

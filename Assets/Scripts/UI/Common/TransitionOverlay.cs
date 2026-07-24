using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionOverlay : MonoBehaviour
{
    public static TransitionOverlay Instance;

    [Header("Settings")]
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.6f;

    void Awake()
    {
        // Singleton pattern: stays alive between scenes
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        
        // Start hidden
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public void StartTransition(string targetScene)
    {
        StartCoroutine(TransitionRoutine(targetScene));
    }

    private IEnumerator TransitionRoutine(string targetScene)
    {
        // 1. Fade In (Show the Navy/Purple screen)
        canvasGroup.blocksRaycasts = true;
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // 2. Load the scene while screen is dark
        AsyncOperation op = SceneManager.LoadSceneAsync(targetScene);
        while (!op.isDone) yield return null;

        // Give the new scene a frame to settle
        yield return new WaitForEndOfFrame();

        // 3. Fade Out (Reveal the new scene)
        elapsed = 0;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            yield return null;
        }
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}

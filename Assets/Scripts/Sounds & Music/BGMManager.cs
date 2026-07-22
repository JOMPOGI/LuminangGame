using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance { get; private set; }
    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern: Ensure only one instance of BGMManager exists
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            
            // Init volume
            UpdateVolume();

            // Start listening for scene changes
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Start listening for volume changes
            AudioManager.onMusicVolumeChange += UpdateVolume;
        }
        else
        {
            // If another instance already exists, destroy this duplicate
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string name = scene.name.ToLower();
        
        // STOP music if we enter the actual gameplay level
        if (name.Contains("sample") || name.Contains("game"))
        {
             Debug.Log("[BGMManager] Stopping menu music for gameplay transition.");
             audioSource.Stop();
        }
        else
        {
            // For all other scenes (MainMenu, Login, Signup, Loading, etc.)
            // Keep the music running!
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    private void UpdateVolume()
    {
        if (audioSource != null && AudioManager.instance != null)
        {
            audioSource.volume = AudioManager.instance.musicVolume;
        }
    }

    private void OnDestroy()
    {
        // Clean up event listener when destroyed
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            AudioManager.onMusicVolumeChange -= UpdateVolume;
        }
    }

    /// <summary>
    /// Smoothly fades the BGM volume to a target value.
    /// </summary>
    public void FadeVolume(float targetVolume, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(targetVolume, duration));
    }

    private IEnumerator FadeCoroutine(float targetVolume, float duration)
    {
        if (audioSource == null) yield break;

        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}

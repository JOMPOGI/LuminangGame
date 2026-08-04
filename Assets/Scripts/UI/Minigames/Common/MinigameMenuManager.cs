using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MinigameMenuManager : MonoBehaviour
{
    [Header("Menu UI")]
    public GameObject menuGroup;
    public GameObject menuPanel;

    [Header("Optional Links")]
    [Tooltip("Link the HowToPlay group if this minigame has one, so the menu can open it.")]
    public GameObject howToPlayGroup;
    public GameObject howToPlayPanel;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip buttonClickSFX;
    public AudioClip panelOpenSFX;

    private bool isAnimating = false;

    public void OpenMenu()
    {
        if (isAnimating) return;
        if (sfxSource != null && buttonClickSFX != null) sfxSource.PlayOneShot(buttonClickSFX);
        menuGroup.SetActive(true);
        if (sfxSource != null && panelOpenSFX != null) sfxSource.PlayOneShot(panelOpenSFX);
        StartCoroutine(AnimatePanelIn(menuPanel.transform));
    }

    public void ResumeGame()
    {
        if (isAnimating) return;
        if (sfxSource != null && buttonClickSFX != null) sfxSource.PlayOneShot(buttonClickSFX);
        StartCoroutine(AnimatePanelOut(menuPanel.transform, () =>
        {
            menuGroup.SetActive(false);
        }));
    }

    public void RestartMinigame()
    {
        if (sfxSource != null && buttonClickSFX != null) sfxSource.PlayOneShot(buttonClickSFX);
        // Reloads the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitMinigame()
    {
        if (sfxSource != null && buttonClickSFX != null) sfxSource.PlayOneShot(buttonClickSFX);
        // Tell the hub scene we did NOT win/complete the objective
        PlayerPrefs.SetInt("FishingMinigameWon", 0);
        PlayerPrefs.SetInt("MinigameWon", 0); // General fallback flag for other games
        PlayerPrefs.Save();

        // Load previous scene, or LanguageSelectionScene if none exists
        string prevScene = PlayerPrefs.GetString("PreviousScene", "LanguageSelectionScene");
        SceneManager.LoadScene(prevScene);
    }

    public void OpenHowToPlay()
    {
        if (howToPlayGroup == null)
        {
            Debug.LogWarning("How To Play Group is not assigned in the MinigameMenuManager!");
            return;
        }

        if (isAnimating) return;
        if (sfxSource != null && buttonClickSFX != null) sfxSource.PlayOneShot(buttonClickSFX);

        // Close the menu panel first, then open How To Play
        StartCoroutine(AnimatePanelOut(menuPanel.transform, () =>
        {
            menuGroup.SetActive(false);
            
            howToPlayGroup.SetActive(true);
            if (sfxSource != null && panelOpenSFX != null) sfxSource.PlayOneShot(panelOpenSFX);
            if (howToPlayPanel != null)
            {
                StartCoroutine(AnimatePanelIn(howToPlayPanel.transform));
            }
        }));
    }

    // --- Animation Coroutines ---

    private IEnumerator AnimatePanelIn(Transform panelTransform)
    {
        isAnimating = true;
        panelTransform.localScale = Vector3.zero;
        
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            panelTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 1.1f, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            panelTransform.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.one, elapsed / 0.1f);
            yield return null;
        }

        panelTransform.localScale = Vector3.one;
        isAnimating = false;
    }

    private IEnumerator AnimatePanelOut(Transform panelTransform, System.Action onComplete = null)
    {
        isAnimating = true;
        float duration = 0.2f;
        float elapsed = 0f;
        Vector3 startScale = panelTransform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            panelTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / duration);
            yield return null;
        }

        panelTransform.localScale = Vector3.zero;
        isAnimating = false;
        
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }
}

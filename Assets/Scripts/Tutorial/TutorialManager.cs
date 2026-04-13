using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image tutorialDisplayImage; // The picture that changes
    public Button nextButton;
    public Button previousButton;
    
    [Header("Tutorial Slides")]
    [Tooltip("Drag all your tutorial pictures here in order")]
    public Sprite[] tutorialSlides;
    
    [Header("Pagination Indicator")]
    [Tooltip("Drag the Images acting as dots here, in order from left to right")]
    public Image[] pageDots; 
    public Sprite dotActiveSprite;
    public Sprite dotInactiveSprite;

    [Header("Button States")]
    [Tooltip("The sprite used for the Next button normally")]
    public Sprite standardNextSprite;
    [Tooltip("The sprite used for the Next button on the last page")]
    public Sprite doneSprite;

    [Header("Animations")]
    public float fadeDuration = 0.3f; // Time in seconds to fade out and in

    [Header("Transitions")]
    public GameObject smallLoadingPrefab;
    public string nextSceneName = "SampleScene";

    private int _currentIndex = 0;
    private Coroutine _fadeCoroutine;

    void Start()
    {
        // Listen to UI button clicks
        if (nextButton != null)
            nextButton.onClick.AddListener(NextSlide);
            
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousSlide);

        // Check if slides are assigned
        if (tutorialSlides == null || tutorialSlides.Length == 0)
        {
            Debug.LogWarning("Tutorial Manager: No tutorial slides assigned!");
            return;
        }

        // Initialize UI State (but skip animation for the very first page)
        UpdateTutorialUI(false);
    }

    public void NextSlide()
    {
        if (_currentIndex < tutorialSlides.Length - 1)
        {
            _currentIndex++;
            UpdateTutorialUI();
        }
        else
        {
            // Tutorial Finished!
            Debug.Log("[Tutorial] Finished! Starting async transition...");
            StartCoroutine(TransitionToGame());
        }
    }

    private IEnumerator TransitionToGame()
    {
        // 1. Trigger the Loading Overlay
        if (smallLoadingPrefab != null)
        {
            var overlay = smallLoadingPrefab.GetComponent<LoadingOverlay>();
            if (overlay != null)
            {
                overlay.Show();
            }
            else
            {
                smallLoadingPrefab.SetActive(true);
            }
        }

        // 2. Start loading in the background
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(nextSceneName);
        
        // Don't let the scene switch automatically until we're ready
        asyncLoad.allowSceneActivation = false;

        // 3. Make sure the user sees the loading animation for at least 1.2 seconds
        // (This makes the transition feel more premium than a "flicker")
        yield return new WaitForSeconds(1.2f);

        // 4. Wait for the actual load to finish if it's not done yet
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // 5. Finally, enter the game!
        asyncLoad.allowSceneActivation = true;
    }

    public void PreviousSlide()
    {
        if (_currentIndex > 0)
        {
            _currentIndex--;
            UpdateTutorialUI();
        }
    }

    private void UpdateTutorialUI(bool animate = true)
    {
        // 1. Update the Main Picture
        if (tutorialDisplayImage != null && tutorialSlides.Length > 0)
        {
            if (animate && gameObject.activeInHierarchy)
            {
                if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = StartCoroutine(FadeTransition(_currentIndex));
            }
            else
            {
                tutorialDisplayImage.sprite = tutorialSlides[_currentIndex];
                Color c = tutorialDisplayImage.color;
                c.a = 1f;
                tutorialDisplayImage.color = c;
            }
        }

        // 2. Hide or Show Previous Button (Hide on first page)
        if (previousButton != null)
        {
            previousButton.gameObject.SetActive(_currentIndex > 0);
        }

        // 4. Update Next/Done Button Sprite Seamlessly
        if (nextButton != null && standardNextSprite != null && doneSprite != null)
        {
            // If on the last slide, show 'Done', else show 'Next'
            nextButton.image.sprite = (_currentIndex == tutorialSlides.Length - 1) ? doneSprite : standardNextSprite;
        }

        // 3. Update the Dots
        for (int i = 0; i < pageDots.Length; i++)
        {
            if (pageDots[i] != null)
            {
                // If this dot matches the current index, make it Active. Otherwise, Inactive.
                pageDots[i].sprite = (i == _currentIndex) ? dotActiveSprite : dotInactiveSprite;
            }
        }
    }

    private IEnumerator FadeTransition(int newIndex)
    {
        float elapsed = 0f;
        Color c = tutorialDisplayImage.color;
        float halfDuration = fadeDuration / 2f;

        // Fade Out the old image
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / halfDuration);
            tutorialDisplayImage.color = c;
            yield return null;
        }

        // Swap Sprite when fully transparent
        tutorialDisplayImage.sprite = tutorialSlides[newIndex];

        // Fade In the new image
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / halfDuration);
            tutorialDisplayImage.color = c;
            yield return null;
        }

        // Ensure it's fully visible at the end
        c.a = 1f;
        tutorialDisplayImage.color = c;
    }
}

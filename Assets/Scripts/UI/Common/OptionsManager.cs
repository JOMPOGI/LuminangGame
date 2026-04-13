using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    [Header("Music Slider")]
    public Slider musicSlider;
    public Image musicFillImage;
    public TextMeshProUGUI musicPercentageText;

    [Header("SFX Slider")]
    public Slider sfxSlider;
    public Image sfxFillImage;
    public TextMeshProUGUI sfxPercentageText;

    [Header("Mute Buttons")]
    public Button musicMuteButton;
    public Image musicMuteImage;
    public Button sfxMuteButton;
    public Image sfxMuteImage;

    [Header("Volume Sprites")]
    public Sprite volumeUpSprite;
    public Sprite volumeOffSprite;

    [Header("Graphics Buttons")]
    public GameObject lowButtonObj;
    public GameObject medButtonObj;
    public GameObject highButtonObj;

    private Button _lowButton;
    private Button _medButton;
    private Button _highButton;

    [Header("Panels (Disabled)")]
    public GameObject confirmSavePanel; // Keep for inspector safety, but not used
    public GameObject noChangesPanel;


    private void Start()
    {
        // 1. Load & Set Volume
        if (AudioManager.instance != null)
        {
            float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

            musicSlider.value = savedMusic;
            sfxSlider.value = savedSFX;
            
            // Apply (which now auto-saves internally)
            AudioManager.instance.ApplyMusicVolume(savedMusic);
            AudioManager.instance.ApplySFXVolume(savedSFX);
        }

        // 2. Load & Set Graphics Quality - Default to High (2) if no save exists
        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 2);
        savedQuality = Mathf.Clamp(savedQuality, 0, 2);
        
        // Get button components
        if (lowButtonObj != null) _lowButton = lowButtonObj.GetComponent<Button>();
        if (medButtonObj != null) _medButton = medButtonObj.GetComponent<Button>();
        if (highButtonObj != null) _highButton = highButtonObj.GetComponent<Button>();

        // Apply immediately
        ApplyQuality(savedQuality);
        UpdateGraphicsUI(savedQuality);

        // Hide panels if they exist (even though not used anymore)
        if (confirmSavePanel != null) confirmSavePanel.SetActive(false);
        if (noChangesPanel != null) noChangesPanel.SetActive(false);

        // 3. Add Listeners for Sliders
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);

        // 4. Auto-assign Images if Buttons are set
        if (musicMuteButton != null && musicMuteImage == null) musicMuteImage = musicMuteButton.GetComponent<Image>();
        if (sfxMuteButton != null && sfxMuteImage == null) sfxMuteImage = sfxMuteButton.GetComponent<Image>();

        // 4. Add Listeners for Graphics Buttons
        if (_lowButton != null) _lowButton.onClick.AddListener(() => OnGraphicsButtonClicked(0));
        if (_medButton != null) _medButton.onClick.AddListener(() => OnGraphicsButtonClicked(1));
        if (_highButton != null) _highButton.onClick.AddListener(() => OnGraphicsButtonClicked(2));

        // 5. Add Listeners for Mute Buttons
        if (musicMuteButton != null) musicMuteButton.onClick.AddListener(ToggleMusicMute);
        if (sfxMuteButton != null) sfxMuteButton.onClick.AddListener(ToggleSFXMute);

        // Update Text & Icons initially
        UpdateMusicUI(musicSlider.value);
        UpdateSFXUI(sfxSlider.value);
    }

    private void OnMusicSliderChanged(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.ApplyMusicVolume(value); // This now auto-saves

        UpdateMusicUI(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.ApplySFXVolume(value); // This now auto-saves

        UpdateSFXUI(value);
    }

    private void OnGraphicsButtonClicked(int index)
    {
        // Find the button object
        GameObject btnObj = null;
        if (index == 0) btnObj = lowButtonObj;
        if (index == 1) btnObj = medButtonObj;
        if (index == 2) btnObj = highButtonObj;

        // No more scale animation (stay bright, only dim the others)
        ApplyQuality(index);
        UpdateGraphicsUI(index);
        Debug.Log($"[OptionsManager] User clicked: {index}");
    }

    private void ApplyQuality(int index)
    {
        // 1. Unity Built-in Quality Level
        QualitySettings.SetQualityLevel(index, true);

        // 2. Save for next session
        PlayerPrefs.SetInt("GraphicsQuality", index);
        PlayerPrefs.Save();

        // 3. Apply custom Mobile Optimization (Resolution/Shadows)
        if (MobilePerformance.Instance != null)
        {
            MobilePerformance.Instance.ApplyQualitySettings(index);
        }
    }

    private void UpdateGraphicsUI(int selectedIndex)
    {
        // Log the current selection to the console so we can see it
        Debug.Log($"[OptionsManager] Setting UI selection to: {selectedIndex}");

        Color selectedColor = Color.white; // Full brightness for active
        Color unselectedColor = new Color(0.6f, 0.6f, 0.6f, 1f); // Dimmed for inactive

        if (_lowButton != null) _lowButton.targetGraphic.color = (selectedIndex == 0) ? selectedColor : unselectedColor;
        if (_medButton != null) _medButton.targetGraphic.color = (selectedIndex == 1) ? selectedColor : unselectedColor;
        if (_highButton != null) _highButton.targetGraphic.color = (selectedIndex == 2) ? selectedColor : unselectedColor;
    }

    public void ToggleMusicMute()
    {
        float targetVolume = (musicSlider.value > 0) ? 0 : 1.0f;
        musicSlider.value = targetVolume; // This triggers OnMusicSliderChanged
    }

    public void ToggleSFXMute()
    {
        float targetVolume = (sfxSlider.value > 0) ? 0 : 1.0f;
        sfxSlider.value = targetVolume; // This triggers OnSFXSliderChanged
    }

    // --- Helper methods (cleaned up) ---

    [ContextMenu("Auto Find All UI Elements")]
    public void AutoFindButtons()
    {
        // Find existing buttons in children
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            string n = btn.name.ToLower();
            if (n.Contains("low")) lowButtonObj = btn.gameObject;
            if (n.Contains("med")) medButtonObj = btn.gameObject;
            if (n.Contains("high")) highButtonObj = btn.gameObject;
            
            if (n.Contains("music") && n.Contains("mute")) musicMuteButton = btn;
            if (n.Contains("soundeffects") && n.Contains("mute")) sfxMuteButton = btn;
            
            if (n.Contains("music") && n.Contains("slider")) musicSlider = btn.GetComponent<Slider>();
            if (n.Contains("soundeffects") && n.Contains("slider")) sfxSlider = btn.GetComponent<Slider>();
        }

        // Find TMP texts
        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var t in texts)
        {
            string n = t.name.ToLower();
            if (n.Contains("music") && n.Contains("percentage")) musicPercentageText = t;
            if (n.Contains("soundeffects") && n.Contains("percentage")) sfxPercentageText = t;
        }

        // Assign Images
        if (musicMuteButton != null) musicMuteImage = musicMuteButton.GetComponent<Image>();
        if (sfxMuteButton != null) sfxMuteImage = sfxMuteButton.GetComponent<Image>();
        
        Debug.Log("<color=green>[OptionsManager] UI Elements AUTO-FOUND!</color>");
    }

    [ContextMenu("Reset All Settings to Factory Default")]
    public void ResetSettings()
    {
        // Deletes saved volumes and graphics
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("<color=cyan>[OptionsManager] All settings RESET! Please restart the game.</color>");
    }

    private System.Collections.IEnumerator AnimateButtonPress(Transform t, System.Action onComplete)
    {
        float duration = 0.05f;
        Vector3 originalScale = Vector3.one;
        Vector3 pressedScale = new Vector3(0.92f, 0.92f, 1f);

        // Squeeze down
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            t.localScale = Vector3.Lerp(originalScale, pressedScale, elapsed / duration);
            yield return null;
        }

        // Pop back up
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            t.localScale = Vector3.Lerp(pressedScale, originalScale, elapsed / duration);
            yield return null;
        }

        t.localScale = originalScale;
        onComplete?.Invoke();
    }

    private void HidePanel(GameObject panel)
    {
        // This method is no longer used.
    }

    private System.Collections.IEnumerator AnimateScale(Transform t, Vector3 start, Vector3 end, float duration, System.Action onComplete = null)
    {
        // This method is no longer used.
        yield break;
    }

    private void UpdateMusicUI(float value)
    {
        if (musicPercentageText != null)
            musicPercentageText.text = Mathf.RoundToInt(value * 100f).ToString() + "%";

        if (musicFillImage != null)
            musicFillImage.fillAmount = value;

        if (musicMuteImage != null)
            musicMuteImage.sprite = (value > 0) ? volumeUpSprite : volumeOffSprite;
    }

    private void UpdateSFXUI(float value)
    {
        if (sfxPercentageText != null)
            sfxPercentageText.text = Mathf.RoundToInt(value * 100f).ToString() + "%";

        if (sfxFillImage != null)
            sfxFillImage.fillAmount = value;

        if (sfxMuteImage != null)
            sfxMuteImage.sprite = (value > 0) ? volumeUpSprite : volumeOffSprite;
    }
}

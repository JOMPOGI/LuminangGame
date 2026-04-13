using UnityEngine;
using UnityEngine.UI;

public class LookInversionRadioGroup : MonoBehaviour
{
    [Header("Visuals")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    [Header("Standard Option")]
    public Button standardButton;
    public Image standardImage;

    [Header("Inverted Option")]
    public Button invertedButton;
    public Image invertedImage;

    void Start()
    {
        // Load preference (0 = Standard, 1 = Inverted)
        int savedPref = PlayerPrefs.GetInt("InvertLookY", 0);
        UpdateUI(savedPref == 0);

        // Hook up button clicks
        if (standardButton != null) standardButton.onClick.AddListener(() => SetInversion(true));
        if (invertedButton != null) invertedButton.onClick.AddListener(() => SetInversion(false));
    }

    public void SetInversion(bool isStandard)
    {
        // Save preference (0 = Standard, 1 = Inverted)
        PlayerPrefs.SetInt("InvertLookY", isStandard ? 0 : 1);
        PlayerPrefs.Save();

        UpdateUI(isStandard);
        
        Debug.Log($"[Settings] Look Y-Axis set to: {(isStandard ? "Standard" : "Inverted")}");
    }

    private void UpdateUI(bool isStandard)
    {
        // Swap sprites based on selection
        if (standardImage != null) standardImage.sprite = isStandard ? activeSprite : inactiveSprite;
        if (invertedImage != null) invertedImage.sprite = !isStandard ? activeSprite : inactiveSprite;
    }
}

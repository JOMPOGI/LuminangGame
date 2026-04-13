using UnityEngine;
using UnityEngine.UI;

public class VerticalSwitchOption : MonoBehaviour
{
    [Header("Switch Visuals")]
    public Button switchButton;
    [Tooltip("The Image component on your button that changes when clicked.")]
    public Image switchImage; 
    
    [Header("Sprites")]
    public Sprite switchUpSprite;
    public Sprite switchDownSprite;

    private bool _isStandard = true;

    void Start()
    {
        // Load preference (0 = Standard, 1 = Inverted)
        int savedPref = PlayerPrefs.GetInt("InvertLookY", 0);
        _isStandard = (savedPref == 0);

        UpdateVisuals();

        if (switchButton != null)
        {
            switchButton.onClick.AddListener(OnSwitchToggled);
        }
    }

    public void OnSwitchToggled()
    {
        // Toggle the boolean state
        _isStandard = !_isStandard;

        // Save preference (0 = Standard/Up, 1 = Inverted/Down)
        PlayerPrefs.SetInt("InvertLookY", _isStandard ? 0 : 1);
        PlayerPrefs.Save();

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (switchImage == null) return;

        // switch up = Standard (swipe up to look up)
        // switch down = Inverted (swipe up to look down)
        if (_isStandard)
        {
            switchImage.sprite = switchUpSprite;
        }
        else
        {
            switchImage.sprite = switchDownSprite;
        }
    }
}

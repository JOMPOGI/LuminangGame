using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JournalRowItem : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nativePhraseText;
    public TextMeshProUGUI englishMeaningText;
    public Image iconImage;
    public Button rowButton;

    private JournalEntry _entry;
    private JournalBookManager _manager;
    private Color _normalColor;
    private Color _pressedColor;

    public JournalEntry Entry => _entry;

    public void Setup(JournalEntry entry, JournalBookManager manager, Sprite langIcon, Color normalColor, Color pressedColor)
    {
        _entry = entry;
        _manager = manager;
        _normalColor = normalColor;
        _pressedColor = pressedColor;

        if (nativePhraseText != null)
            nativePhraseText.text = entry.phrase;

        if (englishMeaningText != null)
            englishMeaningText.text = entry.meaning;

        if (iconImage != null && langIcon != null)
            iconImage.sprite = langIcon;

        // Initialize button with normal colors (will be updated by SetSelected)
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (rowButton != null)
        {
            ColorBlock cb = rowButton.colors;
            cb.normalColor = isSelected ? _pressedColor : _normalColor;
            cb.selectedColor = isSelected ? _pressedColor : _normalColor;
            cb.pressedColor = _pressedColor;
            cb.highlightedColor = Color.Lerp(_normalColor, Color.white, 0.5f);
            rowButton.colors = cb;

            rowButton.onClick.RemoveAllListeners();
            rowButton.onClick.AddListener(OnClick);
        }
    }

    private void OnClick()
    {
        if (_manager != null && _entry != null)
        {
            _manager.DisplayDetails(_entry);
        }
    }
}

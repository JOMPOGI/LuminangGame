using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A single row in the WordsGroup list.
/// Shows just the icon and the phrase — no details.
/// </summary>
public class WordRowItem : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI phraseText;

    public void Setup(string phrase, Sprite icon)
    {
        if (phraseText != null)
            phraseText.text = phrase;

        if (iconImage != null && icon != null)
            iconImage.sprite = icon;
    }
}

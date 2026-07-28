using UnityEngine;
using TMPro;

/// <summary>
/// A single row in the phrase preview list inside LevelDetailPanel.
/// Shows the English word on the left and the native word on the right.
///
/// Attach to your phrase row prefab inside LevelsGroup > RightGroup > PhraseList.
/// </summary>
public class PhrasePreviewRow : MonoBehaviour
{
    [Tooltip("The English word/phrase label.")]
    public TextMeshProUGUI englishText;

    [Tooltip("The native language word/phrase label (Ilokano or Cebuano).")]
    public TextMeshProUGUI nativeText;

    public void Setup(string english, string native)
    {
        if (englishText != null) englishText.text = english;
        if (nativeText != null) nativeText.text = native;
    }
}

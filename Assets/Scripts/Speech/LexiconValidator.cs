using System;
using UnityEngine;

public class LexiconValidator : MonoBehaviour
{
    public static LexiconValidator Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Checks if the input text contains ONLY words from the predefined dataset.
    /// Returns true if valid, false if it contains external words.
    /// </summary>
    public bool ValidateText(string text)
    {
        if (string.IsNullOrEmpty(text)) return false;

        // Split by common delimiters
        string[] words = text.Split(new[] { ' ', '.', ',', '?', '!', '"', '\'' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var word in words)
        {
            if (!DatasetManager.Instance.IsWordInLexicon(word))
            {
                Debug.LogWarning($"STT Validation Failed: Word '{word}' is not in the dataset lexicon.");
                return false;
            }
        }

        return true;
    }
}

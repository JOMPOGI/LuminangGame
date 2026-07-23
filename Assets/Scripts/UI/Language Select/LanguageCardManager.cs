using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Ilokano and Cebuano language card buttons in the Book.
/// When clicked, it tells BookSelectionManager to flip the page to LevelsGroup.
/// Attach this script to the LanguagesGroup object.
/// </summary>
public class LanguageCardManager : MonoBehaviour
{
    [System.Serializable]
    public class LanguageCard
    {
        public string languageName;
        public Button cardButton;
    }

    [Header("Language Cards")]
    public LanguageCard ilokanoCard;
    public LanguageCard cebuanoCard;

    [Header("Callbacks")]
    [Tooltip("Drag CategoryListManager here so it updates its button colors when a card is selected.")]
    public CategoryListManager categoryListManager;
    [Tooltip("Drag WordsListManager here so the word list updates when a card is selected.")]
    public WordsListManager wordsListManager;

    private void Start()
    {
        if (ilokanoCard?.cardButton != null)
            ilokanoCard.cardButton.onClick.AddListener(() => SelectLanguage("Ilokano"));

        if (cebuanoCard?.cardButton != null)
            cebuanoCard.cardButton.onClick.AddListener(() => SelectLanguage("Cebuano"));
    }

    public void SelectLanguage(string languageName)
    {
        if (categoryListManager != null) categoryListManager.SetActiveLanguage(languageName);
        if (wordsListManager != null) wordsListManager.SetLanguage(languageName);

        if (BookSelectionManager.Instance != null)
        {
            BookSelectionManager.Instance.OpenLevelsGroup();
        }
    }
}

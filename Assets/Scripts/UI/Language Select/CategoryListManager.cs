using UnityEngine;
using System.Collections.Generic;

public class CategoryListManager : MonoBehaviour
{
    [Header("List Settings")]
    public Transform contentParent;
    public GameObject categoryButtonPrefab;

    [Header("Style Options")]
    public Color selectedTextColor = Color.white;
    public Color normalTextColor = new Color(0.2f, 0.15f, 0.1f);

    [Header("Language Specific Colours")]
    public Color ilokanoSelectedBgColor = new Color(0.2f, 0.5f, 1f);
    public Color cebuanoSelectedBgColor = new Color(1f, 0.8f, 0.2f);
    public Color normalBgColor = Color.white;

    [Header("Callbacks")]
    public UnityEngine.Events.UnityEvent<string> onCategorySelected;

    private readonly List<CategoryButton> _spawnedButtons = new List<CategoryButton>();
    private string _selectedCategory = "All";

    private enum Language { Ilokano, Cebuano }
    private Language _activeLanguage = Language.Ilokano;

    // The real game categories
    private readonly List<string> _categories = new List<string>
    {
        "All",
        "Greetings",
        "Expressions of Gratitude",
        "Responses",
        "Identity Expressions",
        "Requests",
        "Directions",
        "Count",
        "Action Verbs",
        "Linking Verbs",
        "Pronouns",
        "Interrogatives"
    };

    private void Start()
    {
        BuildCategoryList();
        StartCoroutine(ForceLayoutRebuild());
    }

    private System.Collections.IEnumerator ForceLayoutRebuild()
    {
        // Wait one frame so the parent layout sets button widths first,
        // then TMP can correctly calculate preferred heights
        yield return null;
        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent as RectTransform);
    }

    private void BuildCategoryList()
    {
        // Clear any placeholder/existing buttons
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        _spawnedButtons.Clear();

        foreach (var categoryName in _categories)
        {
            GameObject newBtnObj = Instantiate(categoryButtonPrefab, contentParent, false);
            CategoryButton catBtn = newBtnObj.GetComponent<CategoryButton>();
            if (catBtn != null)
            {
                catBtn.Setup(categoryName, this);
                _spawnedButtons.Add(catBtn);
            }
        }

        // Default select "All"
        SelectCategory("All");
    }

    public void SelectCategory(string categoryName)
    {
        _selectedCategory = categoryName;
        RefreshButtonStyles();
        onCategorySelected?.Invoke(_selectedCategory);
    }

    // Call this from the language card buttons (Ilokano / Cebuano)
    public void SetActiveLanguage(string languageName)
    {
        if (languageName.Equals("Ilokano", System.StringComparison.OrdinalIgnoreCase))
            _activeLanguage = Language.Ilokano;
        else if (languageName.Equals("Cebuano", System.StringComparison.OrdinalIgnoreCase))
            _activeLanguage = Language.Cebuano;
        else
            _activeLanguage = Language.Ilokano;

        RefreshButtonStyles();
    }

    private Color GetSelectedBgColor()
    {
        return _activeLanguage == Language.Ilokano ? ilokanoSelectedBgColor : cebuanoSelectedBgColor;
    }

    private void RefreshButtonStyles()
    {
        Color selectedBg = GetSelectedBgColor();
        foreach (var btn in _spawnedButtons)
        {
            if (btn != null)
            {
                bool isSelected = (btn.categoryNameText.text == _selectedCategory);
                btn.SetSelected(isSelected, selectedBg, normalBgColor, selectedTextColor, normalTextColor);
            }
        }
    }
}

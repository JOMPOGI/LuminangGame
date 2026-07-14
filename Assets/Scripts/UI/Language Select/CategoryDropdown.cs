using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// A custom grouped dropdown for the Journal tab filter.
/// Renders non-clickable section headers and clickable category items.
/// Built entirely via code — no prefabs required.
/// </summary>
public class CategoryDropdown : MonoBehaviour
{
    // =====================================================
    // Inspector Fields
    // =====================================================

    [Header("Dropdown Toggle")]
    [Tooltip("The button the user clicks to open/close the dropdown.")]
    public Button triggerButton;
    [Tooltip("Text on the trigger button showing current selection.")]
    public TextMeshProUGUI triggerLabel;

    [Header("Dropdown Panel")]
    [Tooltip("The root panel that shows/hides.")]
    public GameObject dropdownPanel;
    [Tooltip("The scroll view content parent where rows are spawned.")]
    public Transform contentParent;

    [Header("Row Sizing")]
    public float headerRowHeight = 36f;
    public float itemRowHeight   = 44f;

    [Header("Header Row Style")]
    public Color headerBgColor       = new Color(0.22f, 0.16f, 0.10f, 1f);
    public Color headerTextColor      = new Color(1f, 0.85f, 0.55f, 1f);
    public int   headerFontSize       = 13;
    public FontStyles headerFontStyle = FontStyles.Bold | FontStyles.UpperCase;
    [Tooltip("Optional custom TMP font for header rows.")]
    public TMP_FontAsset headerFont;

    [Header("Item Row Style")]
    public Color itemBgColor          = new Color(0.96f, 0.89f, 0.73f, 1f);
    public Color itemBgHoverColor     = new Color(0.85f, 0.75f, 0.55f, 1f);
    public Color itemTextColor        = new Color(0.18f, 0.12f, 0.06f, 1f);
    public Color itemSelectedTextColor = new Color(0.55f, 0.25f, 0.05f, 1f);
    public int   itemFontSize         = 14;
    [Tooltip("Optional custom TMP font for item rows.")]
    public TMP_FontAsset itemFont;

    [Header("'All' Row Style")]
    public Color allBgColor           = new Color(0.96f, 0.89f, 0.73f, 1f);
    public Color allTextColor         = new Color(0.18f, 0.12f, 0.06f, 1f);

    [Header("Callbacks")]
    [Tooltip("Optional — wire this to your journal list to filter items.")]
    public UnityEngine.Events.UnityEvent<string> onCategorySelected;

    // =====================================================
    // Category Data (matches LuminangPhrases.json)
    // =====================================================

    // Group name → list of category names within that group
    private static readonly (string group, string[] categories)[] _groups =
    {
        ("CONVERSATIONAL & SOCIAL",  new[] { "Greetings", "Gratitude", "Responses", "Identity" }),
        ("FUNCTIONAL & NAVIGATIONAL",new[] { "Requests", "Directions", "Count" }),
        ("GRAMMATICAL FOUNDATIONS",  new[] { "Action Verbs", "Linking Verbs", "Pronouns", "Interrogatives" }),
    };

    // =====================================================
    // Private State
    // =====================================================

    private bool   _isOpen           = false;
    private string _selectedCategory = "All";

    // Tracks each item row's background image for selection highlighting
    private readonly List<(string category, Image bg, TextMeshProUGUI text)> _itemRows
        = new List<(string, Image, TextMeshProUGUI)>();

    // =====================================================
    // Unity Lifecycle
    // =====================================================

    private void Start()
    {
        if (triggerButton != null)
            triggerButton.onClick.AddListener(ToggleDropdown);

        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);

        BuildRows();
    }

    // =====================================================
    // Open / Close
    // =====================================================

    public void ToggleDropdown()
    {
        _isOpen = !_isOpen;
        if (dropdownPanel != null)
            dropdownPanel.SetActive(_isOpen);
    }

    public void CloseDropdown()
    {
        _isOpen = false;
        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);
    }

    // =====================================================
    // Build Rows Dynamically
    // =====================================================

    private void BuildRows()
    {
        if (contentParent == null) return;

        // Clear any existing children
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        _itemRows.Clear();

        // 1. "All" row
        CreateItemRow("All", allBgColor, allTextColor);

        // 2. Each group header + its category items
        int groupNumber = 1;
        foreach (var (group, categories) in _groups)
        {
            CreateHeaderRow($"{groupNumber}. {group}");

            foreach (var cat in categories)
                CreateItemRow(cat, itemBgColor, itemTextColor);

            groupNumber++;
        }

        // Apply initial selection highlight
        RefreshSelectionVisuals();
    }

    // =====================================================
    // Row Factories
    // =====================================================

    /// <summary>Creates a non-clickable section header row.</summary>
    private void CreateHeaderRow(string label)
    {
        GameObject row = CreateBaseRow(headerRowHeight);

        // Background
        Image bg = row.AddComponent<Image>();
        bg.color = headerBgColor;

        // Text
        TextMeshProUGUI tmp = CreateText(row.transform, label,
            headerFontSize, headerTextColor, headerFontStyle, headerFont);
        tmp.margin = new Vector4(10, 0, 10, 0);
    }

    /// <summary>Creates a clickable category item row.</summary>
    private void CreateItemRow(string categoryName, Color bgColor, Color textColor)
    {
        GameObject row = CreateBaseRow(itemRowHeight);

        // Background image (used for hover/selection tinting)
        Image bg = row.AddComponent<Image>();
        bg.color = bgColor;

        // Indent items (not "All")
        float leftPad = categoryName == "All" ? 10f : 20f;

        // Text
        TextMeshProUGUI tmp = CreateText(row.transform, categoryName,
            itemFontSize, textColor, FontStyles.Normal, itemFont);
        tmp.margin = new Vector4(leftPad, 0, 10, 0);

        // Store reference for selection visuals
        _itemRows.Add((categoryName, bg, tmp));

        // Button
        Button btn = row.AddComponent<Button>();

        // Color block so Unity doesn't override our custom tints
        ColorBlock cb = btn.colors;
        cb.normalColor      = Color.white;
        cb.highlightedColor = new Color(1f, 1f, 1f, 0.85f);
        cb.pressedColor     = new Color(0.8f, 0.8f, 0.8f, 1f);
        cb.selectedColor    = Color.white;
        btn.colors = cb;

        string captured = categoryName;
        btn.onClick.AddListener(() => SelectCategory(captured));
    }

    // =====================================================
    // Selection
    // =====================================================

    private void SelectCategory(string categoryName)
    {
        _selectedCategory = categoryName;

        // Update trigger label
        if (triggerLabel != null)
            triggerLabel.text = categoryName == "All" ? "All Categories" : categoryName;

        RefreshSelectionVisuals();
        CloseDropdown();

        // Fire external event
        onCategorySelected?.Invoke(categoryName);
    }

    private void RefreshSelectionVisuals()
    {
        foreach (var (category, bg, text) in _itemRows)
        {
            bool isSelected = category == _selectedCategory;
            bg.color   = isSelected ? itemBgHoverColor : itemBgColor;
            text.color = isSelected ? itemSelectedTextColor : itemTextColor;
            text.fontStyle = isSelected ? FontStyles.Bold : FontStyles.Normal;
        }
    }

    // =====================================================
    // Helpers
    // =====================================================

    private GameObject CreateBaseRow(float minHeight)
    {
        GameObject row = new GameObject("Row", typeof(RectTransform));
        row.transform.SetParent(contentParent, false);

        // Let the VerticalLayoutGroup drive width; height auto-expands with content
        LayoutElement le = row.AddComponent<LayoutElement>();
        le.minHeight = minHeight;

        // Auto-expand height to fit wrapped text
        ContentSizeFitter csf = row.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        return row;
    }

    private TextMeshProUGUI CreateText(Transform parent, string text,
        int fontSize, Color color, FontStyles style, TMP_FontAsset font = null)
    {
        GameObject go = new GameObject("Label", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text                = text;
        tmp.fontSize            = fontSize;
        tmp.color               = color;
        tmp.fontStyle           = style;
        tmp.alignment           = TextAlignmentOptions.MidlineLeft;
        tmp.enableWordWrapping  = true;          // wrap instead of truncate
        tmp.overflowMode        = TextOverflowModes.Overflow;
        tmp.raycastTarget       = false; // let the parent Button handle clicks

        // Apply custom font if assigned
        if (font != null)
            tmp.font = font;

        return tmp;
    }

    // =====================================================
    // Public API for external use
    // =====================================================

    /// <summary>Returns the currently selected category name ("All" means show everything).</summary>
    public string GetSelectedCategory() => _selectedCategory;

    /// <summary>Programmatically select a category by name.</summary>
    public void SetCategory(string categoryName) => SelectCategory(categoryName);
}

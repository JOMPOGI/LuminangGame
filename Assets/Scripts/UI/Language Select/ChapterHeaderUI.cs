using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChapterHeaderUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    public TextMeshProUGUI chapterNumberText;
    public TextMeshProUGUI chapterTitleText;
    public TextMeshProUGUI progressText;
    public Image chevronIcon;
    public Image chapterNumberBgImage;
    public Image chapterIconImage;
    public Image chapterHeaderBgImage; // The full background of the header row

    [Header("Chevrons")]
    public Sprite collapsedChevron;
    public Sprite expandedChevron;

    private int _chapterIndex;
    private CategoryListManager _manager;

    public void Setup(int chapterIndex, string title, string progress, bool isExpanded, CategoryListManager manager,
                      Sprite numberBgSprite = null, Sprite iconSprite = null, Color? headerBgColor = null)
    {
        _chapterIndex = chapterIndex;
        _manager = manager;

        if (chapterNumberText != null) chapterNumberText.text = chapterIndex.ToString();
        if (chapterTitleText != null) chapterTitleText.text = title;
        if (progressText != null) progressText.text = progress;

        if (numberBgSprite != null && chapterNumberBgImage != null)
            chapterNumberBgImage.sprite = numberBgSprite;

        if (iconSprite != null && chapterIconImage != null)
            chapterIconImage.sprite = iconSprite;

        if (headerBgColor.HasValue && chapterHeaderBgImage != null)
            chapterHeaderBgImage.color = headerBgColor.Value;

        UpdateChevron(isExpanded);
    }

    public void UpdateChevron(bool isExpanded)
    {
        if (chevronIcon != null)
        {
            chevronIcon.sprite = isExpanded ? expandedChevron : collapsedChevron;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_manager != null)
        {
            _manager.ToggleChapter(_chapterIndex);
        }
    }
}

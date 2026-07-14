using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class CategoryButton : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI categoryNameText;
    public GameObject selectedIndicator; // The ">" arrow icon
    public Button button;

    [Header("Height Settings")]
    [Tooltip("Padding added above and below the text when calculating button height.")]
    public float verticalPadding = 16f;
    public float minHeight = 44f;

    private string _categoryName;
    private CategoryListManager _manager;

    public void Setup(string categoryName, CategoryListManager manager)
    {
        _categoryName = categoryName;
        _manager = manager;

        if (categoryNameText != null)
            categoryNameText.text = categoryName;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        // Wait a frame so the parent layout sets our width first,
        // then measure TMP and resize the button height to match.
        StartCoroutine(ResizeAfterLayout());
    }

    private IEnumerator ResizeAfterLayout()
    {
        // Wait for the layout system to set our RectTransform width
        yield return null;

        if (categoryNameText == null) yield break;

        // Now TMP knows its actual width — force it to recalculate
        categoryNameText.ForceMeshUpdate();

        // Tell the Layout system (VLG) how tall this button wants to be.
        // Setting sizeDelta gets overridden by the parent VLG, so we use LayoutElement instead.
        float targetHeight = Mathf.Max(minHeight, categoryNameText.preferredHeight + verticalPadding);
        LayoutElement le = GetComponent<LayoutElement>();
        if (le == null) le = gameObject.AddComponent<LayoutElement>();
        le.preferredHeight = targetHeight;

        // Also notify the parent layout to re-run with the new preferred height
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
    }

    public void SetSelected(bool isSelected, Color selectedBgColor, Color normalBgColor, Color selectedTextColor, Color normalTextColor)
    {
        if (selectedIndicator != null)
            selectedIndicator.SetActive(isSelected);

        if (button != null)
        {
            Image btnImg = button.GetComponent<Image>();
            if (btnImg != null)
            {
                btnImg.color = isSelected ? selectedBgColor : normalBgColor;
            }
        }

        if (categoryNameText != null)
        {
            categoryNameText.color = isSelected ? selectedTextColor : normalTextColor;
        }
    }

    private void OnClick()
    {
        if (_manager != null)
        {
            _manager.SelectCategory(_categoryName);
        }
    }
}

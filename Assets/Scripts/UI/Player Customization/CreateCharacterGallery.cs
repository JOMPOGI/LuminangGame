using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateCharacterGallery : MonoBehaviour
{
    [Header("Character")]
    public OutfitManager characterManager;

    [Header("Prefabs")]
    public GameObject itemFramePrefab;

    [Header("Scroll View Contents")]
    public Transform hairContent;
    public Transform shirtsContent;
    public Transform pantsContent;
    public Transform shoesContent;
    public Transform accessoriesContent;

    [Header("Icons")]
    public Sprite noneIcon;
    public Sprite noneBackground;

    [Header("Detail Panel")]
    public ItemDetailPanelController detailPanel;

    void Start()
    {
        GenerateGallery();
    }

    public void GenerateGallery()
    {
        if (characterManager == null || itemFramePrefab == null) return;

        // 1. Clear existing items
        ClearAllContent();

        // 2. Add "None" button to each slot first — these start selected so nothing is equipped
        List<Toggle> noneToggles = new List<Toggle>();
        foreach (OutfitItem.Slot slot in System.Enum.GetValues(typeof(OutfitItem.Slot)))
        {
            var noneToggle = CreateNoneButton(slot);
            if (noneToggle != null) noneToggles.Add(noneToggle);
        }

        // 3. Find all OutfitItems under the character
        OutfitItem[] items = characterManager.GetComponentsInChildren<OutfitItem>(true);

        foreach (OutfitItem item in items)
        {
            // 4. Find the correct parent content
            Transform targetContent = GetTargetContent(item.slot);
            if (targetContent == null) continue;

            // 5. Instantiate ItemFrame
            GameObject frameObj = Instantiate(itemFramePrefab, targetContent);
            frameObj.name = $"Item_{item.name}";

            // 6. Set Icon
            Transform iconTransform = FindChildRecursive(frameObj.transform, "AssetIcon");
            if (iconTransform == null) iconTransform = FindChildRecursive(frameObj.transform, "ItemIcon");

            Image iconImage = iconTransform?.GetComponent<Image>();
            if (iconImage != null)
            {
                if (item.icon != null)
                {
                    iconImage.sprite = item.icon;
                    iconImage.color = Color.white;
                }
                else
                {
                    iconImage.color = new Color(0, 0, 0, 0);
                }
            }

            // 7. Set item name label if it exists on the frame
            Transform nameLabelTransform = FindChildRecursive(frameObj.transform, "ItemName");
            TMPro.TextMeshProUGUI nameLabel = nameLabelTransform?.GetComponent<TMPro.TextMeshProUGUI>();
            if (nameLabel != null)
                nameLabel.text = string.IsNullOrEmpty(item.itemName) ? item.name : item.itemName;

            // 8. Setup Toggle — clicking opens the detail panel (NO auto-equip)
            Toggle toggle = frameObj.GetComponent<Toggle>();
            if (toggle != null)
            {
                ToggleGroup group = targetContent.GetComponent<ToggleGroup>();
                if (group != null) toggle.group = group;

                // Not selected by default
                toggle.isOn = false;

                toggle.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn && detailPanel != null)
                        detailPanel.ShowItem(item);
                });
            }
        }

        // 9. Force all "None" toggles ON at start — nothing equipped, panel hidden
        foreach (var t in noneToggles)
        {
            t.isOn = true;
        }
    }

    private Toggle CreateNoneButton(OutfitItem.Slot slot)
    {
        Transform targetContent = GetTargetContent(slot);
        if (targetContent == null) return null;

        GameObject frameObj = Instantiate(itemFramePrefab, targetContent);
        frameObj.name = $"None_{slot}";

        // Set background sprite
        Image backgroundImage = frameObj.GetComponent<Image>();
        if (backgroundImage == null) backgroundImage = FindChildRecursive(frameObj.transform, "Background")?.GetComponent<Image>();

        if (backgroundImage != null && noneBackground != null)
            backgroundImage.sprite = noneBackground;

        // Set icon
        Transform iconTransform = FindChildRecursive(frameObj.transform, "AssetIcon");
        if (iconTransform == null) iconTransform = FindChildRecursive(frameObj.transform, "ItemIcon");

        Image iconImage = iconTransform?.GetComponent<Image>();
        if (iconImage != null)
        {
            if (noneIcon != null)
            {
                iconImage.sprite = noneIcon;
                iconImage.color = Color.white;
            }
            else
            {
                iconImage.color = new Color(0, 0, 0, 0);
            }
        }

        // Setup Toggle — selecting None unequips and hides the detail panel
        Toggle toggle = frameObj.GetComponent<Toggle>();
        if (toggle != null)
        {
            ToggleGroup group = targetContent.GetComponent<ToggleGroup>();
            if (group != null) toggle.group = group;

            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    characterManager.Unequip(slot);
                    if (detailPanel != null)
                        detailPanel.HidePanel();
                }
            });
        }
        return toggle;
    }

    private Transform GetTargetContent(OutfitItem.Slot slot)
    {
        return slot switch
        {
            OutfitItem.Slot.Hair        => hairContent,
            OutfitItem.Slot.Top         => shirtsContent,
            OutfitItem.Slot.Bottom      => pantsContent,
            OutfitItem.Slot.Shoes       => shoesContent,
            OutfitItem.Slot.Accessories => accessoriesContent,
            _                           => null
        };
    }

    private void ClearAllContent()
    {
        ClearContent(hairContent);
        ClearContent(shirtsContent);
        ClearContent(pantsContent);
        ClearContent(shoesContent);
        ClearContent(accessoriesContent);
    }

    private void ClearContent(Transform content)
    {
        if (content == null) return;
        foreach (Transform child in content) Destroy(child.gameObject);
    }

    private Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}

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

    void Start()
    {
        GenerateGallery();
    }

    public void GenerateGallery()
    {
        if (characterManager == null || itemFramePrefab == null) return;

        // 1. Clear existing items
        ClearAllContent();

        // 2. Add "None" button to each slot first
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

            // 6. Set Icon (from the Sprite you assigned)
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
                    // If no icon, hide the image or show a placeholder
                    iconImage.color = new Color(0, 0, 0, 0); 
                }
            }

            // 7. Setup Toggle Event
            Toggle toggle = frameObj.GetComponent<Toggle>();
            if (toggle != null)
            {
                ToggleGroup group = targetContent.GetComponent<ToggleGroup>();
                if (group != null) toggle.group = group;

                // Ensure clothing items are NOT selected by default
                toggle.isOn = false;

                toggle.onValueChanged.AddListener((isOn) => {
                    if (isOn) characterManager.Equip(item);
                });
            }
        }

        // 8. FINAL STEP: Force all "None" buttons to be selected last
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

        // Set the Background if a special "None Background" is provided
        Image backgroundImage = frameObj.GetComponent<Image>();
        if (backgroundImage == null) backgroundImage = FindChildRecursive(frameObj.transform, "Background")?.GetComponent<Image>();
        
        if (backgroundImage != null && noneBackground != null)
        {
            backgroundImage.sprite = noneBackground;
        }

        // Set Icon to the "None" sprite
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
                iconImage.color = new Color(0, 0, 0, 0); // Transparent if no icon
            }
        }

        // Setup Toggle Event to Unequip
        Toggle toggle = frameObj.GetComponent<Toggle>();
        if (toggle != null)
        {
            ToggleGroup group = targetContent.GetComponent<ToggleGroup>();
            if (group != null) toggle.group = group;

            toggle.onValueChanged.AddListener((isOn) => {
                if (isOn) characterManager.Unequip(slot);
            });
        }
        return toggle;
    }

    private Transform GetTargetContent(OutfitItem.Slot slot)
    {
        return slot switch
        {
            OutfitItem.Slot.Hair => hairContent,
            OutfitItem.Slot.Top => shirtsContent,
            OutfitItem.Slot.Bottom => pantsContent,
            OutfitItem.Slot.Shoes => shoesContent,
            OutfitItem.Slot.Accessories => accessoriesContent,
            _ => null
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

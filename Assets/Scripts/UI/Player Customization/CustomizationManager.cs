using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CustomizationManager : MonoBehaviour
{
    [Header("Manager References")]
    public OutfitManager characterManager;
    public TMPro.TMP_InputField usernameField;
    public TMPro.TextMeshProUGUI reminderText;
    public Button changeButton;
    public Image usernameIcon;

    [Header("Prefab Settings")]
    public GameObject itemFramePrefab;
    
    [Header("Categories")]
    public List<CategoryFolder> categories = new List<CategoryFolder>();

    [Header("Default Sprites")]
    public Sprite noneIcon;
    public Sprite noneBackground;

    [Header("Background Sprites")]
    [Tooltip("Background sprite when item is selected")]
    public Sprite activeBackground;
    [Tooltip("Background sprite when item is not selected")]
    public Sprite inactiveBackground;

    [System.Serializable]
    public class CategoryFolder
    {
        public string categoryName;
        public OutfitItem.Slot slot;
        public Transform contentParent;
    }

    async void Start()
    {
        // 1. Ensure the profile is loaded
        if (UserProfileManager.Instance != null && UserProfileManager.Instance.CurrentProfile == null)
        {
            Debug.Log("[Customization] Profile null, fetching now...");
            await UserProfileManager.Instance.FetchProfile();
        }

        // 2. Set the username and handle the 30-day cooldown
        if (usernameField != null && UserProfileManager.Instance != null && UserProfileManager.Instance.CurrentProfile != null)
        {
            var profile = UserProfileManager.Instance.CurrentProfile;
            usernameField.text = profile.Username;
            
            // Check for 30-day cooldown
            if (profile.UsernameFinalizedAt.HasValue)
            {
                System.TimeSpan timeSinceFinalized = System.DateTime.UtcNow - profile.UsernameFinalizedAt.Value;
                int daysRemaining = 30 - timeSinceFinalized.Days;

                if (daysRemaining > 0)
                {
                    usernameField.interactable = false;
                    Debug.Log($"[Customization] Username cooldown active. {daysRemaining} days remaining.");
                    
                    if (reminderText != null)
                    {
                        reminderText.text = $"You can change your username again in <color=red>{daysRemaining} days</color>.";
                    }

                    // LOCK BUTTON AND ICON
                    if (changeButton != null)
                    {
                        changeButton.interactable = false;
                        Animator anim = changeButton.GetComponent<Animator>();
                        if (anim != null) anim.enabled = false;

                        // Gray out the button image itself
                        Image btnImg = changeButton.GetComponent<Image>();
                        if (btnImg != null) btnImg.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    }

                    if (usernameIcon != null)
                    {
                        usernameIcon.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gray and semi-transparent
                    }
                }
                else
                {
                    if (reminderText != null)
                    {
                        reminderText.text = "One change allowed every 30 days. Choose wisely!";
                    }

                    if (changeButton != null)
                    {
                        changeButton.interactable = true;
                        Animator anim = changeButton.GetComponent<Animator>();
                        if (anim != null) anim.enabled = true;

                        Image btnImg = changeButton.GetComponent<Image>();
                        if (btnImg != null) btnImg.color = Color.white;
                    }

                    if (usernameIcon != null)
                    {
                        usernameIcon.color = Color.white;
                    }
                }
            }
            else
            {
                // Never finalized? Show the policy reminder
                if (reminderText != null)
                {
                    reminderText.text = "One change allowed every 30 days. Choose wisely!";
                }

                if (changeButton != null)
                {
                    changeButton.interactable = true;
                    Animator anim = changeButton.GetComponent<Animator>();
                    if (anim != null) anim.enabled = true;

                    Image btnImg = changeButton.GetComponent<Image>();
                    if (btnImg != null) btnImg.color = Color.white;
                }

                if (usernameIcon != null)
                {
                    usernameIcon.color = Color.white;
                }
            }
            
            Debug.Log("[Customization] Set username to: " + usernameField.text);
        }

        // 2. Load what the character is ALREADY wearing from the database
        if (UserProfileManager.Instance != null && characterManager != null)
        {
            var equippedData = UserProfileManager.Instance.GetEquippedOutfitData();
            if (equippedData != null)
            {
                characterManager.LoadOutfit(equippedData);
            }
        }

        // 3. Fetch inventory and build the UI
        await InitializeGallery();
    }

    public async System.Threading.Tasks.Task InitializeGallery()
    {
        if (characterManager == null || itemFramePrefab == null) return;

        // Fetch owned items from database
        List<string> ownedItems = await FetchOwnedInventory();

        foreach (var category in categories)
        {
            GenerateCategory(category, ownedItems);
        }
    }

    private async System.Threading.Tasks.Task<List<string>> FetchOwnedInventory()
    {
        List<string> owned = new List<string>();
        try
        {
            var user = SupabaseManager.Instance.client.Auth.CurrentUser;
            if (user == null) return owned;

            var response = await SupabaseManager.Instance.client
                .From<InventoryModel>()
                .Where(x => x.UserId == user.Id)
                .Get();

            if (response != null && response.Models != null)
            {
                foreach (var item in response.Models)
                {
                    owned.Add(item.ItemName);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("[Customization] Error fetching inventory: " + ex.Message);
        }
        return owned;
    }

    private void GenerateCategory(CategoryFolder category, List<string> ownedItems)
    {
        if (category.contentParent == null) return;

        // Clear existing
        foreach (Transform child in category.contentParent)
            Destroy(child.gameObject);

        ToggleGroup group = category.contentParent.GetComponent<ToggleGroup>();

        // Add "None" button first
        Toggle noneToggle = CreateItem(category, "None", noneIcon, noneBackground, null, group);

        // Add outfit items
        OutfitItem[] allItems = characterManager.GetComponentsInChildren<OutfitItem>(true);
        Toggle selectedToggle = null;

        // Check what the character is currently wearing
        EquippedOutfitData current = characterManager.GetEquippedNames();
        string equippedName = GetEquippedNameForSlot(current, category.slot);

        foreach (var item in allItems)
        {
            // ONLY show items that the player actually OWNS in their inventory
            if (item.slot == category.slot && ownedItems.Contains(item.name))
            {
                Toggle t = CreateItem(category, item.name, item.icon, null, item, group);
                if (item.name == equippedName)
                    selectedToggle = t;
            }
        }

        // Select the right toggle
        if (selectedToggle == null)
            selectedToggle = noneToggle;

        selectedToggle.isOn = true;
    }

    private Toggle CreateItem(CategoryFolder category, string itemName, Sprite icon, Sprite bg, OutfitItem item, ToggleGroup group)
    {
        GameObject frameObj = Instantiate(itemFramePrefab, category.contentParent);
        frameObj.name = itemName;

        // ============================================================
        // FIX: Remove rogue Toggle components on child objects.
        // The prefab has a Toggle on the Checkmark child that fights
        // with the root Toggle. Destroy all Toggles except the root one.
        // But first, grab the SelectedSprite from it (that's the active background).
        // ============================================================
        Toggle rootToggle = frameObj.GetComponent<Toggle>();
        Toggle[] allToggles = frameObj.GetComponentsInChildren<Toggle>(true);
        
        Sprite activeBg = activeBackground;
        Sprite inactiveBg = inactiveBackground;
        
        foreach (var t in allToggles)
        {
            if (t != rootToggle)
            {
                // Grab the active background sprite from the rogue toggle before destroying it
                if (activeBg == null && t.spriteState.selectedSprite != null)
                    activeBg = t.spriteState.selectedSprite;
                Destroy(t);
            }
        }

        rootToggle.group = group;
        rootToggle.isOn = false; // Start all OFF, we select the right one later

        // Find child references
        Transform bgTransform = frameObj.transform.Find("Background");
        Image bgImage = bgTransform != null ? bgTransform.GetComponent<Image>() : null;

        // Auto-detect inactive background sprite from the Background Image's current sprite
        if (inactiveBg == null && bgImage != null)
            inactiveBg = bgImage.sprite;

        Transform checkTransform = FindChildRecursive(frameObj.transform, "Checkmark");
        GameObject checkObj = checkTransform != null ? checkTransform.gameObject : null;

        Transform iconTransform = FindChildRecursive(frameObj.transform, "ItemIcon");
        if (iconTransform == null) iconTransform = FindChildRecursive(frameObj.transform, "AssetIcon");

        // Set background sprite for "None" button
        if (bg != null && bgImage != null) bgImage.sprite = bg;

        // Set icon
        if (iconTransform != null)
        {
            Image iconImg = iconTransform.GetComponent<Image>();
            if (iconImg != null && icon != null)
            {
                iconImg.sprite = icon;
                iconImg.color = Color.white;
            }
        }

        // Start with checkmark hidden
        if (checkObj != null) checkObj.SetActive(false);

        // Handle selection changes - this controls EVERYTHING visually
        rootToggle.onValueChanged.AddListener((isOn) => {
            // Checkmark
            if (checkObj != null) checkObj.SetActive(isOn);

            // Background sprite
            if (bgImage != null)
            {
                if (isOn && activeBg != null) bgImage.sprite = activeBg;
                else if (!isOn && inactiveBg != null) bgImage.sprite = inactiveBg;
            }

            // Equip/Unequip
            if (isOn)
            {
                if (item == null) characterManager.Unequip(category.slot);
                else characterManager.Equip(item);
            }
        });

        return rootToggle;
    }

    private string GetEquippedNameForSlot(EquippedOutfitData data, OutfitItem.Slot slot)
    {
        switch (slot)
        {
            case OutfitItem.Slot.Hair: return data.hair;
            case OutfitItem.Slot.Top: return data.top;
            case OutfitItem.Slot.Bottom: return data.bottom;
            case OutfitItem.Slot.Shoes: return data.shoes;
            case OutfitItem.Slot.Accessories: return data.accessories;
            default: return "";
        }
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

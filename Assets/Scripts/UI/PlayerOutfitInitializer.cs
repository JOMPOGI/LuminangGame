using UnityEngine;

public class PlayerOutfitInitializer : MonoBehaviour
{
    [Header("Outfit Settings")]
    [Tooltip("The OutfitManager on the Player character. If null, will try to find one in children.")]
    public OutfitManager playerOutfitManager;

    void Start()
    {
        InitializeOutfit();
    }

    public void InitializeOutfit()
    {
        if (playerOutfitManager == null)
        {
            playerOutfitManager = GetComponentInChildren<OutfitManager>();
        }

        if (playerOutfitManager == null)
        {
            Debug.LogWarning("[OutfitInitializer] No OutfitManager found on player!");
            return;
        }

        if (UserProfileManager.Instance != null)
        {
            var data = UserProfileManager.Instance.GetEquippedOutfitData();
            if (data != null)
            {
                Debug.Log($"[OutfitInitializer] Successfully found saved outfit: Hair={data.hair}, Top={data.top}, Bottom={data.bottom}");
                playerOutfitManager.LoadOutfit(data);
            }
            else
            {
                Debug.LogWarning("[OutfitInitializer] No saved outfit found in profile or failed to parse. Check if data was saved correctly in Character Creation.");
            }
        }
        else
        {
            Debug.LogWarning("[OutfitInitializer] UserProfileManager not found!");
        }
    }
}

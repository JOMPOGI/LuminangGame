using UnityEngine;

public class PortraitBooth : MonoBehaviour
{
    [Header("Setup")]
    public Camera portraitCamera;
    public Transform characterSpawnPoint;
    public GameObject characterPrefab;
    
    [Header("Output")]
    public RenderTexture portraitTexture;

    private GameObject spawnedCharacter;
    private OutfitManager outfitManager;

    public void SetupPortrait(EquippedOutfitData outfitData)
    {
        // 1. Only spawn if we actually have a prefab (Optional)
        if (characterPrefab != null)
        {
            if (spawnedCharacter != null) Destroy(spawnedCharacter);
            spawnedCharacter = Instantiate(characterPrefab, characterSpawnPoint.position, characterSpawnPoint.rotation, characterSpawnPoint);
            SetLayerRecursively(spawnedCharacter, gameObject.layer);
            
            outfitManager = spawnedCharacter.GetComponent<OutfitManager>();
            if (outfitManager != null && outfitData != null)
            {
                outfitManager.LoadOutfit(outfitData);
            }
        }
        else
        {
            Debug.Log("[PortraitBooth] No prefab assigned, capturing the existing character in the scene.");
        }

        // 2. Capture the current view
        if (portraitCamera != null && portraitTexture != null)
        {
            portraitCamera.targetTexture = portraitTexture;
            portraitCamera.aspect = 1.0f; // Force 1:1 ratio
            portraitCamera.Render(); // Force a single render
        }
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void OnDestroy()
    {
        if (portraitTexture != null) portraitTexture.Release();
    }
}

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// One-time editor tool to swap the PlayerArmature prefab in CreateCharacterScene
/// and AUTOMATICALLY set up the OutfitManager system.
/// Run via menu: Tools > Swap Character Preview Model
/// </summary>
public class SwapCharacterPreviewModel : Editor
{
    [MenuItem("Tools/Swap Character Preview Model")]
    static void Swap()
    {
        // 1. Make sure CreateCharacterScene is open
        var scene = EditorSceneManager.GetActiveScene();
        if (!scene.name.Contains("CreateCharacter"))
        {
            EditorUtility.DisplayDialog(
                "Wrong Scene",
                "Please open the CreateCharacterScene first, then run this tool again.",
                "OK");
            return;
        }

        // 2. Find CharacterPreviewSetup
        GameObject setup = GameObject.Find("CharacterPreviewSetup");
        if (setup == null)
        {
            EditorUtility.DisplayDialog("Error", "Could not find 'CharacterPreviewSetup' in the scene.", "OK");
            return;
        }

        // 3. Find and destroy old PlayerArmature
        Transform oldModel = setup.transform.Find("PlayerArmature");
        if (oldModel != null)
        {
            Debug.Log($"[SwapModel] Destroying old PlayerArmature");
            Undo.DestroyObjectImmediate(oldModel.gameObject);
        }

        // 4. Load the new FBX model
        string fbxPath = "Assets/CharacterAssets/Player/playerMasterFBX.fbx";
        GameObject fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
        if (fbxAsset == null)
        {
            EditorUtility.DisplayDialog("Error", $"Could not load FBX at: {fbxPath}", "OK");
            return;
        }

        // 5. Instantiate the new model under CharacterPreviewSetup
        GameObject newModel = (GameObject)PrefabUtility.InstantiatePrefab(fbxAsset, setup.transform);
        Undo.RegisterCreatedObjectUndo(newModel, "Swap Character Preview Model");
        newModel.name = "PlayerArmature";

        // 6. Match the transform settings
        newModel.transform.localPosition = Vector3.zero;
        newModel.transform.localRotation = Quaternion.Euler(0f, 184.31f, 0f);
        newModel.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);

        // 7. Set layer to 6 (CharacterPreview) on all children recursively
        SetLayerRecursive(newModel, 6);

        // 8. Disable gameplay components
        var cc = newModel.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        DisableComponentByName(newModel, "ThirdPersonController");
        DisableComponentByName(newModel, "StarterAssetsInputs");
        DisableComponentByName(newModel, "PlayerInput");
        DisableComponentByName(newModel, "BasicRigidBodyPush");

        // 9. SET UP OUTFIT MANAGER
        OutfitManager manager = newModel.AddComponent<OutfitManager>();
        
        // Link body parts
        manager.head = FindChildRecursive(newModel.transform, "face")?.gameObject;
        manager.arms = FindChildRecursive(newModel.transform, "arms")?.gameObject;
        manager.torso = FindChildRecursive(newModel.transform, "torso")?.gameObject;
        manager.legs = FindChildRecursive(newModel.transform, "legs")?.gameObject;
        manager.feet = FindChildRecursive(newModel.transform, "feet")?.gameObject;
        manager.hands = FindChildRecursive(newModel.transform, "hands")?.gameObject;
        manager.hips = FindChildRecursive(newModel.transform, "hips")?.gameObject;
        
        Debug.Log("[SwapModel] OutfitManager added and body parts linked.");

        // 10. SETUP OUTFIT ITEMS
        // We find all children that ARE NOT body parts or Armature
        string[] bodyPartNames = { "face", "arms", "torso", "legs", "feet", "hands", "hips", "Armature", "face.meta", "Outfits" };
        
        foreach (Transform child in newModel.transform)
        {
            bool isBodyPart = false;
            foreach (string bp in bodyPartNames) { if (child.name.Equals(bp, System.StringComparison.OrdinalIgnoreCase)) isBodyPart = true; }

            if (!isBodyPart)
            {
                // Add OutfitItem component
                OutfitItem item = child.gameObject.AddComponent<OutfitItem>();
                
                // Guess Slot based on name
                string lowerName = child.name.ToLower();
                if (lowerName.Contains("hair")) item.slot = OutfitItem.Slot.Hair;
                else if (lowerName.Contains("shirt") || lowerName.Contains("barong") || lowerName.Contains("top")) item.slot = OutfitItem.Slot.Top;
                else if (lowerName.Contains("trouser") || lowerName.Contains("pants") || lowerName.Contains("jeans") || lowerName.Contains("shorts")) item.slot = OutfitItem.Slot.Bottom;
                else if (lowerName.Contains("shoe") || lowerName.Contains("heel") || lowerName.Contains("boots")) item.slot = OutfitItem.Slot.Shoes;
                else if (lowerName.Contains("accessory") || lowerName.Contains("item")) item.slot = OutfitItem.Slot.Accessories;
                
                // Hide the mesh by default
                child.gameObject.SetActive(false);
                Debug.Log($"[SwapModel] Set up item: {child.name} (Slot: {item.slot})");
            }
        }

        // 11. Reconnect CharacterPreviewSpinner
        var spinner = Object.FindFirstObjectByType<CharacterPreviewSpinner>();
        if (spinner != null)
        {
            spinner.characterTransform = newModel.transform;
            EditorUtility.SetDirty(spinner);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorUtility.DisplayDialog("Done!", "Character Swapped and Outfit System Setup Complete!", "OK");
    }

    static void SetLayerRecursive(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform) SetLayerRecursive(child.gameObject, layer);
    }

    static void DisableComponentByName(GameObject obj, string typeName)
    {
        foreach (var comp in obj.GetComponents<MonoBehaviour>())
        {
            if (comp != null && comp.GetType().Name == typeName) comp.enabled = false;
        }
    }

    static Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase)) return child;
            Transform found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }
}

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquippedOutfitData
{
    public string hair;
    public string top;
    public string bottom;
    public string shoes;
    public string accessories;

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>
        {
            { "Hair", hair },
            { "Top", top },
            { "Bottom", bottom },
            { "Shoes", shoes },
            { "Accessories", accessories }
        };
    }
}

public class OutfitManager : MonoBehaviour
{
    [Header("Body parts")]
    public GameObject head;
    public GameObject arms;
    public GameObject torso;
    public GameObject legs;
    public GameObject feet;
    public GameObject hands;
    public GameObject hips;

    private readonly Dictionary<OutfitItem.Slot, OutfitItem> equipped = new();

    public void Equip(OutfitItem item)
    {
        if (item == null) 
        {
            Debug.LogWarning("[OutfitManager] Attempted to equip a null item!");
            return;
        }

        Debug.Log($"[OutfitManager] Equipping item: {item.name} in slot: {item.slot}");

        // REMOVED: item.gameObject.layer = 6; 
        // This was hiding items from the Main Camera in the actual game!

        Unequip(item.slot);

        item.gameObject.SetActive(true);
        
        foreach (var renderer in item.GetComponentsInChildren<Renderer>(true))
        {
            renderer.enabled = true;
        }

        equipped[item.slot] = item;

        RefreshBodyVisibility();
    }

    public void Unequip(OutfitItem.Slot slot)
    {
        if (equipped.TryGetValue(slot, out var existing) && existing != null)
        {
            Debug.Log($"[OutfitManager] Unequipping existing item: {existing.name} from slot: {slot}");
            existing.gameObject.SetActive(false);
        }
        equipped.Remove(slot);
        RefreshBodyVisibility();
    }

    private void RefreshBodyVisibility()
    {
        bool showHead = true, showArms = true, showTorso = true, showLegs = true, showFeet = true, showHands = true, showHips = true;

        foreach (var kv in equipped)
        {
            var item = kv.Value;
            if (item == null) continue;

            if (item.hideHead) showHead = false;
            if (item.hideArms) showArms = false;
            if (item.hideTorso) showTorso = false;
            if (item.hideLegs) showLegs = false;
            if (item.hideFeet) showFeet = false;
            if (item.hideHands) showHands = false; 
            if (item.hideHips) showHips = false;
        }

        if (head) head.SetActive(showHead);
        if (arms) arms.SetActive(showArms);
        if (torso) torso.SetActive(showTorso);
        if (legs) legs.SetActive(showLegs);
        if (feet) feet.SetActive(showFeet);
        if (hands) hands.SetActive(showHands);
        if (hips) hips.SetActive(showHips);
    }

    public EquippedOutfitData GetEquippedNames()
    {
        var data = new EquippedOutfitData();
        if (equipped.TryGetValue(OutfitItem.Slot.Hair, out var h)) data.hair = h.gameObject.name;
        if (equipped.TryGetValue(OutfitItem.Slot.Top, out var t)) data.top = t.gameObject.name;
        if (equipped.TryGetValue(OutfitItem.Slot.Bottom, out var b)) data.bottom = b.gameObject.name;
        if (equipped.TryGetValue(OutfitItem.Slot.Shoes, out var s)) data.shoes = s.gameObject.name;
        if (equipped.TryGetValue(OutfitItem.Slot.Accessories, out var a)) data.accessories = a.gameObject.name;
        return data;
    }

    public void LoadOutfit(EquippedOutfitData data)
    {
        if (data == null) return;

        // 1. Find all potential OutfitItems OR just objects under the character
        // We'll search by name to be extra safe if scripts are missing
        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);
        Dictionary<string, GameObject> objectCache = new();
        foreach (var t in allTransforms)
        {
            objectCache[t.name] = t.gameObject;
        }

        // 2. Equip them based on the saved names
        var dict = data.ToDictionary();
        foreach (var kv in dict)
        {
            Debug.Log($"[OutfitManager] Processing saved slot {kv.Key}: '{kv.Value}'");
            
            if (string.IsNullOrEmpty(kv.Value))
            {
                if (System.Enum.TryParse<OutfitItem.Slot>(kv.Key, out var slot))
                {
                    Unequip(slot);
                }
                continue;
            }

            if (objectCache.TryGetValue(kv.Value, out var itemObj))
            {
                var itemScript = itemObj.GetComponent<OutfitItem>();
                if (itemScript != null)
                {
                    Debug.Log($"[OutfitManager] Found item {kv.Value}. Equipping...");
                    Equip(itemScript);
                }
                else
                {
                    // If no script, at least turn the object on!
                    Debug.LogWarning($"[OutfitManager] Found object {kv.Value} but it's missing the OutfitItem script. Turning it on anyway.");
                    itemObj.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning($"[OutfitManager] Could not find any object named '{kv.Value}' on the player character!");
            }
        }
    }
}
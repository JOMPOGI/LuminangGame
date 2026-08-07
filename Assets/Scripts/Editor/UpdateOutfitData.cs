#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class UpdateOutfitData : MonoBehaviour
{
    [MenuItem("Tools/Update Outfit Data")]
    public static void UpdateData()
    {
        // Data from your image
        var data = new Dictionary<string, (string name, string desc)>
        {
            // Pants/Skirts
            { "beigeTrouser", ("Lino Trousers", "Lightweight trousers that keep you comfy wherever you go.") },
            { "blackTrouser", ("Black Trousers", "A clean and versatile pair for any occasion.") },
            { "cargoShorts", ("Lakbay Shorts", "Packed with pockets for every little adventure.") },
            { "denimJeans", ("Maong Pants", "A timeless favorite made for everyday wear.") },
            { "fantasyPants", ("Mandirigma Greaves", "Rugged legwear fit for a brave adventurer.") },
            { "pinkSkirt", ("Rosas Skirt", "A sweet and playful skirt that brightens any day.") },

            // Shirts/Dresses
            { "AdvShirt", ("Lakbay Vest", "Made for little explorers who are always ready for the next adventure.") },
            { "baro'tSaya", ("Baro't Saya", "A timeless Filipina outfit that celebrates grace and tradition.") },
            { "barong", ("Barong Tagalog", "A proudly Filipino classic, perfect for special occasions.") },
            { "blackShirt", ("Black Tee", "Simple, comfy, and goes with just about anything.") },
            { "bunnyShirt", ("Kuneho Tee", "A soft little shirt with an extra dose of cuteness.") },
            { "guayaberaShirt", ("Guayabera", "A breezy embroidered shirt with timeless island charm.") },
            { "purpleCatShirt", ("Muning Tee", "For cat lovers who carry a little mischief wherever they go.") },
            { "purpleDress", ("Maharlika Dress", "A regal outfit inspired by stories of old Filipino kingdoms.") },
            { "redFantasyDress", ("Diwata Dress", "A magical dress that feels straight out of a Filipino folktale.") },
            { "SayaDress", ("Filipiniana", "An elegant Filipiniana outfit that celebrates Filipino heritage with pride.") },
            { "tuckedShirt", ("Polo Blanco", "A neat and polished look that's always in style.") },
            { "whiteShirt", ("White Tee", "The everyday favorite that never goes out of fashion.") },

            // Shoes
            { "beigeHighHeel", ("Mutya Heels", "A graceful pair that adds a touch of elegance to any outfit.") },
            { "blackShoe", ("Black Loafers", "A dependable pair that's ready for school, work, or adventure.") },
            { "blackSneakers", ("Black Sneakers", "Made for busy days and endless exploring.") },
            { "brownBoots", ("Brown Boots", "Sturdy boots built for every trail and every journey.") },
            { "brownShoe", ("Brown Oxfords", "A classic pair with a warm, timeless look.") },
            { "flatSandalsBrown", ("Bakya Sandals", "Light, comfy sandals perfect for sunny days.") },
            { "lacedBrownBoots", ("Lakbay Boots", "Lace up and head out for your next adventure.") },

            // Hairs
<<<<<<< HEAD
            { "blackHairShort", ("Classic Bob", "A neat bob that never goes out of style.") },
            { "blondHair", ("Golden Layers", "Soft layered hair with a bright, cheerful look.") },
            { "longBlackHair", ("Straight Locks", "Long, sleek hair that's simple and elegant.") },
            { "pinkHair", ("Pink Twin Braids", "A playful braided style full of personality.") },
            { "purpBlackHairShort", ("Side Fringe", "A short cut with a stylish side-swept fringe.") },
            { "shortBlackHair", ("Textured Crop", "A clean, textured haircut for an everyday look.") },
            { "shortBrownHair", ("Tousled Cut", "A slightly messy style with effortless charm.") },
            { "shortMochaHair", ("Soft Layers", "Light layers that create a gentle, relaxed look.") },
            { "spikyBlonde", ("Spiky Cut", "A bold hairstyle with plenty of attitude.") },
=======
            { "blackHairShort", ("Black Hair Short", "A neat bob that never goes out of style.", 100) },
            { "blondHair", ("Golden Layers", "Soft layered hair with a bright, cheerful look.", 250) },
            { "longBlackHair", ("Long Black Hair", "Long, sleek hair that's simple and elegant.", 150) },
            { "pinkHair", ("Pink Twin Braids", "A playful braided style full of personality.", 600) },
            { "purpBlackHairShort", ("Side Fringe", "A short cut with a stylish side-swept fringe.", 300) },
            { "shortBlackHair", ("Textured Crop", "A clean, textured haircut for an everyday look.", 100) },
            { "shortBrownHair", ("Tousled Cut", "A slightly messy style with effortless charm.", 200) },
            { "shortMochaHair", ("Soft Layers", "Light layers that create a gentle, relaxed look.", 250) },
            { "spikyBlonde", ("Spiky Cut", "A bold hairstyle with plenty of attitude.", 600) },
>>>>>>> 8b2d5a45bf39c6000f4e66ab15743a7dab84d6b7

            // Accessories
            { "strawHat", ("Salakot", "A traditional Filipino hat that keeps you cool under the sun.") }
        };

        int count = 0;
        // Find all OutfitItems in the scene (even if they are hidden/disabled)
        OutfitItem[] allItems = Object.FindObjectsByType<OutfitItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        
        foreach (var item in allItems)
        {
            // If the GameObject's name matches one in our dictionary, update it
            if (data.TryGetValue(item.gameObject.name, out var info))
            {
                item.itemName = info.name;
                item.itemDescription = info.desc;
                item.price = 100; // Temporary default price
                EditorUtility.SetDirty(item);
                count++;
            }
        }

        if (count > 0)
        {
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log($"Successfully updated {count} OutfitItems! Please save the scene (Ctrl+S).");
        }
        else
        {
            Debug.LogWarning("No matching OutfitItems found in the scene. Make sure you are in the CreateCharacterScene!");
        }
    }
}
#endif

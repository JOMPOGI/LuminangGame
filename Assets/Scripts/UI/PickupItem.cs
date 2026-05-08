using UnityEngine;

/// <summary>
/// Attach this to any object the player can pick up.
/// Wire the InteractableNPC's OnInteract event to PickupItem.Pickup().
/// </summary>
public class PickupItem : MonoBehaviour
{
    [Tooltip("Optional: Play a sound or particle effect before disappearing.")]
    public GameObject pickupEffect;

    /// <summary>
    /// Called by the InteractableNPC's OnInteract UnityEvent when the player clicks the button.
    /// </summary>
    public void Pickup()
    {
        // Optional: Spawn a pickup effect (e.g. sparkle, sound)
        if (pickupEffect != null)
        {
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        }

        Debug.Log($"[PickupItem] {gameObject.name} was picked up!");

        // Hide the object from the scene
        gameObject.SetActive(false);
    }
}

using UnityEngine;

public class InteractablePickup : InteractableBase
{
    public override void Interact()
    {
        if (!interactionEnabled) return;

        Debug.Log($"Picked up {gameObject.name}");
        OnInteract?.Invoke();
        
        // Hide the item
        gameObject.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.Events;

public abstract class InteractableBase : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public string promptText = "Interact";
    public UnityEvent OnInteract;
    public bool interactionEnabled = true;

    public abstract void Interact();

    protected virtual void OnEnable()
    {
        if (InteractionManager.Instance != null)
            InteractionManager.Instance.RegisterInteractable(this);
    }

    protected virtual void OnDisable()
    {
        if (InteractionManager.Instance != null)
            InteractionManager.Instance.UnregisterInteractable(this);
    }

    protected virtual void Start()
    {
        if (InteractionManager.Instance != null)
            InteractionManager.Instance.RegisterInteractable(this);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.3f);
        Gizmos.DrawSphere(transform.position, interactionDistance);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Allows the player to rotate a character preview by dragging on a UI element.
/// Attach this to the RawImage that displays the character RenderTexture.
/// </summary>
public class CharacterPreviewSpinner : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [Header("Target")]
    [Tooltip("The 3D character model to rotate (not the camera).")]
    public Transform characterTransform;

    [Header("Rotation Settings")]
    [Tooltip("How fast the character spins when dragging.")]
    public float rotationSpeed = 0.5f;

    [Tooltip("If true, rotates only on the Y axis (horizontal spin).")]
    public bool horizontalOnly = true;

    private float previousX;

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousX = eventData.position.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (characterTransform == null) return;

        float deltaX = eventData.position.x - previousX;
        previousX = eventData.position.x;

        // Rotate the character around the Y axis (spin left/right)
        characterTransform.Rotate(Vector3.up, -deltaX * rotationSpeed, Space.World);
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UITouchLookZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Look Sensitivity")]
    [Tooltip("Adjust this to make the camera move faster or slower when swiping.")]
    public float sensitivity = 0.05f;

    [Header("Output")]
    public UnityEvent<Vector2> touchZoneOutputEvent;

    private Vector2 lookDelta;
    private bool isDragging;
    private int activePointerId = -1;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isDragging) return; // Already being controlled by another finger

        isDragging = true;
        activePointerId = eventData.pointerId;
        lookDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && eventData.pointerId == activePointerId)
        {
            // Normalize delta relative to screen width (0.0 to 1.0 range)
            // This makes sensitivity identical on all resolutions!
            float normalizedX = eventData.delta.x / Screen.width;
            float normalizedY = eventData.delta.y / Screen.height;
            
            // Re-scale by a factor to keep sensitivity values similar to what they were
            // (1000 is a good baseline multiplier for normalized drag)
            lookDelta = new Vector2(normalizedX, normalizedY) * sensitivity * 1000f;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == activePointerId)
        {
            isDragging = false;
            activePointerId = -1;
            lookDelta = Vector2.zero;
            touchZoneOutputEvent.Invoke(Vector2.zero); // Stop looking
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            // Output the continuous look delta to the Controller
            touchZoneOutputEvent.Invoke(lookDelta);
            
            // Immediately reset it. If the finger stays still, OnDrag won't update it,
            // and lookDelta will stay zero, stopping the camera rotation.
            lookDelta = Vector2.zero;
        }
    }
}

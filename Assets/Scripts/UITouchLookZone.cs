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

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lookDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            // Capture the distance the finger moved during this frame
            lookDelta = eventData.delta * sensitivity;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        lookDelta = Vector2.zero;
        touchZoneOutputEvent.Invoke(Vector2.zero); // Stop looking
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

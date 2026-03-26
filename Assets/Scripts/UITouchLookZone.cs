using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UITouchLookZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Look Sensitivity")]
    [Tooltip("Adjust this to make the camera move faster or slower when swiping.")]
    public float sensitivity = 1.5f;

    [Header("Output")]
    public UnityEvent<Vector2> touchZoneOutputEvent;

    private Vector2 _lookDelta;
    private int _pointerId = -1;
    private bool _isDragging;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            _isDragging = true;
            _pointerId = eventData.pointerId;
            _lookDelta = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isDragging && eventData.pointerId == _pointerId)
        {
            // Normalize delta based on a reference resolution (e.g. 1080p height)
            // This ensures it feels the same on 4K, 1080p, and 720p screens.
            float referenceHeight = 1080f;
            float deviceScale = referenceHeight / Screen.height;

            // Capture the distance the finger moved, scaled for resolution independence
            // Increased the multiplier from 0.1 to 0.5 to make it snappier
            _lookDelta = eventData.delta * deviceScale * (sensitivity * 0.5f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_isDragging && eventData.pointerId == _pointerId)
        {
            _isDragging = false;
            _pointerId = -1;
            _lookDelta = Vector2.zero;
            touchZoneOutputEvent.Invoke(Vector2.zero); // Stop movement immediately
        }
    }

    private void Update()
    {
        if (_isDragging)
        {
            // Output the continuous look delta to the Controller
            touchZoneOutputEvent.Invoke(_lookDelta);
            
            // Immediately reset it. If the finger stays still, OnDrag won't update it,
            // and lookDelta will stay zero, preventing "drifting".
            _lookDelta = Vector2.zero;
        }
    }
}

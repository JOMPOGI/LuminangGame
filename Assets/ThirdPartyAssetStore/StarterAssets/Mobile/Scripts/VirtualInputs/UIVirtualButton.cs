using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIVirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }
    [System.Serializable]
    public class Event : UnityEvent { }

    [Header("Output")]
    public BoolEvent buttonStateOutputEvent;
    public Event buttonClickOutputEvent;

    private int trackedPointerId = -1;
    private bool isPressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isPressed)
        {
            isPressed = true;
            trackedPointerId = eventData.pointerId;
            OutputButtonStateValue(true);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPressed && eventData.pointerId == trackedPointerId)
        {
            isPressed = false;
            trackedPointerId = -1;
            OutputButtonStateValue(false);
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OutputButtonClickEvent();
    }

    void OutputButtonStateValue(bool buttonState)
    {
        buttonStateOutputEvent.Invoke(buttonState);
    }

    void OutputButtonClickEvent()
    {
        buttonClickOutputEvent.Invoke();
    }

}

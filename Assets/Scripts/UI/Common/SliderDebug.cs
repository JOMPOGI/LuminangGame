using UnityEngine;
using UnityEngine.EventSystems;

public class SliderDebug : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("<color=green>[SliderDebug] MOUSE DETECTED! I am being clicked!</color>");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("<color=yellow>[SliderDebug] MOUSE RELEASED!</color>");
    }
}

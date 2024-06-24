using UnityEngine;
using UnityEngine.EventSystems;

public class HandDragHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public ClockController clockController;

    public void OnPointerDown(PointerEventData eventData)
    {
        clockController.OnHandDragStart(transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        clockController.OnHandDrag(eventData, transform);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clockController.OnHandDragEnd();
    }
}
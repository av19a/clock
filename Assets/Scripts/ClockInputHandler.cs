using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class ClockInputHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private ClockManager clockManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Transform handPivot;

    private Vector3 clockCenter;
    private bool isDragging;

    private void Start()
    {
        clockCenter = GetComponentInParent<Canvas>().GetComponent<RectTransform>().position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!uiManager.IsEditMode()) return;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!uiManager.IsEditMode() || !isDragging || handPivot.name.Contains("Second")) return;

        Vector2 direction = eventData.position - (Vector2)clockCenter;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        handPivot.localRotation = Quaternion.Euler(0, 0, angle);
        UpdateTimeBasedOnDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void UpdateTimeBasedOnDrag()
    {
        DateTime currentTime = clockManager.GetTime();
        float angle = -handPivot.localEulerAngles.z;

        if (handPivot.name.Contains("Hour"))
        {
            int hours = Mathf.RoundToInt(angle / 30f) % 12;
            if (hours < 0) hours += 12;
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
                hours, currentTime.Minute, currentTime.Second);
        }
        else if (handPivot.name.Contains("Minute"))
        {
            int minutes = Mathf.RoundToInt(angle / 6f) % 60;
            if (minutes < 0) minutes += 60;
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
                currentTime.Hour, minutes, currentTime.Second);
        }

        clockManager.SetTime(currentTime);
        
        uiManager.hourInputField.text = currentTime.ToString("HH");
        uiManager.minuteInputField.text = currentTime.ToString("mm");
        
        uiManager.hourTimeText.text = currentTime.ToString("HH");
        uiManager.minuteTimeText.text = currentTime.ToString("mm");
    }
}
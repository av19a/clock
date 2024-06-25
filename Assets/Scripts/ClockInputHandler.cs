using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class ClockInputHandler : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Transform handPivot;
    private Vector3 clockCenter;
    private bool isDragging;
    private float previousAngle;
    private ClockUIManager clockUIManager;

    private void Start()
    {
        clockCenter = GetComponentInParent<Canvas>().GetComponent<RectTransform>().position;
        clockUIManager = FindObjectOfType<ClockUIManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!clockUIManager.IsEditMode())
            return;
        
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!clockUIManager.IsEditMode())
            return;
        
        if (isDragging && !handPivot.name.Contains("Second"))
        {
            Vector2 currentPosition = eventData.position;
            Vector2 direction = currentPosition - (Vector2)clockCenter;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            handPivot.localRotation = Quaternion.Euler(0, 0, angle);
            UpdateTimeBasedOnDrag();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void UpdateTimeBasedOnDrag()
    {
        DateTime currentTime = ClockManager.Instance.GetCurrentTime();

        if (handPivot.name.Contains("Hour"))
        {
            float hourAngle = -handPivot.localEulerAngles.z;
            int hours = Mathf.RoundToInt(hourAngle / 30f) % 12;
            if (hours < 0) hours += 12;
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
                hours, currentTime.Minute, currentTime.Second);
        }
        else if (handPivot.name.Contains("Minute"))
        {
            float minuteAngle = -handPivot.localEulerAngles.z;
            int minutes = Mathf.RoundToInt(minuteAngle / 6f) % 60;
            if (minutes < 0) minutes += 60;
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
                currentTime.Hour, minutes, currentTime.Second);
        }
        else if (handPivot.name.Contains("Second"))
        {
            float secondAngle = -handPivot.localEulerAngles.z;
            int seconds = Mathf.RoundToInt(secondAngle / 6f) % 60;
            if (seconds < 0) seconds += 60;
            currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day,
                currentTime.Hour, currentTime.Minute, seconds);
        }

        ClockManager.Instance.SetCurrentTime(currentTime);

        // Update the InputField with the new time
        clockUIManager.hourInputField.text = currentTime.ToString("HH");
        clockUIManager.minuteInputField.text = currentTime.ToString("mm");
        
        clockUIManager.hourTimeText.text = currentTime.ToString("HH");
        clockUIManager.minuteTimeText.text = currentTime.ToString("mm");
    }
}

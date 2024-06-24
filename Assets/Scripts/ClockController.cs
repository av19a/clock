using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro;

public class ClockController : MonoBehaviour
{
    public Transform hourHandPivot;
    public Transform minuteHandPivot;
    public Transform secondHandPivot;
    public TMP_Text timeText;

    public Button editButton;
    public Button saveButton;
    public TMP_InputField hourInput;
    public TMP_InputField minuteInput;
    public TMP_InputField secondInput;

    private DateTime currentTime;
    private const string TimeApiUrl = "http://worldtimeapi.org/api/timezone/Etc/UTC";
    private bool isEditing = false;
    private Transform draggingHand;
    private Vector3 clockCenter;

    void Start()
    {
        editButton.onClick.AddListener(OnEditButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
        clockCenter = GetComponent<RectTransform>().position;
        StartCoroutine(UpdateTimeFromServer());
        StartCoroutine(UpdateClockHands());
    }

    private IEnumerator UpdateTimeFromServer()
    {
        while (true)
        {
            yield return GetTimeFromServer();
            yield return new WaitForSeconds(3600); // Wait for one hour before updating again
        }
    }

    private async Task<IEnumerator> GetTimeFromServer()
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(TimeApiUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                TimeApiResponse timeApiResponse = JsonUtility.FromJson<TimeApiResponse>(jsonResponse);
                currentTime = DateTime.Parse(timeApiResponse.utc_datetime);
                UpdateClockUI();
            }
        }

        return null;
    }

    private void UpdateClockUI()
    {
        timeText.text = currentTime.ToString("HH:mm:ss");
        float hourAngle = (currentTime.Hour % 12) * 30 + currentTime.Minute * 0.5f;
        float minuteAngle = currentTime.Minute * 6;
        float secondAngle = currentTime.Second * 6;

        hourHandPivot.DORotate(new Vector3(0, 0, -hourAngle), 1, RotateMode.Fast);
        minuteHandPivot.DORotate(new Vector3(0, 0, -minuteAngle), 1, RotateMode.Fast);
        secondHandPivot.DORotate(new Vector3(0, 0, -secondAngle), 1, RotateMode.Fast);
    }

    private IEnumerator UpdateClockHands()
    {
        while (true)
        {
            if (!isEditing)
            {
                currentTime = currentTime.AddSeconds(1);
                UpdateClockUI();
            }
            yield return new WaitForSeconds(1);
        }
    }

    private void OnEditButtonClick()
    {
        isEditing = true;
        hourInput.text = currentTime.Hour.ToString("00");
        minuteInput.text = currentTime.Minute.ToString("00");
        secondInput.text = currentTime.Second.ToString("00");
    }

    private void OnSaveButtonClick()
    {
        if (int.TryParse(hourInput.text, out int hour) &&
            int.TryParse(minuteInput.text, out int minute) &&
            int.TryParse(secondInput.text, out int second))
        {
            try
            {
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hour, minute, second);
                UpdateClockUI();
                isEditing = false;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogError("Invalid time entered: " + e.Message);
            }
        }
    }

    // Dragging functionality
    public void OnHandDragStart(Transform hand)
    {
        draggingHand = hand;
    }

    public void OnHandDrag(PointerEventData eventData, Transform handPivot)
    {
        if (draggingHand == null) return;

        Vector3 currentPosition = eventData.position;
        Vector2 direction = currentPosition - clockCenter;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // Adjust for correct angle
        handPivot.localRotation = Quaternion.Euler(0, 0, angle);

        // Update the current time based on dragging
        UpdateTimeBasedOnDrag(handPivot);
    }

    public void OnHandDragEnd()
    {
        draggingHand = null;
    }

    private void UpdateTimeBasedOnDrag(Transform handPivot)
    {
        try
        {
            if (handPivot == hourHandPivot)
            {
                float hourAngle = -hourHandPivot.localEulerAngles.z;
                int hours = Mathf.RoundToInt(hourAngle / 15) % 24;
                if (hours < 0) hours += 24; // Ensure valid hour
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hours, currentTime.Minute, currentTime.Second);
            }
            else if (handPivot == minuteHandPivot)
            {
                float minuteAngle = -minuteHandPivot.localEulerAngles.z;
                int minutes = Mathf.RoundToInt(minuteAngle / 6) % 60;
                if (minutes < 0) minutes += 60; // Ensure valid minute
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, minutes, currentTime.Second);
            }
            else if (handPivot == secondHandPivot)
            {
                float secondAngle = -secondHandPivot.localEulerAngles.z;
                int seconds = Mathf.RoundToInt(secondAngle / 6) % 60;
                if (seconds < 0) seconds += 60; // Ensure valid second
                currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, seconds);
            }

            UpdateClockUI();
        }
        catch (ArgumentOutOfRangeException e)
        {
            Debug.LogError("Invalid time during drag: " + e.Message);
        }
    }

    [Serializable]
    private class TimeApiResponse
    {
        public string utc_datetime;
    }
}

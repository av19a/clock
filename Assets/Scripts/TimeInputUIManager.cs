using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TimeInputUIManager : MonoBehaviour
{
    public TMP_InputField hourInput;
    public TMP_InputField minuteInput;
    public TMP_InputField secondInput;
    public Button editButton;
    public Button saveButton;

    private void Start()
    {
        editButton.onClick.AddListener(OnEditButtonClick);
        saveButton.onClick.AddListener(OnSaveButtonClick);
    }

    private void OnEditButtonClick()
    {
        DateTime currentTime = ClockManager.Instance.GetCurrentTime();
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
                DateTime currentTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    hour, minute, second);
                ClockManager.Instance.SetCurrentTime(currentTime);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogError("Invalid time entered: " + e.Message);
            }
        }
    }
}
using UnityEngine;
using System;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class ClockUIManager : MonoBehaviour
{
    public Transform hourHandPivot;
    public Transform minuteHandPivot;
    public Transform secondHandPivot;
    public TMP_Text timeText;
    
    public TMP_InputField hourInputField;
    public TMP_InputField minuteInputField;
    public TMP_InputField secondInputField;
    public Button editButton;
    public Button saveButton;
    private bool isEditMode = false;
    
    public Button add12HoursButton;

    private void OnEnable()
    {
        ClockManager.Instance.OnTimeUpdated += UpdateClockUI;
        UpdateClockUI(ClockManager.Instance.GetCurrentTime()); // Initialize clock hands
        
        editButton.onClick.AddListener(OnEditMode);
        saveButton.onClick.AddListener(OffEditMode);
        
        add12HoursButton.onClick.AddListener(Add12Hours);
        
        hourInputField.onEndEdit.AddListener(OnTimeInputEndEdit);
        minuteInputField.onEndEdit.AddListener(OnTimeInputEndEdit);
        secondInputField.onEndEdit.AddListener(OnTimeInputEndEdit);
    }

    private void OnDisable()
    {
        ClockManager.Instance.OnTimeUpdated -= UpdateClockUI;
    }

    private void UpdateClockUI(DateTime currentTime)
    {
        timeText.text = currentTime.ToString("HH:mm:ss");

        // Calculate the angles
        float hourAngle = (currentTime.Hour % 12) * 30 + currentTime.Minute * 0.5f;
        float minuteAngle = currentTime.Minute * 6;
        float secondAngle = currentTime.Second * 6;

        // Update the hand rotations
        hourHandPivot.DORotate(new Vector3(0, 0, -hourAngle), 1, RotateMode.Fast);
        minuteHandPivot.DORotate(new Vector3(0, 0, -minuteAngle), 1, RotateMode.Fast);
        secondHandPivot.DORotate(new Vector3(0, 0, -secondAngle), 1, RotateMode.Fast);
        
        if (!isEditMode)
        {
            hourInputField.text = currentTime.ToString("HH");
            minuteInputField.text = currentTime.ToString("mm");
            secondInputField.text = currentTime.ToString("ss");
        }
    }
    
    private void Add12Hours()
    {
        DateTime currentTime = ClockManager.Instance.GetCurrentTime();
        if (currentTime.Hour < 12 || (currentTime.Hour == 12 && currentTime.Minute == 0 && currentTime.Second == 0))
        {
            DateTime newTime = currentTime.AddHours(12);
            ClockManager.Instance.SetCurrentTime(newTime);
            hourInputField.text = ClockManager.Instance.GetCurrentTime().ToString("HH");
        }
    }
    
    private void OnEditMode()
    {
        isEditMode = true;
        hourInputField.interactable = isEditMode;
        minuteInputField.interactable = isEditMode;
        secondInputField.interactable = isEditMode;
    }
    
    private void OffEditMode()
    {
        isEditMode = false;
        hourInputField.interactable = isEditMode;
        minuteInputField.interactable = isEditMode;
        secondInputField.interactable = isEditMode;
        SetTimeFromInputFields();
    }
    
    private void OnTimeInputEndEdit(string input)
    {
        if (isEditMode)
        {
            SetTimeFromInputFields();
        }
    }
    
    private void SetTimeFromInputFields()
    {
        SetTimeFromInput();
    }

    private void SetTimeFromInput()
    {
        if (int.TryParse(hourInputField.text, out int hours) &&
            int.TryParse(minuteInputField.text, out int minutes) &&
            int.TryParse(secondInputField.text, out int seconds))
        {
            DateTime newTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, seconds);
            ClockManager.Instance.SetCurrentTime(newTime);
        }
        else
        {
            hourInputField.text = ClockManager.Instance.GetCurrentTime().ToString("HH");
            minuteInputField.text = ClockManager.Instance.GetCurrentTime().ToString("mm");
            secondInputField.text = ClockManager.Instance.GetCurrentTime().ToString("ss");
        }
    }

    public bool IsEditMode()
    {
        return isEditMode;
    }
}
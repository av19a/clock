using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ClockManager clockManager;
    [SerializeField] private Transform hourHandPivot, minuteHandPivot, secondHandPivot;
    [SerializeField] public TMP_Text hourTimeText, minuteTimeText, secondTimeText;
    [SerializeField] public TMP_InputField hourInputField, minuteInputField, secondInputField;
    [SerializeField] private Button editButton, saveButton, amButton, pmButton;

    private bool isEditMode = false;

    private void Start()
    {
        clockManager.OnTimeUpdated += UpdateUI;
        SetupButtonListeners();
        UpdateUI(clockManager.GetTime());
    }

    private void OnDestroy()
    {
        clockManager.OnTimeUpdated -= UpdateUI;
    }

    private void SetupButtonListeners()
    {
        editButton.onClick.AddListener(OnEditMode);
        saveButton.onClick.AddListener(OffEditMode);
        amButton.onClick.AddListener(Subtract12Hours);
        pmButton.onClick.AddListener(Add12Hours);
        hourInputField.onEndEdit.AddListener(_ => OnTimeInputEndEdit());
        minuteInputField.onEndEdit.AddListener(_ => OnTimeInputEndEdit());
        secondInputField.onEndEdit.AddListener(_ => OnTimeInputEndEdit());
    }

    private void UpdateUI(DateTime time)
    {
        if (!isEditMode)
        {
            UpdateClockHands(time);
            UpdateTimeText(time);
        }
    }

    private void UpdateClockHands(DateTime time)
    {
        float hourAngle = (time.Hour % 12) * 30 + time.Minute * 0.5f;
        float minuteAngle = time.Minute * 6;
        float secondAngle = time.Second * 6;

        hourHandPivot.DORotate(new Vector3(0, 0, -hourAngle), 1, RotateMode.Fast);
        minuteHandPivot.DORotate(new Vector3(0, 0, -minuteAngle), 1, RotateMode.Fast);
        secondHandPivot.DORotate(new Vector3(0, 0, -secondAngle), 1, RotateMode.Fast);
    }

    private void UpdateTimeText(DateTime time)
    {
        hourTimeText.text = time.ToString("HH");
        minuteTimeText.text = time.ToString("mm");
        secondTimeText.text = time.ToString("ss");

        hourInputField.text = time.ToString("HH");
        minuteInputField.text = time.ToString("mm");
        secondInputField.text = time.ToString("ss");
    }

    private void OnEditMode()
    {
        isEditMode = true;
        SetInputFieldsInteractable(true);
        amButton.interactable = true;
        pmButton.interactable = true;
    }

    private void OffEditMode()
    {
        isEditMode = false;
        SetInputFieldsInteractable(false);
        SetTimeFromInputFields();
        amButton.interactable = false;
        pmButton.interactable = false;
    }

    private void SetInputFieldsInteractable(bool interactable)
    {
        hourInputField.interactable = interactable;
        minuteInputField.interactable = interactable;
        secondInputField.interactable = interactable;
    }

    private void OnTimeInputEndEdit()
    {
        if (isEditMode)
        {
            SetTimeFromInputFields();
        }
    }

    private void SetTimeFromInputFields()
    {
        if (int.TryParse(hourInputField.text, out int hours) &&
            int.TryParse(minuteInputField.text, out int minutes) &&
            int.TryParse(secondInputField.text, out int seconds))
        {
            try
            {
                DateTime newTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                    hours, minutes, seconds);
                clockManager.SetTime(newTime);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogError($"Invalid time entered: {e.Message}");
                ResetInputFields();
            }
        }
        else
        {
            ResetInputFields();
        }
    }

    private void ResetInputFields()
    {
        DateTime currentTime = clockManager.GetTime();
        hourInputField.text = currentTime.ToString("HH");
        minuteInputField.text = currentTime.ToString("mm");
        secondInputField.text = currentTime.ToString("ss");
    }

    private void Subtract12Hours()
    {
        DateTime currentTime = clockManager.GetTime();
        if (currentTime.Hour >= 12)
        {
            var addHours = currentTime.AddHours(-12);
            clockManager.SetTime(addHours);
            hourInputField.text = addHours.ToString("HH");
        }
    }

    private void Add12Hours()
    {
        DateTime currentTime = clockManager.GetTime();
        if (currentTime.Hour < 12)
        {
            var addHours = currentTime.AddHours(12);
            clockManager.SetTime(addHours);
            hourInputField.text = addHours.ToString("HH");
        }
    }

    public bool IsEditMode() => isEditMode;
}
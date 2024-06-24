using UnityEngine;
using System;
using DG.Tweening;
using TMPro;

public class ClockUIManager : MonoBehaviour
{
    public Transform hourHandPivot;
    public Transform minuteHandPivot;
    public Transform secondHandPivot;
    public TMP_Text timeText;

    private void OnEnable()
    {
        ClockManager.Instance.OnTimeUpdated += UpdateClockUI;
        UpdateClockUI(ClockManager.Instance.GetCurrentTime()); // Initialize clock hands
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
    }
}
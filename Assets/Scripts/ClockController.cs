using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;
// using DG.Tweening;

public class ClockController : MonoBehaviour
{
    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;
    public Text timeText;

    private DateTime currentTime;
    private const string TimeApiUrl = "http://worldtimeapi.org/api/timezone/Etc/UTC";

    void Start()
    {
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
        float hourAngle = currentTime.Hour % 12 * 30 + currentTime.Minute * 0.5f;
        float minuteAngle = currentTime.Minute * 6;
        float secondAngle = currentTime.Second * 6;

        // hourHand.DORotate(new Vector3(0, 0, -hourAngle), 1, RotateMode.Fast);
        // minuteHand.DORotate(new Vector3(0, 0, -minuteAngle), 1, RotateMode.Fast);
        // secondHand.DORotate(new Vector3(0, 0, -secondAngle), 1, RotateMode.Fast);
    }

    private IEnumerator UpdateClockHands()
    {
        while (true)
        {
            currentTime = currentTime.AddSeconds(1);
            UpdateClockUI();
            yield return new WaitForSeconds(1);
        }
    }

    [Serializable]
    private class TimeApiResponse
    {
        public string utc_datetime;
    }
}

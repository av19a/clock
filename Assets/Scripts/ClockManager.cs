using UnityEngine;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class ClockManager : MonoBehaviour
{
    public event Action<DateTime> OnTimeUpdated;
    private DateTime currentTime;
    private const string TimeApiUrl = "http://worldtimeapi.org/api/timezone/Etc/UTC";

    private void Start()
    {
        UpdateTimeFromServer();
        InvokeRepeating(nameof(UpdateTime), 1f, 1f);
    }

    public void SetTime(DateTime time)
    {
        currentTime = time;
        OnTimeUpdated?.Invoke(currentTime);
    }

    public DateTime GetTime() => currentTime;

    private async void UpdateTimeFromServer()
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(TimeApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    TimeApiResponse timeApiResponse = JsonUtility.FromJson<TimeApiResponse>(jsonResponse);
                    SetTime(DateTime.Parse(timeApiResponse.utc_datetime));
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error fetching time from server: {e.Message}");
        }
    }

    private void UpdateTime()
    {
        SetTime(currentTime.AddSeconds(1));
    }

    [Serializable]
    private class TimeApiResponse
    {
        public string utc_datetime;
    }
}
using UnityEngine;
using System;
using System.Collections;
using System.Net.Http;
using System.Threading.Tasks;

public class ClockManager : MonoBehaviour
{
    private static ClockManager _instance;
    public static ClockManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject manager = new GameObject("ClockManager");
                _instance = manager.AddComponent<ClockManager>();
                DontDestroyOnLoad(manager);
            }
            return _instance;
        }
    }

    private const string TimeApiUrl = "http://worldtimeapi.org/api/timezone/Etc/UTC";
    private DateTime currentTime;
    public event Action<DateTime> OnTimeUpdated;

    private void Start()
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

    private async Task GetTimeFromServer()
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(TimeApiUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                TimeApiResponse timeApiResponse = JsonUtility.FromJson<TimeApiResponse>(jsonResponse);
                currentTime = DateTime.Parse(timeApiResponse.utc_datetime);
                OnTimeUpdated?.Invoke(currentTime);
            }
        }
    }

    private IEnumerator UpdateClockHands()
    {
        while (true)
        {
            currentTime = currentTime.AddSeconds(1);
            OnTimeUpdated?.Invoke(currentTime);
            yield return new WaitForSeconds(1);
        }
    }

    public DateTime GetCurrentTime()
    {
        return currentTime;
    }

    public void SetCurrentTime(DateTime time)
    {
        currentTime = time;
        OnTimeUpdated?.Invoke(currentTime);
    }

    [Serializable]
    private class TimeApiResponse
    {
        public string utc_datetime;
    }
}

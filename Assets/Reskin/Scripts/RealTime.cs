using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public sealed class RealTime : MonoBehaviour
{
    private const string LastSessionUniversalDateTime = nameof(LastSessionUniversalDateTime);
    private const string LogPrefix = "<b><color=#00D1FF>[RealTime]</color></b>";

    public event Action<TimeSpan> AbsenceTimeCalculated;

    private DateTime serverUniversalDateTimeAtSync;
    private float realtimeSinceStartupAtSync;
    private bool isTimeSynchronized;

    private void Start()
    {
        Debug.Log($"{LogPrefix} Start synchronization");
        StartCoroutine(SynchronizeTime());
        StartCoroutine(SaveTimePeriodically());
    }

    public void SaveAfterImportantEvent()
    {
        SaveCurrentUniversalDateTime("Important event");
    }

    private IEnumerator SynchronizeTime()
    {
        Debug.Log($"{LogPrefix} <color=#FFD166>Synchronizing global time...</color>");

        using UnityWebRequest unityWebRequest = UnityWebRequest.Get("https://google.com");
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"{LogPrefix} <color=#FF4D4D>Failed to synchronize global time.</color>");
            yield break;
        }

        string dateHeader = unityWebRequest.GetResponseHeader("Date");

        if (!DateTime.TryParse(dateHeader, out DateTime parsedDateTime))
        {
            Debug.LogWarning($"{LogPrefix} <color=#FF4D4D>Failed to parse global time.</color>");
            yield break;
        }

        DateTime currentUniversalDateTime = parsedDateTime.ToUniversalTime();

        if (PlayerPrefs.HasKey(LastSessionUniversalDateTime))
        {
            bool isParsed = DateTime.TryParse(
                PlayerPrefs.GetString(LastSessionUniversalDateTime),
                null,
                DateTimeStyles.RoundtripKind,
                out DateTime lastSessionUniversalDateTime);

            if (isParsed)
            {
                TimeSpan absenceTime = currentUniversalDateTime - lastSessionUniversalDateTime;

                Debug.Log($"{LogPrefix} <color=#9BE564>Time of absence:</color> <b>{absenceTime}</b>");
                AbsenceTimeCalculated?.Invoke(absenceTime);
            }
        }

        serverUniversalDateTimeAtSync = currentUniversalDateTime;
        realtimeSinceStartupAtSync = Time.realtimeSinceStartup;
        isTimeSynchronized = true;

        SaveCurrentUniversalDateTime("Initial synchronization");
        Debug.Log($"{LogPrefix} <color=#9BE564>Global UTC time synchronized:</color> <b>{currentUniversalDateTime:O}</b>");
    }

    private IEnumerator SaveTimePeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            SaveCurrentUniversalDateTime("Periodic save");
        }
    }

    private DateTime GetCurrentUniversalDateTime()
    {
        float passedSeconds = Time.realtimeSinceStartup - realtimeSinceStartupAtSync;
        return serverUniversalDateTimeAtSync.AddSeconds(passedSeconds);
    }

    private void SaveCurrentUniversalDateTime(string reason)
    {
        if (!isTimeSynchronized)
        {
            Debug.LogWarning($"{LogPrefix} <color=#FF8C42>Save skipped</color> | Reason: <b>{reason}</b> | Time is not synchronized yet.");
            return;
        }

        DateTime currentUniversalDateTime = GetCurrentUniversalDateTime();
        string universalDateTimeText = currentUniversalDateTime.ToString("O");

        PlayerPrefs.SetString(LastSessionUniversalDateTime, universalDateTimeText);
        PlayerPrefs.Save();

        Debug.Log($"{LogPrefix} <color=#00FF7F><b>TIME SAVED</b></color> | Reason: <b>{reason}</b> | UTC: <b>{universalDateTimeText}</b>");
    }
}
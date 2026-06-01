using UnityEngine;
using System;

public class DailyVisitLogger
{
    public void Log()
    {
        LogDailyVisitOnce();
    }

    private void LogDailyVisitOnce()
    {
        string lastVisitKey = "LastDailyVisit";
        string today = DateTime.Now.ToString("yyyyMMdd");

        if (PlayerPrefs.GetString(lastVisitKey, "") != today)
        {
            PlayerPrefs.SetString(lastVisitKey, today);
            PlayerPrefs.Save();
        }
    }
}

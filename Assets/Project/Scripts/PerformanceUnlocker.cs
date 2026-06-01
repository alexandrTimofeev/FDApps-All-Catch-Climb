using UnityEngine;

public static class PerformanceUnlocker
{
    public static void Execute()
    {
        Application.targetFrameRate = -1;

        QualitySettings.vSyncCount = 0;
    }
}

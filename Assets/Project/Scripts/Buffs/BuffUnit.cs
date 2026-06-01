using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffUnit
{
    public event Action OnComplete;
    /// <summary>
    /// Returns total time left of the buff after extend
    /// </summary>
    public event Action<float> OnTimeExtended;
    public SmartTimer smartTimer { get; private set; }
    public BuffUnit(TimerManager timerManager, float duration, System.Action action)
    {
        smartTimer = timerManager.CreateTimer(duration, OnComplete);

        smartTimer.OnFinished += EndBuff;

        OnComplete += action;
    }
    public void ExtendTime(float duration)
    {
        smartTimer.ExtendTime(duration);

        OnTimeExtended?.Invoke(smartTimer.TimeLeft);
    }
    public void EndBuff()
    {
        OnComplete?.Invoke();
    }
}

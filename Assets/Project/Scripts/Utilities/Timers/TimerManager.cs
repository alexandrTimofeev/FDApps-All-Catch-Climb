using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    private List<SmartTimer> timers = new();

    public float GlobalTimeMultiplier { get; set; } = 1f;

    public SmartTimer CreateTimer(float duration, System.Action callback)
    {
        var timer = new SmartTimer(duration, callback);
        timers.Add(timer);
        return timer;
    }

    void Update()
    {
        float delta = Time.deltaTime * GlobalTimeMultiplier;

        for (int i = timers.Count - 1; i >= 0; i--)
        {
            var timer = timers[i];

            if (!timer.IsFinished && !timer.IsPaused)
                timer.Tick(delta);

            if (timer.IsFinished && !timer.DontDestroy)
                timers.RemoveAt(i);
        }
    }
}

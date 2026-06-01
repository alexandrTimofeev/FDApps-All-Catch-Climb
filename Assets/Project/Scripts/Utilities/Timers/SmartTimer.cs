using System;

public class SmartTimer
{
    public float Duration { get; set; }
    public float TimeLeft { get; private set; }
    public float SpeedMultiplier { get; set; } = 1f;
    public Action OnFinished;
    public Func<bool> TickCondition;

    public bool IsPaused { get; private set; } = false;
    public bool IsLooped { get; set; } = false;
    public bool DontDestroy { get; set; } = false;

    private bool isCompleted = false;

    public SmartTimer(float duration, Action onFinished)
    {
        Duration = duration;
        TimeLeft = duration;
        OnFinished = onFinished;
    }

    internal void Tick(float deltaTime)
    {
        if (isCompleted || IsPaused)
            return;

        if (TickCondition != null && !TickCondition())
            return;

        TimeLeft -= deltaTime * SpeedMultiplier;

        if (TimeLeft <= 0f)
        {
            OnFinished?.Invoke();

            if (IsLooped)
            {
                TimeLeft = Duration;
            }
            else
            {
                isCompleted = true;
            }
        }
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;

    public void Reset()
    {
        TimeLeft = Duration;
        isCompleted = false;
        Resume();
    }

    public void Stop()
    {
        isCompleted = true;
    }
    public void ExtendTime(float duration)
    {
        Duration += duration;
        TimeLeft += duration;
    }
    public bool IsFinished => isCompleted;
}

using UnityEngine;
using VContainer;

public class CookingSpotOutputZone : EventZone
{
    private CookingSpot cookingSpot;
    private PlayerInventory playerInventory;
    private TimerManager timerManager;

    private float outputDelay;
    private SmartTimer outputTimer;

    [Inject]
    public void Construct(CookingSpot cookingSpot, PlayerInventory playerInventory, TimerManager timerManager)
    {
        this.cookingSpot = cookingSpot;
        this.playerInventory = playerInventory;
        this.timerManager = timerManager;
    }

    void Start()
    {
        if (!ValidateDependencies(nameof(Start)))
            return;

        outputDelay = cookingSpot.outputDelay;
    }

    public override void PlayerEnter()
    {
        if (!ValidateDependencies(nameof(PlayerEnter)))
            return;

        if (!playerInventory.HasSpace() && cookingSpot.HasFish())
        {
            UIWarningPopup.Instance.ShowWarning("You have no space left in inventory!", 4f);
            return;
        }

        if (outputTimer == null)
        {
            outputTimer = timerManager.CreateTimer(outputDelay, OnOutputTick);
            outputTimer.IsLooped = true;
        }

        outputTimer.Resume();
    }

    public override void PlayerExit()
    {
        outputTimer?.Pause();
    }

    private void OnOutputTick()
    {
        if (!ValidateDependencies(nameof(OnOutputTick)))
            return;

        if (!cookingSpot.HasFish()) return;

        if (!playerInventory.HasSpace())
        {
            outputTimer.Pause();
            return;
        }

        FishItem fish = cookingSpot.OutputFish();

        if (fish != null)
        {
            playerInventory.AddFishItem(fish, true, true);
        }
    }

    private bool ValidateDependencies(string caller)
    {
        if (cookingSpot == null) return LogMissingDependency(nameof(cookingSpot), caller);
        if (playerInventory == null) return LogMissingDependency(nameof(playerInventory), caller);
        if (timerManager == null) return LogMissingDependency(nameof(timerManager), caller);
        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{nameof(CookingSpotOutputZone)}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }
}

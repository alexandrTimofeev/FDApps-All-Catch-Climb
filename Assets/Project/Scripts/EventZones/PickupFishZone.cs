using System;
using UnityEngine;
using VContainer;

public class PickupFishZone : EventZone
{
    private PlayerInventory playerInventory;
    private TimerManager timerManager;

    private float outputDelay;
    private FishFilter fishToPickup;
    private Action<FishItem> actions;

    private SmartTimer pickupTimer;

    [Inject]
    public void Construct(PlayerInventory playerInventory, TimerManager timerManager)
    {
        this.playerInventory = playerInventory;
        this.timerManager = timerManager;
    }

    public void SetDelay(float delay)
    {
        outputDelay = delay;
    }

    public void SetFishHandler(Action<FishItem> newActions)
    {
        actions = newActions;
    }

    public void SetPickupFilter(FishFilter fishToPickup)
    {
        this.fishToPickup = fishToPickup;
    }

    public override void PlayerEnter()
    {
        if (!ValidateDependencies(nameof(PlayerEnter)))
            return;

        if (pickupTimer == null)
        {
            pickupTimer = timerManager.CreateTimer(outputDelay, OnPickupTick);
            pickupTimer.SpeedMultiplier = 1f;
            pickupTimer.IsLooped = true;
        }
        else
        {
            pickupTimer.Reset();
        }

        pickupTimer.Resume();
    }

    public override void PlayerExit()
    {
        if (pickupTimer != null)
            pickupTimer.Pause();
    }

    private void OnPickupTick()
    {
        if (!ValidateDependencies(nameof(OnPickupTick)))
            return;

        if (playerInventory.ItemsAmount() == 0)
            return;

        FishItem fish = playerInventory.TakeFirstFish(fishToPickup);
        if (fish != null && actions != null)
        {
            actions.Invoke(fish);
        }

        pickupTimer.Reset();
    }

    private bool ValidateDependencies(string caller)
    {
        if (playerInventory == null) return LogMissingDependency(nameof(playerInventory), caller);
        if (timerManager == null) return LogMissingDependency(nameof(timerManager), caller);
        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{nameof(PickupFishZone)}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }
}

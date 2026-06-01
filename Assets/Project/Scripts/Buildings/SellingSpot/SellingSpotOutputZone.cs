using UnityEngine;
using VContainer;

public class SellingSpotOutputZone : EventZone
{
    private TimerManager timerManager;
    private SellingSpot sellingSpot;
    private float minCoinCollectDelay;
    private int minCoinAmountSpeedup;
    private int maxCoinsAmountSpeedup;
    private SmartTimer collectTimer;
    private float coinCollectDelay;
    private float speedupSpeed;
    [Inject]
    public void Construct(SellingSpot sellingSpot, TimerManager timerManager)
    {
        this.sellingSpot = sellingSpot;
        minCoinCollectDelay = sellingSpot.minCoinCollectDelay;
        minCoinAmountSpeedup = sellingSpot.minCoinAmountForSpeedup;
        speedupSpeed = sellingSpot.speedupSpeed;

        maxCoinsAmountSpeedup = minCoinAmountSpeedup * 7;

        this.timerManager = timerManager;
    }

    public override void PlayerEnter()
    {
        if (collectTimer == null)
        {
            collectTimer = timerManager.CreateTimer(minCoinCollectDelay, OnCollectTick);
            collectTimer.IsLooped = true;
        }

        ChangePickupSpeed();

        collectTimer.Resume();
    }
    private void ChangePickupSpeed()
    {
        int coinsLeft = sellingSpot.GetCoinsAmount();

        if (coinsLeft <= minCoinAmountSpeedup)
        {
            coinCollectDelay = minCoinCollectDelay;
            collectTimer.Duration = minCoinCollectDelay;
        }
        else
        {
            float coinDiff = coinsLeft - minCoinAmountSpeedup;

            float pickupDelay = Mathf.Lerp(minCoinCollectDelay, minCoinCollectDelay / 5, coinDiff / (float)maxCoinsAmountSpeedup);

            coinCollectDelay = pickupDelay;
        }

        collectTimer.Duration = minCoinCollectDelay;
    }
    public override void PlayerExit()
    {
        collectTimer?.Pause();
    }
    public override void PlayerStay()
    {
        float timerDuration = collectTimer.Duration;

        if (!Mathf.Approximately(coinCollectDelay, timerDuration))
        {
            collectTimer.Duration = Mathf.MoveTowards(timerDuration, coinCollectDelay, Time.deltaTime * speedupSpeed);
        }
    }
    private void OnCollectTick()
    {
        if (!sellingSpot.HasAnyCoins())
        {
            collectTimer.Pause();
            return;
        }

        sellingSpot.CollectLastCoin();
    }
}

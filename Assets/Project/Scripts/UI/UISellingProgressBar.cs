using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UISellingProgressBar : ProgressBar
{
    private SellingSpot sellingSpot;

    [Inject]
    public void Construct(SellingSpot sellingSpot)
    {
        this.sellingSpot = sellingSpot;
    }

    protected override void SubscribeToEvents()
    {
        EventBus.Subscribe<StartSellingFish_event>(ShowBar);
        EventBus.Subscribe<StopSellingFish_event>(HideBar);
    }

    public override void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<StartSellingFish_event>(ShowBar);
        EventBus.Unsubscribe<StopSellingFish_event>(HideBar);
    }
    public override float GetValueForBar()
    {
        if (sellingSpot == null)
        {
            Debug.LogError($"{nameof(UISellingProgressBar)}.{nameof(GetValueForBar)}: {nameof(sellingSpot)} is null on {name}.");
            return 0f;
        }

        return sellingSpot.GetFillAmount();

    }
}

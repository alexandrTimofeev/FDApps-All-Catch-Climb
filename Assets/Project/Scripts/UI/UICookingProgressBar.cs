using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class UICookingProgressBar : ProgressBar
{
    private CookingSpot cookingSpot;

    [Inject]
    public void Construct(CookingSpot cookingSpot)
    {
        this.cookingSpot = cookingSpot;
    }

    protected override void SubscribeToEvents()
    {
        EventBus.Subscribe<StartCooking_event>(ShowBar);
        EventBus.Subscribe<StopCooking_event>(HideBar);
    }
    public override void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<StartCooking_event>(ShowBar);
        EventBus.Unsubscribe<StopCooking_event>(HideBar);
    }

    public override float GetValueForBar()
    {
        if (cookingSpot == null)
        {
            Debug.LogError($"{nameof(UICookingProgressBar)}.{nameof(GetValueForBar)}: {nameof(cookingSpot)} is null on {name}.");
            return 0f;
        }

        return cookingSpot.GetFillAmount();
    }
}

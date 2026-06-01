using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class CookingSpotCookingZone : EventZone
{
    private CookingSpot cookingSpot;

    [Inject]
    public void Construct(CookingSpot cookingSpot)
    {
        this.cookingSpot = cookingSpot;
    }

    public override void PlayerStay()
    {
        if (cookingSpot == null)
        {
            Debug.LogError($"{nameof(CookingSpotCookingZone)}.{nameof(PlayerStay)}: {nameof(cookingSpot)} is null on {name}.");
            return;
        }

        cookingSpot.CookFish();
    }
    public override void PlayerEnter()
    {
        if (cookingSpot == null)
        {
            Debug.LogError($"{nameof(CookingSpotCookingZone)}.{nameof(PlayerEnter)}: {nameof(cookingSpot)} is null on {name}.");
            return;
        }

        if (cookingSpot.HasCurrentFish() == false)
            cookingSpot.ResetTimer();
    }
}

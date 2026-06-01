using VContainer;

public class CookingSpotPickupZone : PickupFishZone
{
    [Inject]
    public void Construct(CookingSpot cookingSpot)
    {
        SetDelay(cookingSpot.cooldownToPickupFish);
        SetPickupFilter(FishFilter.Raw);
        SetFishHandler(cookingSpot.InputFish);
    }
}

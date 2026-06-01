using VContainer;

public class SellingSpotPickupZone : PickupFishZone
{
    private SellingSpot sellingSpot;
    [Inject]
    public void Construct(SellingSpot sellingSpot)
    {
        this.sellingSpot = sellingSpot;

        SetDelay(sellingSpot.cooldownToPickupFish);
        SetPickupFilter(FishFilter.Cooked);
        SetFishHandler(sellingSpot.InputFish);
    }
}

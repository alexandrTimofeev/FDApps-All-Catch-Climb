using UnityEngine;
using VContainer;

public class SellingSpotSellZone : EventZone
{
    private SellingSpot sellingSpot;
    private StartSellingFish_event startSellingFish_Event = new();
    private StopSellingFish_event stopSellingFish_Event = new();
    public bool canSell = true;
    [Inject]
    public void Construct(SellingSpot sellingSpot)
    {
        this.sellingSpot = sellingSpot;
    }
    public override void PlayerEnter()
    {
        canSell = true;

        if (!sellingSpot.HasAnyFish())
        {
            UIWarningPopup.Instance.ShowWarning("You have no fish to sell", 3f);
            canSell = false;

            return;
        }

        EventBus.Publish(startSellingFish_Event);
    }
    public override void PlayerExit()
    {
        if (canSell) EventBus.Publish(stopSellingFish_Event);
        canSell = false;
    }
    public override void PlayerStay()
    {
        if (canSell == false || !sellingSpot.HasCurrentCustomer()) return;
        if (!CheckForSellAbility()) return;

        sellingSpot.OnUpdate();

        CheckForSellAbility();
    }
    private bool CheckForSellAbility()
    {
        if (!sellingSpot.HasAnyFish())
        {
            canSell = false;

            EventBus.Publish(stopSellingFish_Event);

            return false;
        }
        return true;
    }
}

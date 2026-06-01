using UnityEngine;
using VContainer;
using Unity.Assets.Project.Scripts.Enums;

public class AutomaticSellingUpgrade : Upgrade
{
    private SellingSpot sellingSpot;
    private SellingSpotSellZone sellingSpotSellZone;
    private UISellingProgressBar uISellingProgressBar;
    [Inject]
    public void Construct(SellingSpot sellingSpot, SellingSpotSellZone sellingSpotSellZone, UISellingProgressBar uISellingProgressBar)
    {
        this.sellingSpot = sellingSpot;
        this.sellingSpotSellZone = sellingSpotSellZone;
        this.uISellingProgressBar = uISellingProgressBar;
    }
    public override void LoadUpgrade()
    {
        ActivateUpgrade();
    }
    protected override void ActivateUpgrade()
    {
        var automaticSelling = sellingSpot.gameObject.AddComponent<SellingSpot_AutomaticSelling>();
        automaticSelling.sellingSpot = sellingSpot;

        Instantiate(sellingSpot.cashierPf, sellingSpotSellZone.transform.position + Vector3.up, Quaternion.identity);

        Destroy(sellingSpotSellZone.gameObject);
        uISellingProgressBar.DestroyProgressBar();

        gameObject.SetActive(false);
    }
    public override void OnPurchase()
    {
        vfxPlayer.PlayVFX(VFXType.DefaultPoof, sellingSpotSellZone.transform.position + Vector3.up);
    }
}

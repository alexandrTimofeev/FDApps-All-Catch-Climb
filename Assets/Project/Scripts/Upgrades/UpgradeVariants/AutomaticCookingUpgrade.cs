
using Unity.Assets.Project.Scripts.Enums;
using UnityEngine;
using VContainer;


public class AutomaticCookingUpgrade : Upgrade
{
    private CookingSpot _cookingSpot;
    private CookingSpotCookingZone _cookingSpotCookingZone;
    private UICookingProgressBar _uICookingProgressBar;
    [Inject]
    public void Construct(CookingSpot cookingSpot, CookingSpotCookingZone cookingSpotCookingZone, UICookingProgressBar uICookingProgressBar)
    {
        _cookingSpot = cookingSpot;
        _cookingSpotCookingZone = cookingSpotCookingZone;
        _uICookingProgressBar = uICookingProgressBar;
    }
    public override void LoadUpgrade()
    {
        ActivateUpgrade();
    }
    protected override void ActivateUpgrade()
    {
        var automaticCooking = _cookingSpot.gameObject.AddComponent<CookingSpot_AutomaticCooking>();
        automaticCooking.cookingSpot = _cookingSpot;

        Instantiate(_cookingSpot.cookPf, _cookingSpotCookingZone.transform.position + Vector3.up, Quaternion.identity);

        Destroy(_cookingSpotCookingZone.gameObject);
        _uICookingProgressBar.DestroyProgressBar();

        gameObject.SetActive(false);
    }
    public override void OnPurchase()
    {
        vfxPlayer.PlayVFX(VFXType.DefaultPoof, _cookingSpotCookingZone.transform.position + Vector3.up);
    }
}

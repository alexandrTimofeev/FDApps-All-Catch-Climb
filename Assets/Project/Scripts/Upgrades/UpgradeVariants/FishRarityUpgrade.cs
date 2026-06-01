using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class FishRarityUpgrade : Upgrade
{
    private PlayerFishingCharacteristics playerFishingCharacteristics;
    [SerializeField] private int level;
    [Inject]
    public void Construct(PlayerController playerController)
    {
        playerFishingCharacteristics = playerController.GetPlayerCharacteristics().GetFishingCharacteristics();
    }
    public override void LoadUpgrade()
    {
        ActivateUpgrade();
    }
    protected override void ActivateUpgrade()
    {
        playerFishingCharacteristics.SetChancesLevel(level);

        gameObject.SetActive(false);
    }
}

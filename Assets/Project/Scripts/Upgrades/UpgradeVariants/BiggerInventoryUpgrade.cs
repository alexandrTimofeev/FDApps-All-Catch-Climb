using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BiggerInventoryUpgrade : Upgrade
{
    private PlayerInventory playerInventory;
    [Inject]
    public void Construct(PlayerInventory playerInventory)
    {
        this.playerInventory = playerInventory;
    }
    public override void LoadUpgrade()
    {
        ActivateUpgrade();
    }
    protected override void ActivateUpgrade()
    {
        playerInventory.SetMaxCapacity(10);

        gameObject.SetActive(false);
    }
}

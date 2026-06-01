using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class VIP_CustomersUpgrade : Upgrade
{
    private CustomersLine customersLine;
    [Inject]
    public void Construct(CustomersLine customersLine)
    {
        this.customersLine = customersLine;
    }
    public override void LoadUpgrade()
    {
        ActivateUpgrade();
    }
    protected override void ActivateUpgrade()
    {
        customersLine.UnlockVIP();

        gameObject.SetActive(false);
    }
}

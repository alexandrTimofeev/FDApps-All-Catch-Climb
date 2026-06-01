using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingSpot_AutomaticSelling : MonoBehaviour
{
    public SellingSpot sellingSpot;
    void Update()
    {
        if (sellingSpot != null && sellingSpot.HasCurrentCustomer())
        {
            sellingSpot.OnUpdate();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSpot_AutomaticCooking : MonoBehaviour
{
    public CookingSpot cookingSpot;
    void Update()
    {
        if (cookingSpot != null)
        {
            cookingSpot.CookFish();
        }
    }
}

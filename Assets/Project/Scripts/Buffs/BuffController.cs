using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class BuffController : MonoBehaviour
{
    private Dictionary<RewardedAdType, BuffUnit> buffDictionary = new Dictionary<RewardedAdType, BuffUnit>();
    private TimerManager timerManager;
    private UIBuffController uIBuffController;
    public BuffController(TimerManager timerManager, UIBuffController uIBuffController)
    {
        this.timerManager = timerManager;
        this.uIBuffController = uIBuffController;
    }
    public void AddBuff()
    {
        

    }

}

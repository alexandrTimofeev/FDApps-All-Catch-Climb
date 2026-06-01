using UnityEngine;
using System;

public class EventZone : MonoBehaviour
{
    public event Action OnPlayerEnter;
    public event Action OnPlayerExit;
    public event Action OnPlayerStay;
    public virtual void PlayerEnter()
    {

    }
    public virtual void PlayerExit()
    {

    }
    public virtual void PlayerStay()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        PlayerEnter();
        OnPlayerEnter?.Invoke();
    }
    void OnTriggerExit(Collider other)
    {
        PlayerExit();
        OnPlayerExit?.Invoke();
    }
    void OnTriggerStay(Collider other)
    {
        PlayerStay();
        OnPlayerStay?.Invoke();
    }
}

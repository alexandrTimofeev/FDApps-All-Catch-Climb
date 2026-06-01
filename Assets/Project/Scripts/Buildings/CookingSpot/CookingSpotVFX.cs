using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingSpotVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem fishSplashPS;

    public void PlayFishSplashVFX()
    {
        fishSplashPS.Play(true);
    }
    public void SetFishSplashPosition(Vector3 position)
    {
        fishSplashPS.transform.position = position;
    }
}

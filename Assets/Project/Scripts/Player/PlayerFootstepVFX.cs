using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstepVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    public void PlayVFX()
    {
        _particleSystem.Play();
    }
    public void StopVFX()
    {
        _particleSystem.Stop();
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Assets.Project.Scripts.Enums;
using UnityEngine;

public class VFXUnit : MonoBehaviour
{
    public VFXType vfxType;
    private VFXPlayer vfxPlayer;
    private ParticleSystem _particleSystem;
    void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    public void Initialize(VFXPlayer vfxPlayer, VFXType type)
    {
        this.vfxPlayer = vfxPlayer;
        vfxType = type;
    }
    public void PlayVfx()
    {
        gameObject.SetActive(true);

        _particleSystem.Play();
    }
    void OnDisable()
    {
        vfxPlayer.ReturnSFXobject(this);
    }
}

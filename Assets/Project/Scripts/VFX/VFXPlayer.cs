using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Assets.Project.Scripts.Enums;
using UnityEngine;
using VContainer;
public class VFXPlayer : MonoBehaviour
{
    private VFXManager _VFXManager;

    private Dictionary<VFXType, Queue<VFXUnit>> vfxUnits = new();
    private bool isInitialized;

    [Inject]
    public void Construct(VFXManager vfxManager)
    {
        _VFXManager = vfxManager;
    }

    async void Start()
    {
        if (_VFXManager == null)
        {
            Debug.LogError($"{nameof(VFXPlayer)}.{nameof(Start)}: {nameof(_VFXManager)} is null on {name}.");
            return;
        }

        await InitializeSFXunits();
    }

    private async Task InitializeSFXunits()
    {
        if (_VFXManager == null)
        {
            Debug.LogError($"{nameof(VFXPlayer)}.{nameof(InitializeSFXunits)}: {nameof(_VFXManager)} is null on {name}.");
            return;
        }

        foreach (VFXType type in Enum.GetValues(typeof(VFXType)))
        {
            var prefab = await _VFXManager.GetVFXAsync(type);
            if (prefab == null)
                continue;

            var vfxObject = Instantiate(prefab, transform);
            var unit = vfxObject.GetComponent<VFXUnit>();
            if (unit == null)
            {
                Debug.LogError($"{nameof(VFXPlayer)}.{nameof(InitializeSFXunits)}: {nameof(VFXUnit)} is null on {vfxObject.name}.");
                Destroy(vfxObject);
                continue;
            }

            unit.Initialize(this, type);

            var queue = new Queue<VFXUnit>();
            vfxUnits.Add(type, queue);

            vfxObject.SetActive(false);
        }

        isInitialized = true;
    }
    /// <summary>
    /// Plays VFX at some position
    /// </summary>
    /// <param name="sfxType"></param>
    /// <param name="position"></param>
    public async void PlayVFX(VFXType vfxType, Vector3 position)
    {
        await PlayVFXAsync(vfxType, position);
    }

    public async Task PlayVFXAsync(VFXType vfxType, Vector3 position)
    {
        if (_VFXManager == null)
        {
            Debug.LogError($"{nameof(VFXPlayer)}.{nameof(PlayVFXAsync)}: {nameof(_VFXManager)} is null on {name}.");
            return;
        }

        GameObject vfxPrefab = await _VFXManager.GetVFXAsync(vfxType);

        if (vfxPrefab == null)
        {
            Debug.LogError("VFX was not found");
            return;
        }

        VFXUnit unit;

        if (!isInitialized || !vfxUnits.ContainsKey(vfxType) || vfxUnits[vfxType].Count == 0)
        {
            var vfxObject = Instantiate(vfxPrefab, transform);
            unit = vfxObject.GetComponent<VFXUnit>();
            if (unit == null)
            {
                Debug.LogError($"{nameof(VFXPlayer)}.{nameof(PlayVFXAsync)}: {nameof(VFXUnit)} is null on {vfxObject.name}.");
                Destroy(vfxObject);
                return;
            }

            unit.Initialize(this, vfxType);
        }
        else
        {
            unit = vfxUnits[vfxType].Dequeue();
        }

        unit.transform.position = position;
        unit.PlayVfx();
    }

    public void ReturnSFXobject(VFXUnit unit)
    {
        unit.gameObject.SetActive(false);
        vfxUnits[unit.vfxType].Enqueue(unit);
    }

}

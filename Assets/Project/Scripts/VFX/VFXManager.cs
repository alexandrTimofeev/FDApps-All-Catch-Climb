using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Assets.Project.Scripts.Enums;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer.Unity;
public class VFXManager : MonoBehaviour
{
    private readonly Dictionary<VFXType, GameObject> _vfxObjs = new();
    private bool _isLoaded = false;
    public async void Start()
    {
        await LoadAllSFXAsync();
    }
    public async UniTask LoadAllSFXAsync()
    {
        if (_isLoaded) return;

        foreach (VFXType type in Enum.GetValues(typeof(VFXType)))
        {
            string address = $"VFX/{type}_vfx";
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _vfxObjs[type] = handle.Result;
            }
            else
            {
                Debug.LogWarning($"VFX not found at address: {address}");
            }
        }

        // Debug.Log("All vfx are Loaded!");
        _isLoaded = true;
    }

    public async Task<GameObject> GetVFXAsync(VFXType type)
    {
        if (_vfxObjs.TryGetValue(type, out var cachedVfx))
            return cachedVfx;

        string address = $"VFX/{type}_vfx";
        var handle = Addressables.LoadAssetAsync<GameObject>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _vfxObjs[type] = handle.Result;
            return handle.Result;
        }

        Debug.LogWarning($"VFX for type {type} not found.");
        return null;
    }

}
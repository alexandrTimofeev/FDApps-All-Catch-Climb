using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer.Unity;
using System;

public class SFXManager : IStartable
{
    private readonly Dictionary<SFXType, AudioClip> _sfxClips = new();
    private List<AudioClip> _coinClips;
    private bool _isLoaded = false;

    public async void Start()
    {
        await LoadAllSFXAsync();
        await LoadCoinClipsAsync();
    }

    public async UniTask LoadAllSFXAsync()
    {
        if (_isLoaded) return;

        foreach (SFXType type in Enum.GetValues(typeof(SFXType)))
        {
            string address = $"SFX/{type}_sfx";
            AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(address);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _sfxClips[type] = handle.Result;
            }
            else
            {
                Debug.LogWarning($"SFX not found at address: {address}");
            }
        }

        _isLoaded = true;
    }

    public async UniTask LoadCoinClipsAsync()
    {
        if (_coinClips != null && _coinClips.Count > 0)
            return;

        var handle = Addressables.LoadAssetsAsync<AudioClip>("CoinSFX", null); // label "CoinSFX"
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _coinClips = new List<AudioClip>(handle.Result);
        }
        else
        {
            Debug.LogWarning("Coin SFX clips not found by label CoinSFX");
            _coinClips = new List<AudioClip>();
        }
    }

    public async Task<AudioClip> GetClipAsync(SFXType type)
    {
        if (_sfxClips.TryGetValue(type, out var cachedClip))
            return cachedClip;

        string address = $"SFX/{type}_sfx";
        var handle = Addressables.LoadAssetAsync<AudioClip>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _sfxClips[type] = handle.Result;
            return handle.Result;
        }

        Debug.LogWarning($"SFX for type {type} not found.");
        return null;
    }

    public async Task<AudioClip> GetRandomCoinClipAsync()
    {
        if (_coinClips == null || _coinClips.Count == 0)
            await LoadCoinClipsAsync();

        if (_coinClips.Count == 0)
            return null;

        return _coinClips[UnityEngine.Random.Range(0, _coinClips.Count)];
    }
}

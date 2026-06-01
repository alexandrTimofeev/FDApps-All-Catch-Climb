using System.Collections.Generic;
using UnityEngine;
using VContainer;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class Ocean : MonoBehaviour
{
    private PlayerFishingCharacteristics playerFishingCharacteristics;
    private Dictionary<FishRarity, List<FishSO>> fishDictionary = new Dictionary<FishRarity, List<FishSO>>();
    [Tooltip("Higher number - more average fish")]
    [Range(1.1f, 2.5f)]
    [SerializeField] private float exponent;
    [SerializeField] private Transform fishPoint;
    [Header("VFX")]
    [SerializeField] private ParticleSystem waterSplashPS;
    [SerializeField] private float catchFishEffectsDelay;
    [SerializeField] private ParticleSystem increasedSizeVFX;
    [HideInInspector] public bool isReady = false;
    private bool isIncreasedSize = false;
    private SoundPlayer soundPlayer;
    private System.Random random;
    [Inject]
    public void Construct(PlayerController playerController, SoundPlayer _soundPlayer)
    {
        if (playerController == null)
        {
            Debug.LogError($"{nameof(Ocean)}.{nameof(Construct)}: {nameof(playerController)} is null on {name}.");
            return;
        }

        playerFishingCharacteristics = playerController.GetPlayerCharacteristics().GetFishingCharacteristics();
        soundPlayer = _soundPlayer;
    }
    void Awake()
    {
        random = new System.Random();
    }
    private async void Start()
    {
        await LoadFishSOListAsync();
    }
    public Fish GetFish()
    {
        if (playerFishingCharacteristics == null)
        {
            Debug.LogError($"{nameof(Ocean)}.{nameof(GetFish)}: {nameof(playerFishingCharacteristics)} is null on {name}.");
            return null;
        }

        if (fishDictionary == null || fishDictionary.Count == 0)
        {
            Debug.LogWarning("Fish list not loaded yet!");
            return null;
        }

        // Gets random fish from the dictionary
        FishSO fishSO = fishDictionary[playerFishingCharacteristics.GetRandomRarity()].OrderBy(x => random.Next()).First();

        float weight = GetWeightedRandomWeight(fishSO);

        float sizeMult = weight / fishSO.averageSize;

        Fish fish = fishSO.ToFish(weight, sizeMult);
        fish.isCooked = false;

        return fish;
    }
    private float GetWeightedRandomWeight(FishSO fishSO)
    {
        float average = fishSO.averageSize;
        float delta = fishSO.deltaSize;

        float r = UnityEngine.Random.value;

        // Convert in range from [0..1] to [-1..1]
        float normalized = (r - 0.5f) * 2f;

        normalized = Mathf.Sign(normalized) * Mathf.Pow(Mathf.Abs(normalized), exponent);


        float offset = normalized * delta;
        float weight = average + offset;

        float minSize = average - delta;
        float maxSize = average + delta;

        if (isIncreasedSize)
        {
            float multIncrease = UnityEngine.Random.Range(1.5f, 2f);
            weight *= multIncrease;
            minSize *= multIncrease;
            maxSize *= multIncrease;
        }


        return Mathf.Clamp(weight, minSize, maxSize);
    }

    private async UniTask LoadFishSOListAsync()
    {
        List<FishSO> loadedFishSOList;

        AsyncOperationHandle<IList<FishSO>> handle = Addressables.LoadAssetsAsync<FishSO>("FishSO", null);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            loadedFishSOList = new List<FishSO>(handle.Result);
            // Debug.Log($"Loaded {loadedFishSOList.Count} fish assets from Addressables.");
        }
        else
        {
            Debug.LogError("Failed to load FishSO list from Addressables.");
            return;
        }

        foreach (FishRarity rarity in Enum.GetValues(typeof(FishRarity)))
        {
            if (fishDictionary.ContainsKey(rarity) == false)
            {
                fishDictionary.Add(rarity, new List<FishSO>());
            }

            //List<FishSO> fishList = fishDictionary[rarity];

            for (int i = loadedFishSOList.Count - 1; i >= 0; i--)
            {
                var fishSO = loadedFishSOList[i];
                if (fishSO.rarity != rarity) continue;

                fishDictionary[rarity].Add(fishSO);
                loadedFishSOList.RemoveAt(i);
            }
        }

        isReady = true;
    }
    public void ActivateIncreasedSize()
    {
        isIncreasedSize = true;

        if (increasedSizeVFX == null)
        {
            Debug.LogError($"{nameof(Ocean)}.{nameof(ActivateIncreasedSize)}: {nameof(increasedSizeVFX)} is null on {name}.");
            return;
        }

        increasedSizeVFX.Play();
    }
    public void DeactivateIncreasedSize()
    {
        isIncreasedSize = false;

        if (increasedSizeVFX == null)
        {
            Debug.LogError($"{nameof(Ocean)}.{nameof(DeactivateIncreasedSize)}: {nameof(increasedSizeVFX)} is null on {name}.");
            return;
        }

        increasedSizeVFX.Stop();
    }
    public void PlayFishCatchEffectsDelayed()
    {
        DOVirtual.DelayedCall(catchFishEffectsDelay, PlayFishCatchEffects);
    }
    private void PlayFishCatchEffects()
    {
        if (waterSplashPS == null)
        {
            Debug.LogError($"{nameof(Ocean)}.{nameof(PlayFishCatchEffects)}: {nameof(waterSplashPS)} is null on {name}.");
            return;
        }

        waterSplashPS.Play();

        if (isIncreasedSize) increasedSizeVFX.Play();

        if (soundPlayer == null)
        {
            Debug.LogError($"{nameof(Ocean)}.{nameof(PlayFishCatchEffects)}: {nameof(soundPlayer)} is null on {name}.");
            return;
        }

        soundPlayer.PlaySFX(SFXType.WaterSplash, waterSplashPS.transform.position);
    }
    public Vector3 GetFishPointPosition() => fishPoint.position;
}


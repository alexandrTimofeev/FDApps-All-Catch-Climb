using UnityEngine;
using DG.Tweening;
using VContainer;

public class UICatchedFishIcon : MonoBehaviour
{
    [SerializeField] private Transform modelSpotTransform;
    [SerializeField] private Camera camera;
    private FishObjectPool fishObjectPool;
    private FishItem currentFish;
    private Tween rotationTween;

    [Inject]
    public void Construct(FishObjectPool fishObjectPool)
    {
        this.fishObjectPool = fishObjectPool;
    }

    public void SetIcon(Fish fish, float animationLength)
    {
        if (fishObjectPool == null)
        {
            Debug.LogError($"{nameof(UICatchedFishIcon)}.{nameof(SetIcon)}: {nameof(fishObjectPool)} is null on {name}.");
            return;
        }

        if (fish == null)
        {
            Debug.LogError($"{nameof(UICatchedFishIcon)}.{nameof(SetIcon)}: {nameof(fish)} is null on {name}.");
            return;
        }

        if (camera == null)
        {
            Debug.LogError($"{nameof(UICatchedFishIcon)}.{nameof(SetIcon)}: {nameof(camera)} is null on {name}.");
            return;
        }

        if (modelSpotTransform == null)
        {
            Debug.LogError($"{nameof(UICatchedFishIcon)}.{nameof(SetIcon)}: {nameof(modelSpotTransform)} is null on {name}.");
            return;
        }

        if (currentFish != null) RemovePreviousIcon();

        camera.backgroundColor = RarityColorProvider.GetColor(fish.rarity);

        currentFish = fishObjectPool.GetFish(fish);
        if (currentFish == null)
        {
            Debug.LogError($"{nameof(UICatchedFishIcon)}.{nameof(SetIcon)}: {nameof(currentFish)} is null for {fish.nameString} on {name}.");
            return;
        }

        currentFish.transform.SetParent(modelSpotTransform);
        currentFish.transform.localPosition = Vector3.zero;
        currentFish.transform.rotation = Quaternion.identity;

        currentFish._renderer.material = fish.fishMaterial;

        modelSpotTransform.localRotation = Quaternion.identity;

        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();

        float rotationDuration = 4f;
        int loopsCount = Mathf.FloorToInt(animationLength / rotationDuration) + 1;

        rotationTween = modelSpotTransform
            .DOLocalRotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(loopsCount, LoopType.Incremental);
    }

    private void RemovePreviousIcon()
    {
        if (currentFish == null)
            return;

        if (rotationTween != null && rotationTween.IsActive())
            rotationTween.Kill();

        currentFish.transform.SetParent(null);

        fishObjectPool.ReturnFish(currentFish);
    }
}

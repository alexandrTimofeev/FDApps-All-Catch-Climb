using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int maxCapacity;
    [Header("Components")]
    [SerializeField] private Transform inventoryTransform;
    [Header("Form ocean to inventory values")]
    [SerializeField] private float animDuration = 0.5f;
    [SerializeField] private float maxHeight = 4f;
    private FishObjectPool objectPool;
    private SoundPlayer soundPlayer;

    private List<FishItem> items = new List<FishItem>();

    [Inject]
    public void Construct(FishObjectPool objectPool, SoundPlayer soundPlayer)
    {
        this.objectPool = objectPool;
        this.soundPlayer = soundPlayer;
    }

    public void AddFishFromOcean(Fish fish, Vector3 startPosition)
    {
        if (!ValidateDependencies(nameof(AddFishFromOcean)) || fish == null)
        {
            if (fish == null)
                Debug.LogError($"{nameof(PlayerInventory)}.{nameof(AddFishFromOcean)}: {nameof(fish)} is null on {name}.");
            return;
        }

        var fishItem = objectPool.GetFish(fish);
        if (fishItem == null)
        {
            Debug.LogError($"{nameof(PlayerInventory)}.{nameof(AddFishFromOcean)}: {nameof(fishItem)} is null for {fish.nameString} on {name}.");
            return;
        }

        var fishTransform = fishItem.transform;

        fishItem.fish = fish;
        fishItem._renderer.material = fish.fishMaterial;
        fishTransform.localScale = fish.sizeMult * fishItem.defaultScaleFloat * Vector3.one;
        fishTransform.position = startPosition;
        fishTransform.SetParent(inventoryTransform, true);
        fishTransform.Rotate(Vector3.up * 180);
        fishTransform.Rotate(Vector3.right * -85);

        Vector3 targetPosition = ConfigureTargetPosition(fishItem);

        Vector3 middlePoint = (fishTransform.localPosition + targetPosition) / 2f;
        middlePoint.y = maxHeight;

        Vector3[] path = new Vector3[] { fishTransform.localPosition, middlePoint, targetPosition };

        // Rotation
        fishTransform.DOLocalRotate(new Vector3(85, 180, 0), animDuration)
            .SetEase(Ease.Linear);

        // Wiggling of the fish
        Tween splashTween = DOVirtual.Float(-20f, 20f, 0.12f, y =>
        {
            Vector3 rot = fishTransform.localEulerAngles;
            rot.y = 180 + y;
            fishTransform.localEulerAngles = rot;
        })
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine);

        // Parabloc movement
        fishTransform
            .DOLocalPath(path, animDuration, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                splashTween.Kill();
                fishTransform.DOLocalRotate(new Vector3(0, -90, 90), 0.1f);
                soundPlayer.PlaySFX(SFXType.FishCatchedSuccess);
            });

        items.Add(fishItem);
    }

    public void AddFishItem(FishItem fishItem, bool animateMovement, bool playSFX)
    {
        if (!ValidateDependencies(nameof(AddFishItem)) || fishItem == null)
        {
            if (fishItem == null)
                Debug.LogError($"{nameof(PlayerInventory)}.{nameof(AddFishItem)}: {nameof(fishItem)} is null on {name}.");
            return;
        }

        var fishTransform = fishItem.transform;

        DOTween.Kill(fishTransform);

        if (animateMovement) // Moves fish to inventory with animation
        {
            fishTransform.SetParent(inventoryTransform);

            Vector3 targetPosition = ConfigureTargetPosition(fishItem);

            fishTransform.DOLocalRotate(new Vector3(0, -90, 90), 0.2f, RotateMode.FastBeyond360);
            fishTransform.DOLocalMove(targetPosition, 0.25f).SetEase(Ease.InQuad).OnComplete(() =>
            {
                if (playSFX) soundPlayer.PlaySFX(SFXType.FishPickedUp, null);
            });
        }
        else
        {
            SetFishStrictPosition(fishItem);
        }

        items.Add(fishItem);
    }
    public FishItem TakeFirstFish(FishFilter fishToPickup)
    {
        if (items.Count == 0) return null;

        for (int i = items.Count - 1; i >= 0; i--)
        {
            FishItem fish = items[i];

            if (FilterCheck(fish, fishToPickup))
            {
                fish.transform.SetParent(null);

                items.RemoveAt(i);

                RepositionInventoryFish(i);

                return fish;
            }
        }

        return null;
    }
    private bool FilterCheck(FishItem fish, FishFilter fishToPickup)
    {
        bool isCorrectFish = false;
        switch (fishToPickup)
        {
            case FishFilter.All:
                isCorrectFish = true;
                break;
            case FishFilter.Raw:
                isCorrectFish = !fish.fish.isCooked;
                break;
            case FishFilter.Cooked:
                isCorrectFish = fish.fish.isCooked;
                break;
        }

        return isCorrectFish;
    }
    public bool HasSpace() => items.Count < maxCapacity;

    // Calculate position where fish should be placed
    private Vector3 ConfigureTargetPosition(FishItem fishItem)
    {
        float currentY = 0f;

        // Calculating previous fish height
        for (int i = 0; i < items.Count; i++)
        {
            currentY += items[i].fish.width;
        }

        currentY += fishItem.fish.width / 2f;

        return Vector3.up * currentY;
    }

    // Set strict position of fish in inventory if it does not use animations
    private void SetFishStrictPosition(FishItem fishItem)
    {
        var fishTransform = fishItem.transform;

        fishTransform.SetParent(inventoryTransform);
        fishTransform.localPosition = Vector3.zero;
        fishTransform.localRotation = Quaternion.identity;

        Vector3 targetPosition = ConfigureTargetPosition(fishItem);

        fishTransform.localPosition = targetPosition;

        fishTransform.Rotate(Vector3.up * -90);
        fishTransform.Rotate(Vector3.forward * 90);
    }
    private void RepositionInventoryFish(int index)
    {
        if (index >= items.Count) return;

        float currentY = 0f;

        // Calculating starting Y position
        for (int i = 0; i < index; i++)
        {
            float heightOffset = items[i].fish.width;
            currentY += heightOffset;
        }

        // Reposition all fish, starting from INDEX
        for (int i = index; i < items.Count; i++)
        {
            var fishItem = items[i];
            var fishTransform = fishItem.transform;

            float halfHeight = fishItem.fish.width / 2f;
            currentY += halfHeight;

            Vector3 newPosition = Vector3.up * currentY;

            DOTween.Kill(fishTransform);
            fishTransform.DOLocalMove(newPosition, 0.2f).SetEase(Ease.OutQuad);

            currentY += halfHeight;
        }
    }
    public int ItemsAmount() => items.Count;
    public void SetMaxCapacity(int i)
    {
        maxCapacity = i;
    }

    private bool ValidateDependencies(string caller)
    {
        if (objectPool == null) return LogMissingDependency(nameof(objectPool), caller);
        if (soundPlayer == null) return LogMissingDependency(nameof(soundPlayer), caller);
        if (inventoryTransform == null) return LogMissingDependency(nameof(inventoryTransform), caller);
        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{nameof(PlayerInventory)}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }

#if UNITY_EDITOR
    // Code for testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeFirstFish(FishFilter.All);
        }
    }
#endif
}
public enum FishFilter
{
    All,
    Raw,
    Cooked
}

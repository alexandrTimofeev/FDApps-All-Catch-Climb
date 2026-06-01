using System.Collections.Generic;
using UnityEngine;
using VContainer;
using DG.Tweening;
using System;

public class CookingSpot : MonoBehaviour
{
    [Tooltip("Cooldown between each fish taken from player inventory")]
    [SerializeField] public float cooldownToPickupFish;
    [Tooltip("Time delay to cook one fish")]
    [SerializeField] private float cookDelay;
    [Tooltip("Delay between fish being taken from cooking spot to a player")]
    [SerializeField] public float outputDelay;
    [Header("Components")]
    [SerializeField] private Transform inputFishStackPos;
    [SerializeField] private Transform cookingPos;
    [SerializeField] private Transform outputFishStackPos;
    [Space(15)]
    [SerializeField] private Material cookedMaterial;
    [Space(15)]
    [SerializeField] public GameObject cookPf;
    private SoundPlayer soundPlayer;
    private CookingSpotFryingFishSound fryingFishSound;
    private CookingSpotVFX cookingSpotVFX;
    private Stack<FishItem> fishToCook = new Stack<FishItem>();
    private Stack<FishItem> fishReady = new Stack<FishItem>();
    private StartCooking_event startCooking_Event = new();
    private StopCooking_event stopCooking_Event = new();
    private FishItem currentCookingFish;
    private float cookDelayDelta = 0;

    [Inject]
    public void Construct(SoundPlayer soundPlayer, CookingSpotFryingFishSound fryingFishSound, CookingSpotVFX cookingSpotVFX)
    {
        this.soundPlayer = soundPlayer;
        this.fryingFishSound = fryingFishSound;
        this.cookingSpotVFX = cookingSpotVFX;
    }

    public void CookFish()
    {
        if (!ValidateDependencies(nameof(CookFish)))
            return;

        cookDelayDelta -= Time.deltaTime;

        if (fishToCook.Count == 0 && currentCookingFish == null) return;

        if (cookDelayDelta <= 0)
        {
            // Finishing cooking last fish if it is not null
            if (currentCookingFish != null)
            {
                EventBus.Publish(stopCooking_Event);
                FinishCookingCurrentFish();

                fryingFishSound.MuteAudio();
            }

            // Take new fish to cook if there are any left
            if (fishToCook.Count > 0)
            {
                EventBus.Publish(startCooking_Event);

                currentCookingFish = fishToCook.Pop();

                CookCurrentFish();

                fryingFishSound.EnableAudio();
            }

            cookDelayDelta = cookDelay;
        }
    }
    public void InputFish(FishItem fishItem)
    {
        Action action = () => OnInputFish(fishItem);

        HelpUtilities.Player.PickupFishAndPutToStack(fishItem, inputFishStackPos, fishToCook, action);
    }
    private void OnInputFish(FishItem fishItem)
    {
        if (!ValidateDependencies(nameof(OnInputFish)) || fishItem == null)
        {
            if (fishItem == null)
                Debug.LogError($"{nameof(CookingSpot)}.{nameof(OnInputFish)}: {nameof(fishItem)} is null on {name}.");
            return;
        }

        soundPlayer.PlaySFX(SFXType.FishPlaced, null, 0.8f);

        Vector3 position = fishItem.transform.position;
        position.y -= fishItem.fish.width;

        cookingSpotVFX.SetFishSplashPosition(position);
        cookingSpotVFX.PlayFishSplashVFX();
    }
    public FishItem OutputFish()
    {
        FishItem fish = null;

        if (fishReady.Count != 0)
        {
            fish = fishReady.Pop();
        }

        return fish;
    }
    private void CookCurrentFish()
    {
        PlayCurrentFishToCookingSpotAnimation();
    }
    private void FinishCookingCurrentFish()
    {
        if (currentCookingFish == null)
        {
            Debug.LogError($"{nameof(CookingSpot)}.{nameof(FinishCookingCurrentFish)}: {nameof(currentCookingFish)} is null on {name}.");
            return;
        }

        currentCookingFish.fish.isCooked = true;
        currentCookingFish._renderer.material = cookedMaterial;

        PlayOutputAnimation(currentCookingFish);

        fishReady.Push(currentCookingFish);

        currentCookingFish = null;
    }
    private void PlayOutputAnimation(FishItem fishItem)
    {
        // DO: Upgrade animation

        fishItem.transform.SetParent(null);

        Vector3 localTargetPosition = HelpUtilities.Player.CalculateTargetLocalPositionStack(fishItem, fishReady);

        Vector3 worldTargetPos = outputFishStackPos.TransformPoint(localTargetPosition);

        float animationDelay = 0.2f;

        fishItem.transform.DOMove(worldTargetPos, animationDelay).SetEase(Ease.OutQuad);
    }

    private void PlayCurrentFishToCookingSpotAnimation()
    {
        // DO: Upgrade animation
        Sequence sequence = DOTween.Sequence();

        DOTween.Kill(currentCookingFish.transform);

        currentCookingFish.transform.SetParent(cookingPos, worldPositionStays: true);

        Vector3 targetPosition = Vector3.up * currentCookingFish.fish.width / 2;

        float animationDelay = 0.3f;

        sequence.Append(currentCookingFish.transform.DOLocalMove(targetPosition, animationDelay).SetEase(Ease.OutQuad));
        sequence.Join(currentCookingFish.transform.DORotate(new Vector3(0f, -90f, -90f), animationDelay).SetEase(Ease.OutQuad));
    }
    public bool HasFish() => fishReady.Count > 0;
    public bool HasCurrentFish() => currentCookingFish != null;
    public void ResetTimer() => cookDelayDelta = 0;
    public float GetFillAmount() => 1f - cookDelayDelta / cookDelay;

    private bool ValidateDependencies(string caller)
    {
        if (soundPlayer == null) return LogMissingDependency(nameof(soundPlayer), caller);
        if (fryingFishSound == null) return LogMissingDependency(nameof(fryingFishSound), caller);
        if (cookingSpotVFX == null) return LogMissingDependency(nameof(cookingSpotVFX), caller);
        if (inputFishStackPos == null) return LogMissingDependency(nameof(inputFishStackPos), caller);
        if (cookingPos == null) return LogMissingDependency(nameof(cookingPos), caller);
        if (outputFishStackPos == null) return LogMissingDependency(nameof(outputFishStackPos), caller);
        if (cookedMaterial == null) return LogMissingDependency(nameof(cookedMaterial), caller);
        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{nameof(CookingSpot)}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }
}

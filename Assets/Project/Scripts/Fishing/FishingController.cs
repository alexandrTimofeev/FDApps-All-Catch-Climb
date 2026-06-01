using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using VContainer;

public class FishingController : MonoBehaviour
{
    public event Action OnExit;
    [Tooltip("Time to catch fish on a hook")]
    [SerializeField] private float timeToCatch;
    [SerializeField] private FishingZone fishingZone;
    private UIFishing fishingUI;
    private UIFishingBar fishingBarUI;
    private UICatchedFish catchedFishUI;
    private PlayerInventory playerInventory;
    private Ocean ocean;
    private SoundPlayer soundPlayer;
    private Fish currentFish;
    private bool isFishing = false;
    private bool isDoubleCatch = false;

    [Inject]
    public void Construct(
        UIFishing fishingUI,
        UIFishingBar fishingBarUI,
        UICatchedFish catchedFishUI,
        PlayerInventory playerInventory,
        Ocean ocean,
        SoundPlayer soundPlayer)
    {
        this.fishingUI = fishingUI;
        this.fishingBarUI = fishingBarUI;
        this.catchedFishUI = catchedFishUI;
        this.playerInventory = playerInventory;
        this.ocean = ocean;
        this.soundPlayer = soundPlayer;
    }

    void Start()
    {
        if (!ValidateDependencies(nameof(Start)))
            return;

        EventBus.Subscribe<CatchPressed_event>(CatchFish);
        EventBus.Subscribe<ExitPressed_event>(OnPlayerExit);
    }
    public void StartFishing()
    {
        if (!ValidateDependencies(nameof(StartFishing)))
            return;

        if (ocean.isReady == false)
        {
            UIWarningPopup.Instance.ShowWarning("Come back later", 2f);
            ExitFishing();
            return;
        }

        fishingUI.ShowUI();

        StartCoroutine(nameof(GetFishOnHook));
    }
    private IEnumerator GetFishOnHook()
    {
        if (!ValidateDependencies(nameof(GetFishOnHook)))
            yield break;

        if (!playerInventory.HasSpace())
        {
            ExitFishing();
            yield break;
        }

        yield return new WaitForSeconds(timeToCatch);

        if (!playerInventory.HasSpace())
        {
            ExitFishing();
            yield break;
        }

        currentFish = ocean.GetFish();
        if (currentFish == null)
        {
            Debug.LogError($"{nameof(FishingController)}.{nameof(GetFishOnHook)}: {nameof(currentFish)} is null on {name}.");
            ExitFishing();
            yield break;
        }

        fishingBarUI.PrepareFishing(currentFish);
        fishingBarUI.ShowUI();
        isFishing = true;
    }

    private void CatchFish(CatchPressed_event e)
    {
        if (!isFishing) return;
        if (currentFish == null)
        {
            Debug.LogError($"{nameof(FishingController)}.{nameof(CatchFish)}: {nameof(currentFish)} is null on {name}.");
            ExitFishing();
            return;
        }

        bool isSuccess = fishingBarUI.CatchFish();

        if (isSuccess && playerInventory.HasSpace())
        {
            playerInventory.AddFishFromOcean(currentFish, ocean.GetFishPointPosition());

            catchedFishUI.OpenUI(currentFish);
        }

        if (isSuccess)
        {
            ocean.PlayFishCatchEffectsDelayed();

            HandleDoubleCatch();
        }
        else
        {
            soundPlayer.PlaySFX(SFXType.FishCatchedFail, null);
        }

        isFishing = false;

        fishingBarUI.HideUI();

        StartCoroutine(nameof(GetFishOnHook));
    }
    private void OnPlayerExit(ExitPressed_event e)
    {
        ExitFishing();
    }
    private void ExitFishing()
    {
        if (isFishing)
        {
            fishingBarUI.HideUI();
        }

        isFishing = false;

        fishingUI.HideUI();

        StopCoroutine(nameof(GetFishOnHook));

        OnExit?.Invoke();
    }
    private void HandleDoubleCatch()
    {
        if (isDoubleCatch)
        {
            int rndNum = UnityEngine.Random.Range(0, 2);

            if (rndNum != 0) return;

            Fish fishClone = currentFish.Clone();

            DOVirtual.DelayedCall(0.3f, () =>
            {
                if (playerInventory.HasSpace())
                {
                    playerInventory.AddFishFromOcean(fishClone, ocean.GetFishPointPosition());

                    //TODO: Play VFX and SFX of double catch

                    ocean.PlayFishCatchEffectsDelayed();
                }
            });
        }
    }
    public void ActivateDoubleCatch() => isDoubleCatch = true;
    public void DeactivateDoubleCatch() => isDoubleCatch = false;
    public FishingZone GetFishingZone() => fishingZone;
    public void InjectDependency(FishingZone fishingZone)
    {
        this.fishingZone = fishingZone;
    }

    private bool ValidateDependencies(string caller)
    {
        if (fishingUI == null) return LogMissingDependency(nameof(fishingUI), caller);
        if (fishingBarUI == null) return LogMissingDependency(nameof(fishingBarUI), caller);
        if (catchedFishUI == null) return LogMissingDependency(nameof(catchedFishUI), caller);
        if (playerInventory == null) return LogMissingDependency(nameof(playerInventory), caller);
        if (ocean == null) return LogMissingDependency(nameof(ocean), caller);
        if (soundPlayer == null) return LogMissingDependency(nameof(soundPlayer), caller);
        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{nameof(FishingController)}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }
}

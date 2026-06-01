using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using Unity.Assets.Project.Scripts.Enums;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public abstract class Upgrade : MonoBehaviour
{
    protected UpgradesController upgradesController;
    protected PlayerMoneyBalance playerMoneyBalance;
    protected TimerManager timerManager;
    public event Action<UpgradePurchased_event> OnPurchased;

    [SerializeField] private UpgradeConfigComponents configComponents;
    protected SoundPlayer soundPlayer;
    protected VFXPlayer vfxPlayer;
    public UpgradeSO upgradeSO;
    private SmartTimer buyingTimer;
    [HideInInspector]
    public bool isPurchased = false;
    private Color clearColor = new Color(1, 1, 1, 0);
    public abstract void LoadUpgrade();
    protected abstract void ActivateUpgrade();
    public virtual void OnPurchase() { }
    public virtual void PurchaseUpgrade()
    {
        //TODO: Play some SFX and VFX
        playerMoneyBalance.DiscardMoney(upgradeSO.cost);

        soundPlayer.PlaySFX(SFXType.UpgradeUnlocked);
        isPurchased = true;
        EventBus.Publish(new UpgradePurchased_event { upgrade = this });

        vfxPlayer.PlayVFX(VFXType.MagicPoof, transform.position + Vector3.up);

        ActivateUpgrade();

        //Debug.Log($"Unlocked {upgradeSO.uniqueName} upgrade");

        OnPurchase();
    }
    [Inject]
    public void ConstructBase(
        UpgradesController _upgradesController,
        PlayerMoneyBalance _playerMoneyBalance,
        TimerManager _timerManager,
        SoundPlayer soundPlayer,
        VFXPlayer vfxPlayer)
    {
        upgradesController = _upgradesController;
        playerMoneyBalance = _playerMoneyBalance;
        timerManager = _timerManager;
        this.soundPlayer = soundPlayer;
        this.vfxPlayer = vfxPlayer;

        if (upgradesController != null)
            upgradesController.Register(this);
        else
            Debug.LogError($"{nameof(Upgrade)}.{nameof(ConstructBase)}: {nameof(upgradesController)} is null on {name}.");
    }
    private void Awake()
    {
        if (!ValidateSerializedReferences(nameof(Awake)))
            return;

        OnAwake();
        configComponents.costText.SetText(upgradeSO.cost.ToString());

        configComponents.descriptionText.SetText(upgradeSO.descriptionText);
        configComponents.descriptionText.color = clearColor;

        configComponents.buyingProgressImage.fillAmount = 0;
        configComponents.iconImage.sprite = upgradeSO.icon;

        configComponents.buyingZone.OnPlayerEnter += PlayerEnter;
        configComponents.buyingZone.OnPlayerExit += PlayerExit;

        configComponents.interactZone.OnPlayerEnter += ShowDescription;
        configComponents.interactZone.OnPlayerExit += HideDescription;
    }

    public virtual void OnAwake() { }
    public void PlayerEnter()
    {
        if (!ValidateDependencies(nameof(PlayerEnter)))
            return;

        if (!playerMoneyBalance.IsAffordable(upgradeSO.cost))
        {
            UIWarningPopup.Instance.ShowWarning("Not enough money!", 1f);
            return;
        }

        if (buyingTimer != null) return;

        buyingTimer = timerManager.CreateTimer(2f, () =>
        {
            if (playerMoneyBalance.IsAffordable(upgradeSO.cost))
            {
                PurchaseUpgrade();
            }
        });

        StartCoroutine(UpdateBuyingProgress(buyingTimer));
    }
    public void PlayerExit()
    {
        if (buyingTimer != null)
        {
            buyingTimer.Stop();
            configComponents.buyingProgressImage.fillAmount = 0f;
            buyingTimer = null;
        }
    }
    private IEnumerator UpdateBuyingProgress(SmartTimer timer)
    {
        while (!timer.IsFinished)
        {
            configComponents.buyingProgressImage.fillAmount = 1f - (timer.TimeLeft / timer.Duration);
            yield return null;
        }

        configComponents.buyingProgressImage.fillAmount = 0f;
        buyingTimer = null;
    }
    public void HideUpgrade()
    {
        gameObject.SetActive(false);
    }
    public void ShowUpgrade()
    {
        gameObject.SetActive(true);
    }
    private void ShowDescription()
    {
        if (configComponents == null || configComponents.descriptionText == null)
        {
            Debug.LogError($"{nameof(Upgrade)}.{nameof(ShowDescription)}: description text is null on {name}.");
            return;
        }

        configComponents.descriptionText.gameObject.SetActive(true);

        configComponents.descriptionText.DOColor(Color.white, 0.3f).SetEase(Ease.OutQuad);
    }
    private void HideDescription()
    {
        if (configComponents == null || configComponents.descriptionText == null)
        {
            Debug.LogError($"{nameof(Upgrade)}.{nameof(HideDescription)}: description text is null on {name}.");
            return;
        }

        configComponents.descriptionText.DOColor(clearColor, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            configComponents.descriptionText.gameObject.SetActive(false);
        });
    }

    private bool ValidateDependencies(string caller)
    {
        if (playerMoneyBalance == null) return LogMissingDependency(nameof(playerMoneyBalance), caller);
        if (timerManager == null) return LogMissingDependency(nameof(timerManager), caller);
        if (soundPlayer == null) return LogMissingDependency(nameof(soundPlayer), caller);
        if (vfxPlayer == null) return LogMissingDependency(nameof(vfxPlayer), caller);
        if (upgradeSO == null) return LogMissingDependency(nameof(upgradeSO), caller);
        if (configComponents == null) return LogMissingDependency(nameof(configComponents), caller);
        return true;
    }

    private bool ValidateSerializedReferences(string caller)
    {
        if (upgradeSO == null) return LogMissingDependency(nameof(upgradeSO), caller);
        if (configComponents == null) return LogMissingDependency(nameof(configComponents), caller);
        if (configComponents.costText == null) return LogMissingDependency($"{nameof(configComponents)}.{nameof(configComponents.costText)}", caller);
        if (configComponents.descriptionText == null) return LogMissingDependency($"{nameof(configComponents)}.{nameof(configComponents.descriptionText)}", caller);
        if (configComponents.buyingProgressImage == null) return LogMissingDependency($"{nameof(configComponents)}.{nameof(configComponents.buyingProgressImage)}", caller);
        if (configComponents.iconImage == null) return LogMissingDependency($"{nameof(configComponents)}.{nameof(configComponents.iconImage)}", caller);
        if (configComponents.buyingZone == null) return LogMissingDependency($"{nameof(configComponents)}.{nameof(configComponents.buyingZone)}", caller);
        if (configComponents.interactZone == null) return LogMissingDependency($"{nameof(configComponents)}.{nameof(configComponents.interactZone)}", caller);
        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{GetType().Name}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }
}

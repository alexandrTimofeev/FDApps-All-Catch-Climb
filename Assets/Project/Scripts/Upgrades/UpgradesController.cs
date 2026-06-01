using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
public class UpgradesController
{
    private List<Upgrade> upgradesList = new();
    private List<Upgrade> upgradesLeft = new();
    private List<Upgrade> startingUpgrades = new();
    private List<string> unlockedUpgradeIDs = new();
    private SaveService saveService;
    private CancellationTokenSource cts;
    public UpgradesController()
    {
        cts = new CancellationTokenSource();
        EventBus.Subscribe<UpgradePurchased_event>(OnUpgradePurchased);
    }

    [Inject]
    public void Construct(SaveService saveService)
    {
        this.saveService = saveService;
    }

    public void Register(Upgrade upgrade)
    {
        if (upgrade == null)
        {
            Debug.LogError($"{nameof(UpgradesController)}.{nameof(Register)}: {nameof(upgrade)} is null.");
            return;
        }

        if (upgradesList.Contains(upgrade))
            return;

        upgradesList.Add(upgrade);
    }
    public UniTask LoadAllUpgrades()
    {
        if (saveService == null)
        {
            Debug.LogError($"{nameof(UpgradesController)}.{nameof(LoadAllUpgrades)}: {nameof(saveService)} is null.");
            return UniTask.CompletedTask;
        }

        unlockedUpgradeIDs = saveService.GetUnlockedUpgrades();
        List<Upgrade> loadedUpgrades = new();

        startingUpgrades = new List<Upgrade>(upgradesList);

        foreach (Upgrade upgrade in upgradesList)
        {
            if (unlockedUpgradeIDs.Contains(upgrade.upgradeSO.uniqueName))
            {
                loadedUpgrades.Add(upgrade);
                startingUpgrades.Remove(upgrade);
            }
        }

        foreach (Upgrade upgrade in loadedUpgrades)
            upgrade.LoadUpgrade();

        InitializeStartingUpgrades();

        return UniTask.CompletedTask;
    }


    private void InitializeStartingUpgrades()
    {
        upgradesLeft = new List<Upgrade>(startingUpgrades);

        foreach (Upgrade currentUpgrade in startingUpgrades)
        {
            UpgradeSO upgradeSO = currentUpgrade.upgradeSO;

            foreach (Upgrade upgrade in startingUpgrades)
            {
                if (upgrade.upgradeSO.nextUpgradesList.Contains(upgradeSO))
                {
                    // startingUpgrades.Remove(currentUpgrade);
                    currentUpgrade.HideUpgrade();
                    break;
                }
            }
        }
    }
    private void OnUpgradePurchased(UpgradePurchased_event e)
    {
        HandleUpgradePurchasedAsync(e).Forget();

        
    }
    private async UniTaskVoid HandleUpgradePurchasedAsync(UpgradePurchased_event e)
    {
        if (saveService == null)
        {
            Debug.LogError($"{nameof(UpgradesController)}.{nameof(HandleUpgradePurchasedAsync)}: {nameof(saveService)} is null.");
            return;
        }

        upgradesLeft.Remove(e.upgrade);

        string id = e.upgrade.upgradeSO.uniqueName;
        List<string> unlockedIDs = saveService.GetUnlockedUpgrades();

        if (!unlockedIDs.Contains(id))
        {
            unlockedIDs.Add(id);
            await saveService.SetUnlockedUpgradesAsync(unlockedIDs, cts.Token);
        }

        foreach (UpgradeSO nextSO in e.upgrade.upgradeSO.nextUpgradesList)
        {
            Upgrade nextUpgrade = upgradesLeft.Find(u => u.upgradeSO == nextSO);
            if (nextUpgrade == null) continue;

            bool allParentsPurchased = true;

            foreach (Upgrade potentialParent in upgradesList)
            {
                if (potentialParent.upgradeSO.nextUpgradesList.Contains(nextSO) &&
                    upgradesLeft.Contains(potentialParent))
                {
                    allParentsPurchased = false;
                    break;
                }
            }

            if (allParentsPurchased)
            {
                nextUpgrade.ShowUpgrade();
            }
        }
    }
    public void Dispose()
    {
        cts.Cancel();
        cts.Dispose();
    }

}

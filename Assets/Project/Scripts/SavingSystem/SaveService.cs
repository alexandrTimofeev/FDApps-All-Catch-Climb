using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SaveService
{
    private const string SaveFileName = "savefile.json";
    private SaveData runtimeData = new();
    private bool isLoaded = false;

    private string GetSavePath() => Path.Combine(Application.persistentDataPath, SaveFileName);

    public async UniTask InitAsync(CancellationToken ct = default)
    {
        if (isLoaded) return;

        string path = GetSavePath();
        if (File.Exists(path))
        {
            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(path), cancellationToken: ct);
            if (!string.IsNullOrEmpty(json))
                runtimeData = JsonUtility.FromJson<SaveData>(json) ?? new SaveData();
        }

        isLoaded = true;
    }
    public async UniTask SaveToDiskAsync(CancellationToken ct = default)
    {
        string path = GetSavePath();
        string json = JsonUtility.ToJson(runtimeData, true);

        await UniTask.RunOnThreadPool(() =>
        {
            if (!ct.IsCancellationRequested)
            {
                File.WriteAllText(path, json);
            }
        }, cancellationToken: ct);
    }

    public List<string> GetUnlockedUpgrades()
    {
        return runtimeData.unlockedUpgradeIDs;
    }

    public async UniTask SetUnlockedUpgradesAsync(List<string> ids, CancellationToken ct = default)
    {
        runtimeData.unlockedUpgradeIDs = ids;
        await SaveToDiskAsync(ct);
    }
    public int GetSavedBalance()
    {
        return runtimeData.savedBalance;
    }

    public async void SaveBalance(int additionalMoney)
    {
        await SaveBalanceAsync(additionalMoney);
    }
    public async UniTask SaveBalanceAsync(int additionalMoney, CancellationToken ct = default)
    {
        runtimeData.savedBalance += additionalMoney;
        await SaveToDiskAsync(ct);
    }

    public async UniTask DeleteSaveAsync(CancellationToken ct = default)
    {
        string path = GetSavePath();

        PlayerPrefs.DeleteAll();

        await UniTask.RunOnThreadPool(() =>
        {
            if (File.Exists(path))
                File.Delete(path);
        }, cancellationToken: ct);

        runtimeData = new SaveData();
        isLoaded = false;
    }
}

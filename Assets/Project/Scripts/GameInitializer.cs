using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class GameInitializer
{
    private readonly SaveService saveService;
    private readonly UpgradesController upgradesController;
    private readonly PlayerMoneyBalance playerMoneyBalance;
    private readonly MusicController musicController;
    private readonly DailyVisitLogger dailyVisitLogger;

    [Inject]
    public GameInitializer(SaveService saveService, UpgradesController upgradesController, PlayerMoneyBalance playerMoneyBalance, MusicController musicController)
    {
        this.saveService = saveService;
        this.upgradesController = upgradesController;
        this.playerMoneyBalance = playerMoneyBalance;
        this.musicController = musicController;

        dailyVisitLogger = new DailyVisitLogger();
    }

    public async UniTask InitializeAsync()
    {
        if (saveService == null)
        {
            Debug.LogError($"{nameof(GameInitializer)}: {nameof(saveService)} is null.");
            return;
        }

        if (upgradesController == null)
        {
            Debug.LogError($"{nameof(GameInitializer)}: {nameof(upgradesController)} is null.");
            return;
        }

        if (playerMoneyBalance == null)
        {
            Debug.LogError($"{nameof(GameInitializer)}: {nameof(playerMoneyBalance)} is null.");
            return;
        }

        if (musicController == null)
        {
            Debug.LogError($"{nameof(GameInitializer)}: {nameof(musicController)} is null.");
            return;
        }

        await saveService.InitAsync();
        //await saveService.DeleteSaveAsync();
        await upgradesController.LoadAllUpgrades();

        await InitializeFireBase();

        playerMoneyBalance.InitializeAsync();
        musicController.StartPlaying();

        dailyVisitLogger.Log();
    }
    private async UniTask InitializeFireBase()
    {
        
    }
}

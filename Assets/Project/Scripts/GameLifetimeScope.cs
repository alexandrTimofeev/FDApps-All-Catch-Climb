using VContainer;
using VContainer.Unity;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class GameLifetimeScope : LifetimeScope
{
    [Header("Player")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerStateController playerStateController;
    [SerializeField] private PlayerStepsSound playerStepsSound;
    [SerializeField] private PlayerFootstepVFX playerFootstepVFX;
    [Space(10)]
    [SerializeField] private FishingController fishingController;
    [SerializeField] private FishObjectPool fishObjectPool;
    [SerializeField] private CoinSpawner coinSpawner;
    [Space(10)]
    [SerializeField] private Ocean ocean;
    [SerializeField] private FishingZone fishingZone;
    [SerializeField] private FishingRod fishingRod;

    [Header("Cooking Spot")]
    [SerializeField] private CookingSpot cookingSpot;
    [SerializeField] private CookingSpotPickupZone cookingSpotPickupZone;
    [SerializeField] private CookingSpotOutputZone cookingSpotOutputZone;
    [SerializeField] private CookingSpotCookingZone cookingSpotCookingZone;
    [SerializeField] private CookingSpotFryingFishSound cookingSpotFryingFishSound;
    [SerializeField] private CookingSpotVFX cookingSpotVFX;
    [Header("Selling Spot")]
    [SerializeField] private SellingSpot sellingSpot;
    [SerializeField] private SellingSpotPickupZone sellingSpotPickupZone;
    [SerializeField] private SellingSpotOutputZone sellingSpotOutputZone;
    [SerializeField] private SellingSpotSellZone sellingSpotSellZone;
    [SerializeField] private SellingSpotVFX sellingSpotVFX;
    [SerializeField] private CustomersLine customersLine;
    [Header("UI")]
    [SerializeField] private UIFishing fishingUI;
    [SerializeField] private UIFishingBar fishingBarUI;
    [SerializeField] private UICatchedFish catchedFishUI;
    [SerializeField] private UICatchedFishIcon catchedFishIconUI;
    [SerializeField] private UIPlayerBalance playerBalanceUI;
    [SerializeField] private UISellingProgressBar sellingProgressBarUI;
    [SerializeField] private UICookingProgressBar cookingProgressBarUI;
    [SerializeField] private UIRewardedAdPopup rewardedAdPopupUI;
    [SerializeField] private UIBuffController buffControllerUI;
    [SerializeField] private UISettings settingsUI;
    [Header("Systems")]
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private MusicController musicController;
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private VFXPlayer vfxPlayer;
    [SerializeField] private VFXManager vfxManager;
    private UpgradesController upgradesController;
    private GameInitializer gameInitializer;
    private PlayerMoneyBalance playerMoneyBalance;
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<PlayerCharacteristics>(Lifetime.Singleton);
        builder.RegisterComponent(playerController).AsSelf();
        builder.RegisterComponent(playerInventory).AsSelf();
        builder.RegisterComponent(playerMovement).AsSelf();
        builder.RegisterComponent(playerStateController).AsSelf();
        builder.RegisterComponent(playerStepsSound).AsSelf();
        builder.RegisterComponent(playerFootstepVFX).AsSelf();

        playerMoneyBalance = new PlayerMoneyBalance();
        builder.RegisterInstance(playerMoneyBalance);
        builder.Register<GameInitializer>(Lifetime.Singleton);
        builder.Register<SaveService>(Lifetime.Singleton);
        builder.Register<BuffController>(Lifetime.Singleton);

        builder.Register<SFXManager>(Lifetime.Singleton).As<IStartable>().AsSelf();

        upgradesController = new UpgradesController();
        builder.RegisterInstance(upgradesController);

        builder.RegisterComponent(soundPlayer).AsSelf();

        builder.RegisterComponent(fishingController).AsSelf();
        builder.RegisterComponent(fishObjectPool).AsSelf();
        builder.RegisterComponent(fishingRod).AsSelf();
        builder.RegisterComponent(fishingZone).AsSelf();
        builder.RegisterComponent(coinSpawner).AsSelf();

        builder.RegisterComponent(cookingSpot).AsSelf();
        builder.RegisterComponent(cookingSpotPickupZone).AsSelf();
        builder.RegisterComponent(cookingSpotCookingZone).AsSelf();
        builder.RegisterComponent(cookingSpotOutputZone).AsSelf();
        builder.RegisterComponent(cookingSpotFryingFishSound).AsSelf();
        builder.RegisterComponent(cookingSpotVFX).AsSelf();

        builder.RegisterComponent(sellingSpot).AsSelf();
        builder.RegisterComponent(sellingSpotPickupZone).AsSelf();
        builder.RegisterComponent(sellingSpotSellZone).AsSelf();
        builder.RegisterComponent(sellingSpotOutputZone).AsSelf();
        builder.RegisterComponent(sellingSpotVFX).AsSelf();
        builder.RegisterComponent(customersLine).AsSelf();

        builder.RegisterComponent(ocean).AsSelf();

        builder.RegisterComponent(joystick).AsSelf();

        builder.RegisterComponent(fishingUI).AsSelf();
        builder.RegisterComponent(fishingBarUI).AsSelf();
        builder.RegisterComponent(catchedFishUI).AsSelf();
        builder.RegisterComponent(rewardedAdPopupUI).AsSelf();
        builder.RegisterComponent(catchedFishIconUI).AsSelf();
        builder.RegisterComponent(playerBalanceUI).AsSelf();
        builder.RegisterComponent(buffControllerUI).AsSelf();
        builder.RegisterComponent(settingsUI).AsSelf();
        builder.RegisterComponent(timerManager).AsSelf();
        builder.RegisterComponent(musicController).AsSelf();
        builder.RegisterComponent(cookingProgressBarUI).AsSelf();
        builder.RegisterComponent(sellingProgressBarUI).AsSelf();

        builder.RegisterComponent(vfxPlayer).AsSelf();
        builder.RegisterComponent(vfxManager).AsSelf();

        PerformanceUnlocker.Execute();
    }

    async void Start()
    {
        Container.Inject(playerController);
        Container.Inject(playerInventory);
        Container.Inject(playerMovement);
        Container.Inject(playerStateController);
        Container.Inject(playerStepsSound);

        Container.Inject(fishingController);
        Container.Inject(fishingZone);
        Container.Inject(fishingRod);
        Container.Inject(ocean);

        Container.Inject(cookingSpotPickupZone);
        Container.Inject(cookingSpotOutputZone);
        Container.Inject(cookingSpotCookingZone);
        Container.Inject(cookingSpot);

        Container.Inject(sellingSpotPickupZone);
        Container.Inject(sellingSpotOutputZone);
        Container.Inject(sellingSpotSellZone);
        Container.Inject(sellingSpot);
        Container.Inject(customersLine);

        Container.Inject(catchedFishUI);
        Container.Inject(catchedFishIconUI);
        Container.Inject(playerBalanceUI);
        Container.Inject(sellingProgressBarUI);
        Container.Inject(cookingProgressBarUI);
        Container.Inject(rewardedAdPopupUI);
        Container.Inject(settingsUI);

        Container.Inject(playerMoneyBalance);

        Container.Inject(soundPlayer);

        foreach (var upgrade in FindObjectsOfType<Upgrade>(true))
        {
            Container.Inject(upgrade);
        }

        Container.Inject(vfxManager);
        Container.Inject(vfxPlayer);

        Container.Inject(upgradesController);

        gameInitializer = Container.Resolve<GameInitializer>();

        await gameInitializer.InitializeAsync();
        // upgradesController.LoadAllUpgrades();
    }
}

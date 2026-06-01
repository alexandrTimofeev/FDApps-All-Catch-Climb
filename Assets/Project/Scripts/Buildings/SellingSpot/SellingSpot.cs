using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using DG.Tweening;
using System;

public class SellingSpot : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Cooldown between each fish taken from player inventory")]
    [SerializeField] public float cooldownToPickupFish;
    [Header("Components")]
    [SerializeField] private Transform fishStackTR;
    [SerializeField] private Transform outputMoneyStackTR;
    [Header("Coin stack stats")]
    [SerializeField] private int coinRows = 5;
    [SerializeField] private float coinsSpacing = 0.2f;
    [SerializeField] private float coinHeight = 0.1f;
    [Header("Coin pick up stats")]
    [Space(5)]
    [Tooltip("Delay between each coin collected")]
    [SerializeField] public float minCoinCollectDelay = 0.1f;
    [Tooltip("Amount of coins that will trigger speedup of collecting coins")]
    [SerializeField] public int minCoinAmountForSpeedup = 25;
    [Tooltip("Speed of decreasing delay between each coin picked up")]
    [SerializeField] public float speedupSpeed;
    [Tooltip("Animation length of coin getting picked up")]
    [SerializeField] public float coinCollectMoveLength = 0.2f;
    private float sellingDelay = 1f;
    public GameObject cashierPf;
    private CoinSpawner coinSpawner;
    private PlayerMoneyBalance playerMoneyBalance;
    private PlayerController playerController;
    private CustomersLine customersLine;
    private SoundPlayer soundPlayer;
    private SaveService saveService;
    private SellingSpotVFX sellingSpotVFX;
    private Stack<FishItem> fishStack = new Stack<FishItem>();
    private Stack<Coin> coinsStack = new Stack<Coin>();
    private float coinsInStackSideOffset;
    private FishSold_event fishSold_Event = new();
    private bool isDoubleProfit = false;
    private float sellingDelayDelta;

    [Inject]
    public void Construct(
        CoinSpawner coinSpawner,
        PlayerMoneyBalance playerMoneyBalance,
        PlayerController playerController,
        CustomersLine customersLine,
        SoundPlayer soundPlayer,
        SaveService saveService,
        SellingSpotVFX sellingSpotVFX)
    {
        this.coinSpawner = coinSpawner;
        this.playerMoneyBalance = playerMoneyBalance;
        this.playerController = playerController;
        this.customersLine = customersLine;
        this.soundPlayer = soundPlayer;
        this.saveService = saveService;
        this.sellingSpotVFX = sellingSpotVFX;
    }

    void Awake()
    {
        coinsInStackSideOffset = (coinRows - 1) * coinsSpacing / 2f;
        sellingDelayDelta = sellingDelay;
    }
    public void OnUpdate()
    {
        if (!ValidateDependencies(nameof(OnUpdate)))
            return;

        if (!HasAnyFish()) return;

        sellingDelayDelta -= Time.deltaTime;

        if (sellingDelayDelta <= 0)
        {
            SellFish();

            sellingDelayDelta = sellingDelay;
        }
    }
    public void InputFish(FishItem fishItem)
    {
        Action action = () => OnInputFish(fishItem);

        HelpUtilities.Player.PickupFishAndPutToStack(fishItem, fishStackTR, fishStack, action);
    }
    private void OnInputFish(FishItem fishItem)
    {
        if (!ValidateDependencies(nameof(OnInputFish)) || fishItem == null)
        {
            if (fishItem == null)
                Debug.LogError($"{nameof(SellingSpot)}.{nameof(OnInputFish)}: {nameof(fishItem)} is null on {name}.");
            return;
        }

        soundPlayer.PlaySFX(SFXType.FishPlaced, null, 0.8f);

        Vector3 position = fishItem.transform.position;
        position.y -= fishItem.fish.width;

        sellingSpotVFX.SetFishSplashPosition(position);
        sellingSpotVFX.PlayFishSplashVFX();
    }
    private void AddCoinsToStack(List<Coin> newCoins)
    {
        int coinsPerLayer = coinRows * coinRows;
        int startIndex = coinsStack.Count;

        for (int i = 0; i < newCoins.Count; i++)
        {
            int globalIndex = startIndex + i;
            int layerNumber = globalIndex / coinsPerLayer;
            int indexInLayer = globalIndex % coinsPerLayer;

            int xIndex = indexInLayer % coinRows;
            int zIndex = indexInLayer / coinRows;

            Vector3 localPos = new Vector3(
                xIndex * coinsSpacing - coinsInStackSideOffset,
                layerNumber * coinHeight,
                zIndex * coinsSpacing - coinsInStackSideOffset
            );

            Coin coin = newCoins[i];
            coin.transform.SetParent(outputMoneyStackTR);
            coin.transform.localPosition = localPos;

            coinsStack.Push(coin);
        }
    }
    public void CollectLastCoin()
    {
        if (!ValidateDependencies(nameof(CollectLastCoin)))
            return;

        if (coinsStack.Count == 0) return;

        Coin coin = coinsStack.Pop();

        PlayCoinPickupAnimation(coin, () =>
        {
            playerMoneyBalance.AddMoney(coin.GetValue());
            soundPlayer.PlayCoinSFX(null, 0.8f);
            coinSpawner.ReturnCoin(coin);
        });
    }
    private void PlayCoinPickupAnimation(Coin coin, Action onComplete)
    {
        Vector3 startPos = coin.transform.position;
        Vector3 playerPos = playerController.GetPlayerPosition();

        float baseDurationPerUnit = 0.1f;

        Vector3 midPoint = new Vector3(
            (startPos.x + playerPos.x) / 2f,
            startPos.y + 4f,
            (startPos.z + playerPos.z) / 2f
        );

        // Time to travel first half of the animation
        float startDistance = Vector3.Distance(startPos, midPoint);
        float startDuration = Mathf.Clamp(startDistance * baseDurationPerUnit, 0.2f, 1.2f) * coinCollectMoveLength;

        // Time to travel first half of the animation
        float endDistance = Vector3.Distance(midPoint, playerPos);
        float endDuration = Mathf.Clamp(endDistance * baseDurationPerUnit, 0.2f, 1.2f) * coinCollectMoveLength;

        Sequence seq = DOTween.Sequence();

        // First half of the animation
        seq.Append(coin.transform.DOMoveX(midPoint.x, startDuration).SetEase(Ease.Linear));
        seq.Join(coin.transform.DOMoveZ(midPoint.z, startDuration).SetEase(Ease.Linear));
        seq.Join(coin.transform.DOMoveY(midPoint.y, startDuration).SetEase(Ease.OutQuad));

        // First hals of the animation
        seq.Append(coin.transform.DOMoveX(playerPos.x, endDuration).SetEase(Ease.Linear));
        seq.Join(coin.transform.DOMoveZ(playerPos.z, endDuration).SetEase(Ease.Linear));
        seq.Join(coin.transform.DOMoveY(playerPos.y, endDuration).SetEase(Ease.InQuad));

        seq.Join(coin.transform.DOScale(Vector3.zero, endDuration).SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
    public void SellFish()
    {
        if (!ValidateDependencies(nameof(SellFish)))
            return;

        if (fishStack.Count == 0) return;

        FishItem fishItem = fishStack.Pop();
        int value = fishItem.fish.sellValue;
        int finalValue = isDoubleProfit ? value * 2 : value;

        fishItem.fish.sellValue = finalValue;

        bool isDoubled = isDoubleProfit;
        // Spawns coin on completing selling process with the customer
        customersLine.SellToCurrentCustomer(fishItem, () =>
        {
            AddCoinsToStack(coinSpawner.SpawnCoinsFromSum(fishItem.fish.sellValue));

            fishSold_Event.fishItem = fishItem;
            fishSold_Event.isDoubleProfit = isDoubled;
            EventBus.Publish(fishSold_Event);

            soundPlayer.PlaySFX(SFXType.FishSold, null, 1);

            saveService.SaveBalance(fishItem.fish.sellValue);
        });
    }
    public void ActivateDoubleProfit()
    {
        isDoubleProfit = true;
    }
    public void DeactivateDoubleProfit()
    {
        isDoubleProfit = false;
    }
    public float GetFillAmount()
    {
        if (sellingDelayDelta == 0) return 0;

        float fillAmount = 1f - (sellingDelayDelta / sellingDelay);
        return Mathf.Clamp01(fillAmount);
    }
    public bool HasAnyCoins() => coinsStack.Count > 0;
    public int GetCoinsAmount() => coinsStack.Count;
    public bool HasCurrentCustomer() => customersLine.HasCurrentCustomer();
    public bool HasAnyFish() => fishStack.Count > 0;

    private bool ValidateDependencies(string caller)
    {
        if (coinSpawner == null) return LogMissingDependency(nameof(coinSpawner), caller);
        if (playerMoneyBalance == null) return LogMissingDependency(nameof(playerMoneyBalance), caller);
        if (playerController == null) return LogMissingDependency(nameof(playerController), caller);
        if (customersLine == null) return LogMissingDependency(nameof(customersLine), caller);
        if (soundPlayer == null) return LogMissingDependency(nameof(soundPlayer), caller);
        if (saveService == null) return LogMissingDependency(nameof(saveService), caller);
        if (sellingSpotVFX == null) return LogMissingDependency(nameof(sellingSpotVFX), caller);
        if (fishStackTR == null) return LogMissingDependency(nameof(fishStackTR), caller);
        if (outputMoneyStackTR == null) return LogMissingDependency(nameof(outputMoneyStackTR), caller);
        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{nameof(SellingSpot)}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }

#if UNITY_EDITOR
    [ContextMenu("Spawn random coins")]
    private void SpawnRandomCoins()
    {
        int rnd = UnityEngine.Random.Range(300, 600);

        Debug.Log($"Random sum: <color=green>{rnd}</color>");

        AddCoinsToStack(coinSpawner.SpawnCoinsFromSum(rnd));
    }
#endif
}

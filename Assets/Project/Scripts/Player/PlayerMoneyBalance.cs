using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using VContainer;

public class PlayerMoneyBalance
{
    private SaveService saveService;
    private static readonly PlayerMoneyAdded_event MoneyAdded = new();
    private static readonly PlayerMoneyDiscarded_event MoneyDiscraded = new();
    private long currentBalance = 0;
    [Inject]
    public void Construct(SaveService saveService)
    {
        this.saveService = saveService;
    }

    public void InitializeAsync()
    {
        if (saveService == null)
        {
            UnityEngine.Debug.LogError($"{nameof(PlayerMoneyBalance)}.{nameof(InitializeAsync)}: {nameof(saveService)} is null.");
            return;
        }

        currentBalance = saveService.GetSavedBalance();

        EventBus.Publish(MoneyAdded);
    }

    public void AddMoney(int x)
    {
        if (x <= 0) return;

        currentBalance += x;

        EventBus.Publish(MoneyAdded);
    }
    public void DiscardMoney(int x)
    {
        if (x <= 0) return;

        currentBalance -= x;

        if (currentBalance < 0) currentBalance = 0;

        EventBus.Publish(MoneyDiscraded);

        SaveBalance(-x);
    }

    public async void SaveBalance(int additionalMoney)
    {
        await SaveBalanceAsync(additionalMoney);
    }
    public async UniTask SaveBalanceAsync(int additionalMoney)
    {
        if (saveService == null)
        {
            UnityEngine.Debug.LogError($"{nameof(PlayerMoneyBalance)}.{nameof(SaveBalanceAsync)}: {nameof(saveService)} is null.");
            return;
        }

        await saveService.SaveBalanceAsync(additionalMoney);
    }
    public bool IsAffordable(int x) => currentBalance >= x;
    public long GetCurrentBalance() => currentBalance;
}

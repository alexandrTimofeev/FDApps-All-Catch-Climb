using TMPro;
using UnityEngine;
using VContainer;
using DG.Tweening;
public class UIPlayerBalance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI balanceTxt;
    [SerializeField] private RectTransform parentRt;
    private PlayerMoneyBalance playerBalance;

    [Inject]
    public void Construct(PlayerMoneyBalance playerBalance)
    {
        this.playerBalance = playerBalance;
    }

    void Start()
    {
        if (playerBalance == null)
        {
            Debug.LogError($"{nameof(UIPlayerBalance)}.{nameof(Start)}: {nameof(playerBalance)} is null on {name}.");
            return;
        }

        if (balanceTxt == null)
        {
            Debug.LogError($"{nameof(UIPlayerBalance)}.{nameof(Start)}: {nameof(balanceTxt)} is null on {name}.");
            return;
        }

        EventBus.Subscribe<PlayerMoneyDiscarded_event>(OnDiscard);
        EventBus.Subscribe<PlayerMoneyAdded_event>(OnAdd);

        balanceTxt.SetText(FormatBalance(playerBalance.GetCurrentBalance()));
    }
    private void OnDiscard(PlayerMoneyDiscarded_event e)
    {
        UpdateBalance(true);
    }
    private void OnAdd(PlayerMoneyAdded_event e)
    {
        UpdateBalance(false);
    }
    private void UpdateBalance(bool isDiscard)
    {
        if (playerBalance == null || balanceTxt == null || parentRt == null)
        {
            Debug.LogError($"{nameof(UIPlayerBalance)}.{nameof(UpdateBalance)}: balance dependency is null on {name}.");
            return;
        }

        // Determin where to "punch" animation depending on money update type
        int punchDir = isDiscard ? -1 : 1;

        DOTween.Kill(parentRt);

        parentRt.localScale = Vector3.one;

        parentRt.DOPunchScale(Vector3.one * 0.1f * punchDir, 0.3f, 5, 0.2f);

        balanceTxt.SetText(FormatBalance(playerBalance.GetCurrentBalance()));
    }
    private string FormatBalance(long value)
    {
        if (value >= 1_000_000_000)
            return (value / 1_000_000_000f).ToString("0.#") + "B";
        if (value >= 1_000_000)
            return (value / 1_000_000f).ToString("0.#") + "M";
        if (value >= 1_000)
            return (value / 1_000f).ToString("0.#") + "k";

        return value.ToString();
    }
#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            playerBalance.AddMoney(100000);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            playerBalance.DiscardMoney(100);
        }
    }
#endif
}

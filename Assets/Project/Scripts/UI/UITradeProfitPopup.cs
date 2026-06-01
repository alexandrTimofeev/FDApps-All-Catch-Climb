using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITradeProfitPopup : MonoBehaviour
{
    [SerializeField] private RectTransform popupRT;
    [SerializeField] private TextMeshProUGUI profitTxt;
    [SerializeField] private ParticleSystem vfx;
    [SerializeField] private Image background;
    [Header("Colors")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color doubledColor;

    void Awake()
    {
        popupRT.localScale = Vector3.zero;

        EventBus.Subscribe<FishSold_event>(Popup);
    }
    public void Popup(FishSold_event e)
    {

        popupRT.localScale = Vector3.zero;

        int profit = e.fishItem.fish.sellValue;
        background.color = e.isDoubleProfit ? doubledColor : defaultColor;

        profitTxt.SetText($"+{profit}");

        vfx.Emit(60);

        // Animation
        Sequence sequence = DOTween.Sequence();

        sequence.Append(popupRT.DOScale(Vector3.one, 0.05f).SetEase(Ease.InCubic));
        sequence.Append(popupRT.DOPunchScale(Vector3.one * 0.3f, 0.2f, 6, 0.6f));
        sequence.AppendInterval(0.75f);
        sequence.Append(popupRT.DOScale(Vector3.zero, 0.3f));
    }
}

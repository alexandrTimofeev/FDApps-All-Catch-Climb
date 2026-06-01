using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VContainer;

public class UICatchedFish : MonoBehaviour
{
    [SerializeField] private RawImage fishIcon;
    [SerializeField] private TextMeshProUGUI labelTxt;
    [SerializeField] private TextMeshProUGUI weightTxt;
    [SerializeField] private TextMeshProUGUI valueTxt;
    private UICatchedFishIcon uICatchedFishIcon;

    RectTransform rt;
    private float panelWidth;
    private float panelOffset;
    Sequence sequence;

    [Inject]
    public void Construct(UICatchedFishIcon uICatchedFishIcon)
    {
        this.uICatchedFishIcon = uICatchedFishIcon;
    }

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError($"{nameof(UICatchedFish)}.{nameof(Awake)}: {nameof(rt)} is null on {name}.");
            enabled = false;
            return;
        }

        panelWidth = rt.rect.width;

        panelOffset = panelWidth / 20;

        rt.anchoredPosition = new Vector2(panelWidth + panelOffset, 0);
    }

    public void OpenUI(Fish fish)
    {
        if (fish == null)
        {
            Debug.LogError($"{nameof(UICatchedFish)}.{nameof(OpenUI)}: {nameof(fish)} is null on {name}.");
            return;
        }

        if (uICatchedFishIcon == null)
        {
            Debug.LogError($"{nameof(UICatchedFish)}.{nameof(OpenUI)}: {nameof(uICatchedFishIcon)} is null on {name}.");
            return;
        }

        labelTxt.SetText(fish.nameString);
        labelTxt.color = RarityColorProvider.GetColor(fish.rarity);
        weightTxt.SetText($"Weight: {fish.weight.ToString("F2")}kg");
        valueTxt.SetText($"Value: {fish.sellValue}");

        uICatchedFishIcon.SetIcon(fish, 5f);

        PlayAnimation();
    }
    private void PlayAnimation()
    {
        if (sequence != null && sequence.IsActive() && sequence.IsPlaying()) sequence.Kill();

        sequence = DOTween.Sequence();

        rt.DOPunchScale(Vector2.one * 0.2f, 0.4f, 0, 0.2f);

        sequence.Append(rt.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutQuad));
        sequence.AppendInterval(4f);
        sequence.Append(rt.DOAnchorPosX(panelWidth + panelOffset, 0.3f).SetEase(Ease.InQuad));
    }
}

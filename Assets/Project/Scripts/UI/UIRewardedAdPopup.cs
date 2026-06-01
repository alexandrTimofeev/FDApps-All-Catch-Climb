using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UIRewardedAdPopup : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image mainPanel;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Image bigIconImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button claimBtn;
    [SerializeField] private TextMeshProUGUI claimBtnText;
    [SerializeField] private Button skipBtn;
    [SerializeField] private Image closeBtnImage;
    private Color mainPanelDefaultColor;
    private Color mainPanelClearColor;
    private Color skipBtnDefaultColor;
    private Color skipBtnClearColor;
    void Awake()
    {
        mainPanelDefaultColor = mainPanel.color;
        mainPanelClearColor = mainPanel.color;
        mainPanelClearColor.a = 0;

        skipBtnDefaultColor = closeBtnImage.color;
        skipBtnClearColor = closeBtnImage.color;
        skipBtnClearColor.a = 0;

        claimBtn.onClick.AddListener(ClaimReward);
        skipBtn.onClick.AddListener(SkipReward);

        ResetRewardButton();

        mainPanel.gameObject.SetActive(false);
    }
    private void ClaimReward()
    {
    }
    private void SkipReward()
    {
    }
    public void ClosePanel()
    {
        // Animation
        DOTween.Kill(mainPanel);
        DOTween.Kill(closeBtnImage);
        DOTween.Kill(panelRect);

        mainPanel.DOColor(mainPanelClearColor, 0.4f);
        panelRect.DOScale(Vector3.zero, 0.4f).OnComplete(() => mainPanel.gameObject.SetActive(false));
        closeBtnImage.DOColor(skipBtnClearColor, 0.1f);
    }
    public void RewardBtnToLoading()
    {
        claimBtn.interactable = false;
        skipBtn.interactable = false;
        claimBtnText.SetText("LOADING...");
    }
    private void ResetRewardButton()
    {
        claimBtn.interactable = true;
        skipBtn.interactable = true;
        claimBtnText.SetText("CLAIM");
    }
    public void OpenPanel()
    {
        ResetRewardButton();

        panelRect.localScale = Vector3.zero;
        mainPanel.color = mainPanelClearColor;
        closeBtnImage.color = skipBtnClearColor;


        mainPanel.gameObject.SetActive(true);

        // Animation
        DOTween.Kill(mainPanel);
        DOTween.Kill(closeBtnImage);
        DOTween.Kill(panelRect);

        mainPanel.DOColor(mainPanelDefaultColor, 0.8f);
        panelRect.DOScale(Vector3.one, 1f);
        DOVirtual.DelayedCall(1.3f, () => { closeBtnImage.DOColor(skipBtnDefaultColor, 0.5f); });
    }
    public void InjectDependency()
    {
    }
}

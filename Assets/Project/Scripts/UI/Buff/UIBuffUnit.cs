using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

public class UIBuffUnit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image backgroundImage;
    private BuffUnit buffUnit;
    private UIBuffController uIBuffController;
    public void Setup(UIBuffController uIBuffController)
    {
        this.uIBuffController = uIBuffController;
    }
    public void ShowUnit( BuffUnit buffUnit)
    {
        this.buffUnit = buffUnit;

        iconImage.fillAmount = 1f;

        iconImage.DOFillAmount(0f, buffUnit.smartTimer.TimeLeft).SetEase(Ease.Linear);
        buffUnit.OnTimeExtended += OnBuffExtended;
        buffUnit.OnComplete += DisableUnit;

        gameObject.SetActive(true);
    }
    public void DisableUnit()
    {
        buffUnit.OnTimeExtended -= OnBuffExtended;
        buffUnit.OnComplete -= DisableUnit;

        HideDescription();

        uIBuffController.ReturnUiBUffUnit(this);

        gameObject.SetActive(false);
    }
    public void OnBuffExtended(float extendDuration)
    {
        iconImage.fillAmount = 1f;

        DOTween.Kill(iconImage);

        iconImage.DOFillAmount(0f, buffUnit.smartTimer.TimeLeft).SetEase(Ease.Linear);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowDescription();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        HideDescription();
    }
    private void ShowDescription()
    {
        uIBuffController.ShowDescription(buffUnit.smartTimer.TimeLeft);
    }
    private void HideDescription()
    {
        uIBuffController.HideDescription();
    }
}

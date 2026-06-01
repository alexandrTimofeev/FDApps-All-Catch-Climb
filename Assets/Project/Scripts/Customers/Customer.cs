using UnityEngine;
using DG.Tweening;
using System;

public class Customer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform itemPointTR;
    [SerializeField] private GameObject VIP_icon;
    private FishObjectPool fishObjectPool;
    private CustomersLine customersLine;
    private FishItem fishItem;
    private Quaternion VIP_iconRotation;
    private bool isVIP = false;
    public void SetValues(CustomersLine customersLine, FishObjectPool fishObjectPool)
    {
        this.customersLine = customersLine;
        this.fishObjectPool = fishObjectPool;

        VIP_icon.SetActive(false);
        VIP_iconRotation = VIP_icon.transform.localRotation;
    }
    public void LeaveQueue()
    {
        SetMoving(true);
        SetHandsUp(true);

        Sequence sequence = DOTween.Sequence();

        // Moves right out of the line
        sequence.Append(transform.DOMove(transform.position + Vector3.right * 2, 0.5f).SetEase(Ease.Linear));
        sequence.Join(transform.DORotate(Vector3.up * 90, 0.3f));
        sequence.Join(VIP_icon.transform.DOLocalRotate((Quaternion.AngleAxis(-90, Vector3.up) * VIP_iconRotation).eulerAngles, 0.3f));

        // Moves back to the line
        sequence.Append(transform.DOMove(transform.position + Vector3.back * 10, 3f).SetEase(Ease.Linear));
        sequence.Join(transform.DORotate(Vector3.up * 180, 0.5f));
        sequence.Join(VIP_icon.transform.DOLocalRotate((Quaternion.AngleAxis(-180, Vector3.up) * VIP_iconRotation).eulerAngles, 0.5f));

        sequence.OnComplete(() =>
        {
            customersLine.AddNewCustomer(this);
            VIP_icon.transform.localRotation = VIP_iconRotation;
            Reset();
        });
    }

    private void Reset()
    {
        transform.rotation = Quaternion.identity;

        SetMoving(false);
        SetHandsUp(false);

        fishObjectPool.ReturnFish(fishItem);
        fishItem = null;

        if (isVIP)
        {
            isVIP = false;
            VIP_icon.SetActive(false);
        }

        customersLine.ReturnCustomer(this);
    }
    public void BuyProduct(FishItem fishItem, Action action)
    {
        this.fishItem = fishItem;

        DOTween.Kill(fishItem.transform);

        // Increase payout by 2X if customer is VIP
        if (isVIP) fishItem.fish.sellValue *= 2;

        action?.Invoke();

        fishItem.transform.SetParent(itemPointTR, true);

        // Animation for the fish
        Sequence sequence = DOTween.Sequence();

        sequence.Append(fishItem.transform.DOLocalMove(Vector3.zero, 0.2f));
        sequence.Join(fishItem.transform.DOLocalRotate(Vector3.up * 90, 0.2f));

        sequence.OnComplete(LeaveQueue);
    }
    public void SetMoving(bool b)
    {
        animator.SetBool("IsMoving", b);
    }
    public void SetHandsUp(bool b)
    {
        animator.SetBool("IsHandsUp", b);
    }
    public void MakeVIP()
    {
        isVIP = true;
        VIP_icon.SetActive(true);
    }
    public bool IsVIP() => isVIP;
}

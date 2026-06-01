using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VContainer;

[RequireComponent(typeof(LineRenderer))]
public class FishingRod : MonoBehaviour
{
    [SerializeField] private Transform stringSource;
    [SerializeField] private Transform bobberStringTR;
    [SerializeField] private Transform bobberTR;
    [SerializeField] private Transform oceanFloatingPoint;
    [Header("Rod string parameters")]
    [SerializeField] private int segmentCount = 10;
    [SerializeField] private float maxCurveHeight = 0.5f;
    private SoundPlayer soundPlayer;
    private LineRenderer lineRenderer;
    private float currentCurveHeight = 0f;

    [Inject]
    public void Construct(SoundPlayer soundPlayer)
    {
        this.soundPlayer = soundPlayer;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError($"{nameof(FishingRod)}.{nameof(Awake)}: {nameof(lineRenderer)} is null on {name}.");
            enabled = false;
            return;
        }

        lineRenderer.positionCount = segmentCount;
        lineRenderer.useWorldSpace = true;

        EventBus.Subscribe<CatchPressed_event>(OnCatch);
    }

    private void Update()
    {
        if (lineRenderer == null)
            return;

        UpdateLine();
    }

    private void UpdateLine()
    {
        if (stringSource == null || bobberTR == null || oceanFloatingPoint == null)
        {
            Debug.LogError($"{nameof(FishingRod)}.{nameof(UpdateLine)}: rod transform reference is null on {name}.");
            return;
        }

        float currentLength = Vector3.Distance(stringSource.position, bobberTR.position);
        float curveHeight = currentCurveHeight * (currentLength / Vector3.Distance(stringSource.position, oceanFloatingPoint.position));
        curveHeight = Mathf.Clamp(curveHeight, 0f, currentCurveHeight);

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / (segmentCount - 1);
            Vector3 pos = Vector3.Lerp(stringSource.position, bobberTR.position, t);
            pos.y -= Mathf.Sin(Mathf.PI * t) * curveHeight;
            lineRenderer.SetPosition(i, pos);
        }
    }

    public void Show()
    {
        if (!ValidateDependencies(nameof(Show)))
            return;

        gameObject.SetActive(true);
        bobberTR.gameObject.SetActive(true);
        lineRenderer.enabled = true;

        bobberTR.position = oceanFloatingPoint.position;
        currentCurveHeight = maxCurveHeight;
    }

    public void Hide()
    {
        if (!ValidateDependencies(nameof(Hide)))
            return;

        lineRenderer.enabled = false;
        bobberTR.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnCatch(CatchPressed_event e)
    {
        ReturnBobber();
    }

    private void ReturnBobber()
    {
        if (!ValidateDependencies(nameof(ReturnBobber)))
            return;

        DOTween.To(() => currentCurveHeight, x => currentCurveHeight = x, 0f, 0.3f);

        bobberTR.DOMove(stringSource.position, 0.3f)
                 .OnComplete(() => ReleaseBobber(0.3f));

        soundPlayer.PlaySFX(SFXType.ReelPull);
    }

    private void ReleaseBobber(float delay = 0f)
    {
        DOVirtual.DelayedCall(delay, () =>
        {
            currentCurveHeight = 0f;

            // Start loosening a string after delay
            DOVirtual.DelayedCall(0.2f, () =>
                DOTween.To(() => currentCurveHeight, x => currentCurveHeight = x, maxCurveHeight, 0.5f)
                   .SetEase(Ease.OutQuad));

            bobberTR.DOJump(
                oceanFloatingPoint.position,
                1.5f,
                1,
                0.6f
            ).OnComplete(() =>
            {
                if (gameObject.activeSelf == false) return;
                soundPlayer.PlaySFX(SFXType.BobberSplash, bobberTR.position, 0.5f);
            });
        });
    }

    private bool ValidateDependencies(string caller)
    {
        if (lineRenderer == null) return LogMissingDependency(nameof(lineRenderer), caller);
        if (bobberTR == null) return LogMissingDependency(nameof(bobberTR), caller);
        if (oceanFloatingPoint == null) return LogMissingDependency(nameof(oceanFloatingPoint), caller);
        if (stringSource == null) return LogMissingDependency(nameof(stringSource), caller);
        if (soundPlayer == null) return LogMissingDependency(nameof(soundPlayer), caller);
        return true;
    }

    private bool LogMissingDependency(string dependencyName, string caller)
    {
        Debug.LogError($"{nameof(FishingRod)}.{caller}: {dependencyName} is null on {name}.");
        return false;
    }
}

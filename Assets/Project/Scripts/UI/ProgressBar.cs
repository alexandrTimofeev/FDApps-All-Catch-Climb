using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using VContainer;

public abstract class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Image background;
    private Material material;
    Color clearColor = new Color(1, 1, 1, 0);
    protected float fillAmount = 0;
    private void Awake()
    {
        fillImage.fillAmount = 0;
        gameObject.SetActive(false);

        material = Instantiate(fillImage.material);

        fillImage.material = material;
        background.material = material;

        material.color = clearColor;

        SubscribeToEvents();
    }
    protected abstract void SubscribeToEvents();
    public abstract void UnsubscribeFromEvents();
    public void ShowBar(object e)
    {
        DOTween.Kill(gameObject);
        DOTween.Kill(material);

        gameObject.SetActive(true);

        material.DOColor(Color.white, 0.3f);
    }
    public void HideBar(object e)
    {
        material.DOColor(clearColor, 0.3f).OnComplete(() => gameObject.SetActive(false));
    }
    void Update()
    {
        fillAmount = GetValueForBar();
        UpdateBar(fillAmount);
    }
    public abstract float GetValueForBar();
    public void UpdateBar(float fillAmount)
    {
        fillImage.fillAmount = fillAmount;
    }
    private void OnDestroy()
    {
        if (Application.isPlaying && material != null)
        {
            Destroy(material);
        }
    }
    public void DestroyProgressBar()
    {
        UnsubscribeFromEvents();
        Destroy(gameObject);
    }
}

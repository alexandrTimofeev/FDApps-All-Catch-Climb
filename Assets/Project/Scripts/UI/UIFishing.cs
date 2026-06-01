using UnityEngine;
using UnityEngine.UI;

public class UIFishing : MonoBehaviour
{
    [SerializeField] private Button exitBtn;
    void Awake()
    {
        ExitPressed_event exitEvent = new();
        exitBtn.onClick.AddListener(() =>
        {
            EventBus.Publish(exitEvent);
        });

        HideUI();
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
    public void ShowUI()
    {
        gameObject.SetActive(true);
    }
}

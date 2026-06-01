using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBuffController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private int startingPool = 3;
    [SerializeField] private Transform uiBuffPrefab;
    [SerializeField] private Transform uiBuffParent;
    private Queue<UIBuffUnit> uiBuffUnitsPool = new Queue<UIBuffUnit>();
    void Awake()
    {
        Initialize();

        HideDescription();
    }
    private void Initialize()
    {
        for (int i = 0; i < startingPool; i++)
        {
            var uiBuff = Instantiate(uiBuffPrefab, uiBuffParent).GetComponent<UIBuffUnit>();

            uiBuff.Setup(this);
            uiBuff.gameObject.SetActive(false);

            uiBuffUnitsPool.Enqueue(uiBuff);
        }
    }
    public void AddBuff( BuffUnit buffUnit)
    {
        if (uiBuffUnitsPool.Count > 0)
        {
            var uiBuff = uiBuffUnitsPool.Dequeue();

            uiBuff.ShowUnit( buffUnit);
        }
        else
        {
            var uiBuff = Instantiate(uiBuffPrefab, uiBuffParent).GetComponent<UIBuffUnit>();

            uiBuff.Setup(this);
            uiBuff.gameObject.SetActive(false);

            uiBuff.ShowUnit( buffUnit);
        }
    }
    public void ShowDescription( float timeLeft)
    {
        int minutes = (int)(timeLeft / 60f);
        int seconds = (int)(timeLeft % 60f);

        string formattedTime = $"{minutes:00}:{seconds:00}";

        descriptionTxt.gameObject.SetActive(true);
    }

    public void HideDescription()
    {
        descriptionTxt.SetText("");

        descriptionTxt.gameObject.SetActive(false);
    }
    public void ReturnUiBUffUnit(UIBuffUnit uiBuffUnit)
    {
        uiBuffUnitsPool.Enqueue(uiBuffUnit);
    }
}

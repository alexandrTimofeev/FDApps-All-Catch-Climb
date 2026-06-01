using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Renderer coinRenderer;
    [SerializeField] private int value;

    public void SetParams(CoinSO coinSO)
    {
        value = coinSO.value;
        coinRenderer.material.mainTexture = coinSO.texture;

        gameObject.SetActive(true);
    }
    public void EnableCoin() => gameObject.SetActive(true);
    public void DisableCoin() => gameObject.SetActive(false);
    public int GetValue() => value;
}

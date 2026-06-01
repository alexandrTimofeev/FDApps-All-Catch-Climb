using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private List<CoinSO> coinsList = new List<CoinSO>();
    [SerializeField] private Transform prefab;
    Queue<Coin> coinPool = new Queue<Coin>();
    int startingPool = 15;
    void Awake()
    {
        InitializePool();
    }
    private void InitializePool()
    {
        for (int i = 0; i < startingPool; i++)
        {
            var coinObject = Instantiate(prefab);
            var coinScript = coinObject.GetComponent<Coin>();

            coinPool.Enqueue(coinScript);

            coinScript.DisableCoin();
        }
    }
    public List<Coin> SpawnCoinsFromSum(int sum)
    {
        if (sum == 0) sum = 1;

        List<Coin> coins = new List<Coin>();

        int totalValue = sum;
        for (int i = coinsList.Count - 1; i >= 0; i--)
        {
            CoinSO coinSO = coinsList[i];
            int coinsToSpawn = (totalValue - totalValue % coinSO.value) / coinSO.value;
            totalValue -= coinsToSpawn * coinSO.value;

            for (int x = 0; x < coinsToSpawn; x++)
            {
                var coin = GetCoin();
                coin.SetParams(coinSO);

                coins.Add(coin);
            }
        }

        return coins;
    }

    public Coin GetCoin()
    {
        Coin coin;
        if (coinPool.Count > 0)
        {
            coin = coinPool.Dequeue();
        }
        else
        {
            coin = Instantiate(prefab).GetComponent<Coin>();
        }

        coin.EnableCoin();
        return coin;
    }
    public void ReturnCoin(Coin coin)
    {
        coin.DisableCoin();
        coin.transform.SetParent(null);
        coin.transform.localScale = prefab.transform.localScale;

        coinPool.Enqueue(coin);
    }
}

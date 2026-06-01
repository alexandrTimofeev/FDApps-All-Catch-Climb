using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FishObjectPool : MonoBehaviour
{
    private List<FishSO> startingPool;
    [Tooltip("Starting amount of each element")]
    [SerializeField] private int startingAmount;

    // Fish ObjectPool that is accessed by fish model
    private Dictionary<string, Queue<FishItem>> fishPool = new Dictionary<string, Queue<FishItem>>();

    private GameObject tempGO;
    private Renderer tempRenderer;
    private async void Start()
    {
        await LoadFishSOListAsync();
        await PreloadMaterialsAsync();
        InitializePool();
    }
    public async UniTask LoadFishSOListAsync()
    {
        AsyncOperationHandle<IList<FishSO>> handle = Addressables.LoadAssetsAsync<FishSO>("FishSO", null);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            startingPool = new List<FishSO>(handle.Result);
            // Debug.Log($"Loaded {startingPool.Count} fish assets from Addressables.");
        }
        else
        {
            Debug.LogError("Failed to load FishSO list from Addressables.");
            startingPool = new List<FishSO>();

            return;
        }
    }
    private void InitializePool()
    {
        foreach (FishSO fishSO in startingPool)
        {
            for (int i = 0; i < startingAmount; i++)
            {
                var fishObject = Instantiate(fishSO.fishModel);
                var fishItemScript = fishObject.GetComponent<FishItem>();
                fishItemScript.fish = fishSO.ToFish(0, 0);

                // Adds new queue of fish type if it doesn't exist yet
                if (fishPool.ContainsKey(fishSO.nameString) == false)
                {
                    fishPool.Add(fishSO.nameString, new Queue<FishItem>());
                }

                fishPool[fishSO.nameString].Enqueue(fishItemScript);

                fishObject.gameObject.SetActive(false);
            }
        }
    }
    public FishItem GetFish(Fish fishItem)
    {
        if (fishPool.ContainsKey(fishItem.nameString) == false) return null;

        Queue<FishItem> pool = fishPool[fishItem.nameString];

        if (pool.Count > 0)
        {
            FishItem fish = pool.Dequeue();
            fish.gameObject.SetActive(true);
            return fish;
        }
        else
        {
            FishItem fish = Instantiate(FindFishSO(fishItem).fishModel).GetComponent<FishItem>();
            fish.fish = fishItem;
            return fish;
        }
    }

    public void ReturnFish(FishItem fish)
    {
        fish.transform.localScale = Vector3.one * fish.defaultScaleFloat;

        fish.gameObject.SetActive(false);
        fish.transform.SetParent(null);

        string fishName = fish.fish.nameString;

        if (fishPool.ContainsKey(fishName) == false)
        {
            Debug.LogError("No such fish model in fish object pool");
            return;
        }

        fish.transform.position = Vector3.zero;
        fish.transform.rotation = Quaternion.identity;
        fishPool[fishName].Enqueue(fish);
    }
    private FishSO FindFishSO(Fish fishItem)
    {
        FishSO fishSO = null;
        foreach (FishSO fish in startingPool)
        {
            if (fish.fishModel == fishItem.fishModel)
            {
                fishSO = fish;
                break;
            }
        }

        if (fishSO == null)
            Debug.LogError("No such fish in starting pool");

        return fishSO;
    }
    public async UniTask PreloadMaterialsAsync()
    {
        if (startingPool == null || startingPool.Count == 0)
            return;

        tempGO = new GameObject("TempMaterialLoader");
        tempRenderer = tempGO.AddComponent<MeshRenderer>();

        foreach (var fishSO in startingPool)
        {
            if (fishSO.fishMaterial == null)
                continue;

            tempRenderer.material = fishSO.fishMaterial;

            await UniTask.Yield();
        }

        Destroy(tempGO);
    }
}
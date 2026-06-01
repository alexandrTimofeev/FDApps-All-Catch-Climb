using UnityEngine;

public class RarityColorProvider : MonoBehaviour
{
    [SerializeField] private FishRarityColorsSO rarityColorsSO;
    public static RarityColorProvider Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static Color GetColor(FishRarity rarity)
    {
        return Instance.rarityColorsSO.GetColor(rarity);
    }
}

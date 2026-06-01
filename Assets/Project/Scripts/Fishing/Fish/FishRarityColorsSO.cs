using UnityEngine;

[CreateAssetMenu(fileName = "FishRarityColors", menuName = "Scriptable Objects/FishRarityColorsSO")]
public class FishRarityColorsSO : ScriptableObject
{
    [System.Serializable]
    public struct RarityColor
    {
        public FishRarity rarity;
        public Color color;
    }

    public RarityColor[] rarityColors;
    public Color GetColor(FishRarity rarity)
    {
        foreach (var entry in rarityColors)
        {
            if (entry.rarity == rarity)
                return entry.color;
        }

        Debug.LogWarning($"Color for rarity {rarity} not found!");
        return Color.white;
    }
}

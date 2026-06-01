using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerFishingCharacteristics
{
    private Dictionary<FishRarity, int> chances = new Dictionary<FishRarity, int>();
    private int currentLevel = 0;
    public PlayerFishingCharacteristics()
    {
        SetChancesLevel(1);
    }
    public Dictionary<FishRarity, int> GetChances() => chances;
    public FishRarity GetRandomRarity()
    {
        int totalWeight = 0;

        foreach (var pair in chances)
        {
            totalWeight += pair.Value;
        }

        if (totalWeight == 0)
        {
            Debug.LogWarning("Total weight is 0. Cannot choose random rarity.");
            return FishRarity.Common;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentSum = 0;

        foreach (var pair in chances)
        {
            currentSum += pair.Value;
            if (randomValue < currentSum)
            {
                return pair.Key;
            }
        }

        Debug.LogWarning("Error in calculating chances");
        return FishRarity.Common;
    }
    public void SetChancesLevel(int level)
    {
        if (level < currentLevel) return;

        switch (level)
        {
            case 1:
                chances = new Dictionary<FishRarity, int>
                {
                    {FishRarity.Common, 65 },
                    {FishRarity.Uncommon, 25 },
                    {FishRarity.Rare, 9 },
                    {FishRarity.Epic, 1 },
                    {FishRarity.Mythic, 0 },
                    {FishRarity.Legendary, 0 }
                };
                break;

            case 2:
                chances = new Dictionary<FishRarity, int>
                {
                    {FishRarity.Common, 35 },
                    {FishRarity.Uncommon, 30 },
                    {FishRarity.Rare, 20 },
                    {FishRarity.Epic, 10 },
                    {FishRarity.Mythic, 5 },
                    {FishRarity.Legendary, 0 }
                };
                break;

            case 3:
                chances = new Dictionary<FishRarity, int>
                {
                    {FishRarity.Common, 15 },
                    {FishRarity.Uncommon, 20 },
                    {FishRarity.Rare, 30 },
                    {FishRarity.Epic, 23 },
                    {FishRarity.Mythic, 10 },
                    {FishRarity.Legendary, 2 }
                };
                break;

            case 4:
                chances = new Dictionary<FishRarity, int>
                {
                    {FishRarity.Common, 10 },
                    {FishRarity.Uncommon, 15 },
                    {FishRarity.Rare, 20 },
                    {FishRarity.Epic, 25 },
                    {FishRarity.Mythic, 20 },
                    {FishRarity.Legendary, 10 }
                };
                break;
            case 5:
                chances = new Dictionary<FishRarity, int>
                {
                    {FishRarity.Common, 5 },
                    {FishRarity.Uncommon, 10 },
                    {FishRarity.Rare, 15 },
                    {FishRarity.Epic, 30 },
                    {FishRarity.Mythic, 25 },
                    {FishRarity.Legendary, 15 }
                };
                break;
        }

        currentLevel = level;
    }
}
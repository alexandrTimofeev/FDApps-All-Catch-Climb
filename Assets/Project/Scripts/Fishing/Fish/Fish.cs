using UnityEngine;

public class Fish
{
    public string nameString;
    public FishSpeed speed;
    public FishStrength strength;
    public FishRarity rarity;
    public float weight;
    public float sizeMult;
    public int sellValue;
    public bool isCooked = false;
    public GameObject fishModel;
    public Material fishMaterial;
    public float width;
    public Fish Clone()
    {
        return new Fish
        {
            nameString = this.nameString,
            speed = this.speed,
            strength = this.strength,
            rarity = this.rarity,
            weight = this.weight,
            sizeMult = this.sizeMult,
            sellValue = this.sellValue,
            isCooked = this.isCooked,
            fishModel = this.fishModel,       
            fishMaterial = this.fishMaterial, 
            width = this.width
        };
    }
}

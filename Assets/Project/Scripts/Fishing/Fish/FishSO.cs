using UnityEngine;


[CreateAssetMenu(fileName = "FishSO", menuName = "Scriptable Objects/FishSO")]
public class FishSO : ScriptableObject
{
    public string nameString;
    [Tooltip("How fast the cursor moves during fishing")]
    public FishSpeed speed;
    [Tooltip("How small is catch area during fishing")]
    public FishStrength strength;
    public FishRarity rarity;
    public int sellValue;
    [Space(10)]
    public float averageSize = 2f;
    public float deltaSize = 1.8f;
    [Tooltip("How wide is the model of fish. It is needed for properly managing player inventory")]
    public float width = 0.2f;
    [Space(10)]
    public GameObject fishModel;
    public Material fishMaterial;
}

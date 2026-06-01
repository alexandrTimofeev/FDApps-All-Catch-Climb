using UnityEngine;

[CreateAssetMenu(fileName = "New Coin", menuName = "Scriptable Objects/CoinSO")]
public class CoinSO : ScriptableObject
{
    public int value;
    public Texture texture;
}

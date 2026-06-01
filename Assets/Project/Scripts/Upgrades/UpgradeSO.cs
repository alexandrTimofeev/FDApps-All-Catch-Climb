using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO", order = 0)]
public class UpgradeSO : ScriptableObject
{
    public string uniqueName;
    public Sprite icon;
    public string label;
    public string descriptionText;
    public int cost;
    [Tooltip("Upgrades that will unlock after this upgraded is purchased")]
    public List<UpgradeSO> nextUpgradesList;
    [HideInInspector] public Vector2 editorPosition;
}

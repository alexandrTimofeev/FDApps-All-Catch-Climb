using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeConfigComponents : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] public Image buyingProgressImage;
    [SerializeField] public Image iconImage;
    [Space(5)]
    [SerializeField] public TextMeshPro costText;
    [SerializeField] public TextMeshPro descriptionText;
    [Space(5)]
    [SerializeField] public EventZone buyingZone;
    [SerializeField] public EventZone interactZone;
}

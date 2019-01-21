using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shields.Modules;

public class ChargeBar : MonoBehaviour
{
    public ShieldController shield;
    [SerializeField] private Image image;

    void Awake()
    {
        shield.OnSpecialAbilityChargeChanged += Refresh;
    }

    private void Refresh(float value, float maxValue)
    {
        image.fillAmount = value / maxValue;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public Damagable damagable;
    [SerializeField] private Image image;

    void Awake()
    {
        damagable.OnHealthChanged += Refresh;
    }

    private void Refresh(float health, float maxHealth)
    {
        image.fillAmount = health / maxHealth;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Bar : MonoBehaviour
    {
        [SerializeField] private Image image;


        public void Refresh(float value, float maxValue)
        {
            image.fillAmount = value / maxValue;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; private set; }

        [HelpBox("Контроллер UI. Использовать только 1 экземляр в сцене!")]
        [SerializeField] private Bar hpBar;
        public static Bar HPBar { get { return Instance.hpBar; } }

        [SerializeField] private Bar specialChargeBar;
        public static Bar SpecialChargeBar { get { return Instance.specialChargeBar; } }

        [SerializeField] private Bar shieldChargeBar;
        public static Bar ShieldChargeBar { get { return Instance.shieldChargeBar; } }


        private void Awake()
        {
            Instance = this;
        }
    }
}
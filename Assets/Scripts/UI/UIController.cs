using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance { get; private set; }

        [SerializeField] private Bar hpBar;
        public static Bar HPBar { get { return Instance.hpBar; } }

        [SerializeField] private Bar chargeBar;
        public static Bar ChargeBar { get { return Instance.chargeBar; } }


        private void Awake()
        {
            Instance = this;
        }
    }
}
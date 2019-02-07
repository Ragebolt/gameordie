using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

namespace Shields.Modules
{
    public abstract class ModuleSpecialAbilityBase : ShieldModule
    {
        public event System.Action<float, float> OnChargeChanged = (f1, f2) => { };

        [HelpBox("Модуль щита, вызывающий специальную способность при нажатии на ПКМ и расходующий специальный заряд")]
        [SerializeField] private float charge = 0f;
        [SerializeField] private float maxCharge = 1f;
        [SerializeField] private float chargeCost = 1f;


        private float Charge
        {
            get { return charge; }
            set
            {
                charge = Mathf.Clamp(value, 0f, maxCharge);
                OnChargeChanged(charge, maxCharge);
            }
        }
        


        protected virtual void Start()
        {
            InputController.OnSuperAbilityButton += StartAbility;
            OnChargeChanged += UIController.SpecialChargeBar.Refresh;
            Charge = charge;

            GetAnotherModules();
        }


        /// <summary>
        /// Добавляет значение к заряду способности
        /// </summary>
        /// <param name="value"></param>
        public void AddCharge(float value)
        {
            Charge += value;
        }


        /// <summary>
        /// Старт способности
        /// </summary>
        public void StartAbility()
        {
            if (Charge < chargeCost || !CanStartAbility()) return;
            Charge -= chargeCost;

            OnStartAbility();
        }

        /// <summary>
        /// Переопределять для условия старта способности
        /// </summary>
        protected virtual bool CanStartAbility() { return true; }

        /// <summary>
        /// Действия после старта для переопределения
        /// </summary>
        protected abstract void OnStartAbility();


        /// <summary>
        /// Принудительная остановка способности
        /// </summary>
        public virtual void StopAbility() { }


        protected override void OnModuleGetted(ShieldModule module)
        {
            if (module is ISpecialChargeProvider) (module as ISpecialChargeProvider).OnChargeGenerated += AddCharge;
        }
    }
}
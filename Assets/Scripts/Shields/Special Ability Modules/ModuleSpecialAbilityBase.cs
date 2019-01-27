using System.Collections;
using System.Collections.Generic;
using UnityEngine;
<<<<<<< HEAD
=======
using UI;
>>>>>>> master

namespace Shields.Modules
{
    public abstract class ModuleSpecialAbilityBase : ShieldModule
    {
<<<<<<< HEAD
        private float charge = 0f;
=======
        public event System.Action<float, float> OnChargeChanged = (f1, f2) => { };

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
        


        private void Start()
        {
            InputController.OnSuperAbilityButton += StartAbility;
            OnChargeChanged += UIController.ChargeBar.Refresh;
            Charge = charge;

            GetAnotherModules();
        }
>>>>>>> master


        /// <summary>
        /// Добавляет значение к заряду способности
        /// </summary>
        /// <param name="value"></param>
        public void AddCharge(float value)
        {
<<<<<<< HEAD
            charge += value;
            ShieldController.SpecialAbilityChargeChanged(charge);
=======
            Charge += value;
>>>>>>> master
        }


        /// <summary>
        /// Старт способности
        /// </summary>
        public void StartAbility()
        {
<<<<<<< HEAD
            if (charge < 1f) return;
=======
            if (Charge < chargeCost || !CanStartAbility()) return;
            Charge -= chargeCost;
>>>>>>> master

            OnStartAbility();
        }

        /// <summary>
<<<<<<< HEAD
=======
        /// Переопределять для условия старта способности
        /// </summary>
        protected virtual bool CanStartAbility() { return true; }

        /// <summary>
>>>>>>> master
        /// Действия после старта для переопределения
        /// </summary>
        protected abstract void OnStartAbility();


        /// <summary>
        /// Принудительная остановка способности
        /// </summary>
        public virtual void StopAbility() { }
<<<<<<< HEAD
=======


        protected override void OnModuleGetted(ShieldModule module)
        {
            if (module is ISpecialChargeProvider) (module as ISpecialChargeProvider).OnChargeGenerated += AddCharge;
        }
>>>>>>> master
    }
}
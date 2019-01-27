using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    public abstract class ModuleSpecialAbilityBase : ShieldModule
    {
        private float charge = 0f;


        /// <summary>
        /// Добавляет значение к заряду способности
        /// </summary>
        /// <param name="value"></param>
        public void AddCharge(float value)
        {
            charge += value;
            ShieldController.SpecialAbilityChargeChanged(charge);
        }


        /// <summary>
        /// Старт способности
        /// </summary>
        public void StartAbility()
        {
            if (charge < 1f) return;

            OnStartAbility();
        }

        /// <summary>
        /// Действия после старта для переопределения
        /// </summary>
        protected abstract void OnStartAbility();


        /// <summary>
        /// Принудительная остановка способности
        /// </summary>
        public virtual void StopAbility() { }
    }
}
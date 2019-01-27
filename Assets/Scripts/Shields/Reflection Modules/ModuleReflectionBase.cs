using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    public abstract class ModuleReflectionBase : ShieldModule, ISpecialChargeProvider
    {
        public event System.Action<float> OnChargeGenerated = (f) => { };

        [SerializeField] private float chargePerBullet = 0.01f;


        /// <summary>
        /// Вызывать при касании пули щита
        /// </summary>
        /// <param name="bullet"> Пуля, которая каснулась щита </param>
        /// <param name="collision"> Информация о касании </param>
        public abstract void OnBullet(Bullet bullet, Collision2D collision);

        /// <summary>
        /// Вызывать при отбивании любой пули
        /// </summary>
        protected void OnAnyBullet()
        {
            OnChargeGenerated(chargePerBullet);
        }
    }
}
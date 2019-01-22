using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    public class ModuleReflection : ShieldModule
    {
        [SerializeField] private float chargePerBullet = 0.01f;


        /// <summary>
        /// Вызывать при касании пули щита
        /// </summary>
        /// <param name="bullet"> Пуля, которая каснулась щита </param>
        /// <param name="collision"> Информация о касании </param>
        public virtual void OnBullet(Bullet bullet, Collision2D collision)
        {
            if (bullet.Creator == PlayerController.Instance.gameObject) return;

            bullet.OnContact();

            OnAnyBullet();
        }

        protected void OnAnyBullet()
        {
            ShieldController.SpecialAbilityModule?.AddCharge(chargePerBullet);
        }
    }
}
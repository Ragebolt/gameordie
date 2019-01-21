using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Legacy
{
    [CreateAssetMenu(fileName = "Double Shield", menuName = "Shields/Double Shield")]
    public class DoubleShield : ShieldBase
    {
        public float abilityTime;
        public float chargePerBullet;

        private float charge;

        private bool abilityActive = false;


        public override void OnBullet(Bullet bullet, Collision2D collision)
        {
            bullet.transform.up = transform.up;
            bullet.OnContact();

            charge += chargePerBullet;
            charge = Mathf.Clamp01(charge);
        }

        public override void StartAbility()
        {
            if (charge < 1f || abilityActive) return;

            charge = 0f;
            abilityActive = true;

            //PlayerController.Instance.secondShield.SetActive(true);

            Timer.Wait(abilityTime, () =>
            {
                //PlayerController.Instance.secondShield.SetActive(false);
                abilityActive = false;
            });
        }
    }
}
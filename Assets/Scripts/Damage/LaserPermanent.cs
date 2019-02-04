using UnityEngine;
using System.Collections;

namespace Damage
{
    public class LaserPermanent : LaserBase
    {
        public const float LoopDamageRate = 10f;

        protected bool isDisabled = false;



        private void Start()
        {
            StartLoopDamage();
        }


        public void StartLoopDamage()
        {
            StartCoroutine(LoopDamage());
        }

        protected IEnumerator LoopDamage()
        {
            while (true)
            {
                if (LoopDamageRate == 0f || isDisabled) yield return 0;

                yield return new WaitForSeconds(1f / LoopDamageRate);

                SetToDirectionAndDamage(config.Damage / LoopDamageRate);
            }
        }


        public void Disable()
        {
            line.enabled = false;

            isDisabled = true;
        }

        public void Activate()
        {
            line.enabled = true;

            isDisabled = false;
        }
    }
}
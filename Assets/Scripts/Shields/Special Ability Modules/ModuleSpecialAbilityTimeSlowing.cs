using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    public class ModuleSpecialAbilityTimeSlowing : ModuleSpecialAbilityBase
    {
        [SerializeField] private float realtimeDuration = 3f;
        [SerializeField] private float slowedScale = 0.5f;
        [SerializeField] private float playerSlowedScale = 0.75f;

        private bool isActive = false;


        protected override void OnStartAbility()
        {
            isActive = true;

            this.InvokeDelegateUnscaled(StopAbility, realtimeDuration);

            Time.timeScale = slowedScale;
            PlayerController.Instance.TimeScale = playerSlowedScale / slowedScale;
        }

        protected override bool CanStartAbility()
        {
            return !isActive;
        }

        public override void StopAbility()
        {
            Time.timeScale = 1f;
            PlayerController.Instance.TimeScale = 1f;
            isActive = false;
        }
    }
}
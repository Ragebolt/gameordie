using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    public class ModuleSpecialAbilityDisable : ModuleSpecialAbilityBase
    {
        [SerializeField] private float duration = 1f;

        private bool isActive = false;


        protected override void OnStartAbility()
        {
            this.InvokeDelegate(StopAbility, duration);

            isActive = true;

            foreach (var enemy in PlayerController.Instance.Room.enemies)
            {
                enemy.Disable();
            }
        }

        protected override bool CanStartAbility()
        {
            return !isActive;
        }

        public override void StopAbility()
        {
            isActive = false;

            foreach (var enemy in PlayerController.Instance.Room.enemies)
            {
                enemy.Activate();
            }
        }
    }
}
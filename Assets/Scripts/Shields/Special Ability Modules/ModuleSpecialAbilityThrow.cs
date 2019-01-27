using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    public class ModuleSpecialAbilityThrow : ModuleSpecialAbilityBase
    {
        [SerializeField] private float maxDistance;
        [SerializeField] private GameObject thrownShieldPrefab;

        private Vector3 startPoint;
        private ThrownShield thrownShield;

        private ShieldState state = ShieldState.InHands;
        private enum ShieldState
        {
            InHands,
            FlightsAway,
            Arrive
        }

        private List<GameObject> objectsToDisable = new List<GameObject>();


        protected override void OnStartAbility()
        {
            startPoint = transform.position;
            thrownShield = Instantiate(thrownShieldPrefab, startPoint, Quaternion.identity).GetComponent<ThrownShield>();
            thrownShield.transform.up = Direction;
            thrownShield.ReturnShield = StopAbility;

            state = ShieldState.FlightsAway;

            this.InvokeWhile(AbilityUpdate, () => { return state != ShieldState.InHands; });

            foreach (var obj in objectsToDisable) obj.SetActive(false);
            ShieldRoot.GetComponent<ShieldMovement>().Renderer.gameObject.SetActive(false);
        }

        protected override bool CanStartAbility()
        {
            return state == ShieldState.InHands;
        }

        private void AbilityUpdate()
        {
            if (state == ShieldState.FlightsAway && Vector3.Distance(startPoint, thrownShield.transform.position) >= maxDistance) StopAbility();

            else if (state == ShieldState.Arrive)
            {
                Vector3 dir = thrownShield.transform.position - transform.position;

                thrownShield.transform.up = -dir.normalized;

                if (Vector3.Distance(transform.position, thrownShield.transform.position) <= 0.5f)
                {
                    state = ShieldState.InHands;

                    foreach (var obj in objectsToDisable) obj.SetActive(true);
                    ShieldRoot.GetComponent<ShieldMovement>().Renderer.gameObject.SetActive(true);

                    Destroy(thrownShield.gameObject);
                }
            }
        }

        public override void StopAbility()
        {
            state = ShieldState.Arrive;
            thrownShield.Speed *= 1.5f;
        }


        protected override void OnModuleGetted(ShieldModule module)
        {
            base.OnModuleGetted(module);

            if (module is ModuleReflectionBase)
            {
                objectsToDisable.Add(module.gameObject);
            }
            else if (module is ModuleActiveDefenceBase)
            {
                objectsToDisable.Add(module.gameObject);
            }
        }
    }
}
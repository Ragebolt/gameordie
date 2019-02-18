using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

namespace Entities.EnemyModules
{
    [AddComponentMenu("Enemy Modules/Laser Caster")]
    public class LaserCaster : Enemy
    {
        [HelpBox("Поведение врага, которое создаёт постоянный лазер")]
        [SerializeField] private GameObject laserPrefab;
        [SerializeField] private int lasersCount = 1;
        [SerializeField, Range(0f, 180f)] private float lasersOffset = 0f;
        public float laserDamagePerSecond = 1f;
        [SerializeField] private GameObject customCreator;

        private List<LaserPermanent> lasers = new List<LaserPermanent>();
        private bool isDisabled = false;


        void Start()
        {
            float side = -1f;
            for (int i = 0; i < lasersCount; i++)
            {
                Quaternion barrelRot = Quaternion.Euler(0f, 0f, side * ((i + 1) / 2) * lasersOffset);
                side *= -1;

                var laser = Instantiate(laserPrefab, transform.position, transform.rotation * barrelRot, transform).GetComponent<LaserPermanent>();

                laser.config = new ProjectiveBase.Config(laserDamagePerSecond);
                laser.Creator = customCreator ?? gameObject;

                laser.StartLoopDamage();

                if (isDisabled) laser.Disable();

                lasers.Add(laser);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            foreach (var laser in lasers) Destroy(laser.gameObject);
        }


        public override void Disable()
        {
            foreach (var laser in lasers) laser.Disable();

            isDisabled = true;
        }

        public override void Activate()
        {
            foreach (var laser in lasers) laser.Activate();

            isDisabled = false;
        }
    }
}
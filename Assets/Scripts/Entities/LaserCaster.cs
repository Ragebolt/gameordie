using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

namespace Entities
{
    public class LaserCaster : Enemy
    {
        [SerializeField] private GameObject laserPrefab;

        public float laserDamagePerSecond = 1f;

        private LaserPermanent laser;
        private bool isDisabled = false;


        void Start()
        {
            laser = Instantiate(laserPrefab, transform.position, transform.rotation, transform).GetComponent<LaserPermanent>();

            laser.config = new ProjectiveBase.Config(laserDamagePerSecond);
            laser.Creator = gameObject;

            laser.StartLoopDamage();

            if (isDisabled) laser.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (laser != null) Destroy(laser.gameObject);
        }


        public override void Disable()
        {
            if (laser != null) laser.Disable();

            isDisabled = true;
        }

        public override void Activate()
        {
            if (laser != null) laser.Activate();

            isDisabled = false;
        }
    }
}
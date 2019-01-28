using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

namespace Shields.Modules
{
    public class ModuleActiveDefenceLaser : ModuleActiveDefenceBase
    {
        [Header("Laser")]
        [SerializeField] private PrefabData laserPrefabData;
        [SerializeField] private float damage;


        protected override void OnBullet(Transform t)
        {
            Vector3 pos = t.position;
            Destroy(t.gameObject);

            Laser.CreateOneshotLaser(laserPrefabData.Prefab, pos, Direction, damage);

            OnAnyBullet();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

namespace Shields.Modules
{
    public class ModuleActiveDefenceDefault : ModuleActiveDefenceBase
    {
        [Header("Default")]
        [SerializeField] protected bool useNewBulletSettings = false;
        [SerializeField] protected Bullet.Settings newBulletSettings;

        [SerializeField] protected float speedMultiply = 1f;
        [SerializeField] protected float damageMultiply = 1f;


        protected override void OnBullet(Transform t)
        {
            Bullet bullet = t.GetComponent<Bullet>();

            bullet.Rigidbody.velocity = newBulletSettings.startSpeed * Direction;
            if (useNewBulletSettings) bullet.settings = newBulletSettings;
            bullet.settings.startSpeed *= speedMultiply;
            bullet.settings.damage *= damageMultiply;
            bullet.Creator = null;

            OnAnyBullet();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;
using Curve = UnityEngine.AnimationCurve;

namespace Shields.Modules
{
    public class ModuleActiveDefenceDefault : ModuleActiveDefenceBase
    {
        [Header("Default")]
        [SerializeField] protected bool useNewBulletSettings = false;
        [SerializeField] protected Bullet.Settings newBulletSettings;

        [SerializeField] protected Curve speedMultiply = Curve.Constant(0f, 1f, 1f);
        [SerializeField] protected Curve damageMultiply = Curve.Constant(0f, 1f, 1f);

        [SerializeField] protected float maxDistance = 1f;


        protected override void OnBullet(Transform t)
        {
            Bullet bullet = t.GetComponent<Bullet>();

            if (useNewBulletSettings) bullet.settings = newBulletSettings;

            float distance = Vector3.Distance(ShieldRoot.position, t.position);
            distance /= maxDistance;

            bullet.settings.startSpeed *= speedMultiply.Evaluate(distance);
            bullet.settings.damage *= speedMultiply.Evaluate(distance);
            bullet.Creator = null;

            bullet.Rigidbody.velocity = bullet.StartSpeed * Direction;

            OnAnyBullet();
        }
    }
}
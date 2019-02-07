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
        [SerializeField] protected bool useNewBulletConfig = false;
        [SerializeField] protected ProjectiveBase.Config newBulletConfig;

        [SerializeField] protected Curve speedMultiply = Curve.Constant(0f, 1f, 1f);
        [SerializeField] protected Curve damageMultiply = Curve.Constant(0f, 1f, 1f);

        [SerializeField] protected float maxDistance = 1f;


        protected override void OnBullet(Transform t)
        {
            Bullet bullet = t.GetComponent<Bullet>();

            if (useNewBulletConfig) bullet.config = newBulletConfig;

            float distance = Vector3.Distance(ShieldRoot.position, t.position);
            distance /= maxDistance;

            bullet.config.Speed *= speedMultiply.Evaluate(distance);
            bullet.config.Damage *= speedMultiply.Evaluate(distance);
            bullet.Creator = null;

            bullet.Rigidbody.velocity = bullet.config.Speed * Direction;

            OnAnyBullet();
        }
    }
}
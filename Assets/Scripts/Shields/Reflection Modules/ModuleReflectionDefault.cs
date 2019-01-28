using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

namespace Shields.Modules
{
    public class ModuleReflectionDefault : ModuleReflectionBase
    {
        public override void OnBullet(Bullet bullet, Collision2D collision)
        {
            bullet.OnContact();

            OnAnyBullet();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Legacy
{
    public abstract class ShieldBase : MonoBehaviour
    {
        public GameObject shieldRoot;


        public abstract void OnBullet(Bullet bullet, Collision2D collision);

        public abstract void StartAbility();

        public virtual void AbilityUpdate() { }
    }
}
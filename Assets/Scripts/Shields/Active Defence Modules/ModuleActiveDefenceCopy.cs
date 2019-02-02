using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

namespace Shields.Modules
{
    public class ModuleActiveDefenceCopy : ModuleActiveDefenceBase
    {
        [Header("Copies")]
        [SerializeField] protected int copiesCount = 2;
        [SerializeField] protected float angleOffset = 10f;


        protected override void OnBullet(Transform t)
        {
            Bullet bullet = t.GetComponent<Bullet>();

            int side = 1;
            for (int i = 0; i < copiesCount; i++)
            {
                Bullet b = Instantiate(bullet.gameObject, t.position, Quaternion.identity).GetComponent<Bullet>();
                b.transform.up = (Quaternion.Euler(0f, 0f, ((i / 2) + 1) * side * angleOffset) * Direction);

                side = -side;
            }

            bullet.Rigidbody.velocity = bullet.StartSpeed * Direction;

            OnAnyBullet();
        }
    }
}
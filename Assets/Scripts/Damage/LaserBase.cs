using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entities;

namespace Damage
{
    public abstract class LaserBase : ProjectiveBase
    {
        [Space]
        [HelpBox("Снаряд, предстваляющий лазер. Бывает двух типов: \n Shoot - одиночный выстрел (похож на пулю) \n Permanent - постоянный")]
        [SerializeField] protected LineRenderer line;

        protected Vector3 StartPoint
        {
            get { return line.GetPosition(0); }
            set { line.SetPosition(0, value); }
        }
        protected Vector3 EndPoint
        {
            get { return line.GetPosition(1); }
            set { line.SetPosition(1, value); }
        }


        protected RaycastResult Raycast()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up);

            hits.OrderBy(hit => hit.distance);

            foreach (var hit in hits)
            {
                if (hit.transform.tag == "Shield") return new RaycastResult(hit, false);

                string layerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
                if (hit.transform.gameObject != Creator && layerName != "Ground") return new RaycastResult(hit, true);
            }

            return new RaycastResult(hits[0], false);
        }

        protected void SetToDirection(RaycastResult raycastResult)
        {
            EndPoint = Vector3.Distance(raycastResult.Hit.point, transform.position) * Vector3.up;
        }

        protected void CauseDamage(RaycastResult raycastResult, float amount)
        {
            if (!raycastResult.CanDamage) return;

            Damagable damagable = raycastResult.Hit.transform.GetComponent<Damagable>();

            if (damagable != null && damagable.gameObject != Creator)
            {
                damagable.GetDamage(amount);
            }
        }

        protected void SetToDirectionAndDamage(float damage)
        {
            var hit = Raycast();

            SetToDirection(hit);
            CauseDamage(hit, damage);
        }



        protected struct RaycastResult
        {
            public RaycastHit2D Hit { get; private set; }
            public bool CanDamage { get; private set; }

            public RaycastResult(RaycastHit2D hit, bool canDamage)
            {
                Hit = hit;
                CanDamage = canDamage;
            }
        }
    }
}
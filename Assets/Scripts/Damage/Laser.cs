using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Entities;

namespace Damage
{
    public class Laser : ProjectiveBase
    {
        public LineRenderer line;
        public PolygonCollider2D polygonCollider;

        private Vector3 startPoint;
        public Vector3 StartPoint
        {
            get { return startPoint; }
            set
            {
                startPoint = value;

                line.SetPosition(0, value);

                polygonCollider.points[0] = value;
                polygonCollider.points[1] = (startPoint - endPoint) / 2f;
            }
        }

        private Vector3 endPoint;
        public Vector3 EndPoint
        {
            get { return endPoint; }
            set
            {
                endPoint = value;

                line.SetPosition(1, value);

                polygonCollider.points[2] = value;
                polygonCollider.points[1] = (startPoint - endPoint) / 2f;
            }
        }

        [SerializeField] private float damage;
        public override float Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        public float LoopDamageRate { get; set; }

        public Vector3 Direction { get; set; }


        private bool isDisabled = false;


        void Start()
        {

        }

        void Update()
        {

        }

        public void SetToDirection(Vector3 direction)
        {
            Direction = direction;

            RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, direction);

            hits.OrderBy(hit => hit.distance);


            RaycastHit2D rightHit = hits[0];

            foreach (var hit in hits)
            {
                if (hit.transform.gameObject != Creator && LayerMask.LayerToName(hit.transform.gameObject.layer) != "Ground" && hit.transform.tag != "Laser Caster")
                {
                    rightHit = hit;
                    break;
                }
            }

            EndPoint = rightHit.point;
        }

        public void CauseDamage(float amount)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, Direction);

            hits.OrderBy(hit => hit.distance);


            RaycastHit2D rightHit = hits[0];

            foreach (var hit in hits)
            {
                if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Shield") return;

                if (hit.transform.gameObject != Creator && LayerMask.LayerToName(hit.transform.gameObject.layer) != "Ground" && hit.transform.tag != "Laser Caster")
                {
                    rightHit = hit;
                    break;
                }
            }

            Damagable damagable = rightHit.transform.GetComponent<Damagable>();

            if (damagable != null && damagable.gameObject != Creator)
            {
                damagable.GetDamage(amount);
            }
        }

        private void SetToDirectionAndHit(Vector3 direction, float damage)
        {
            Direction = direction;

            RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, direction);

            RaycastHit2D rightHit = hits[0];

            foreach (var hit in hits)
            {
                if (hit.transform.gameObject != Creator && hit.transform.tag != "Shield" && LayerMask.LayerToName(hit.transform.gameObject.layer) != "Ground")
                {
                    rightHit = hit;
                    break;
                }
            }

            EndPoint = rightHit.point;

            Damagable damagable = rightHit.transform.GetComponent<Damagable>();

            if (damagable != null && damagable.gameObject != Creator)
            {
                damagable.GetDamage(damage);
            }
        }

        public void StartLoopDamage()
        {
            StartCoroutine(LoopDamage());
        }

        private IEnumerator LoopDamage()
        {
            while (true)
            {
                if (LoopDamageRate == 0f || isDisabled) yield return 0;

                yield return new WaitForSeconds(1f / LoopDamageRate);

                SetToDirection(Direction);

                CauseDamage(Damage);
            }
        }

        public void Disable()
        {
            line.enabled = false;

            isDisabled = true;
        }

        public void Activate()
        {
            line.enabled = true;

            isDisabled = false;
        }


        public static Laser CreateOneshotLaser(GameObject prefab, Vector3 position, Vector3 direction, float damage, float duration = 0.2f)
        {
            Laser laser = Instantiate(prefab).GetComponent<Laser>();
            laser.StartPoint = position;
            laser.SetToDirectionAndHit(direction, damage);
            Destroy(laser.gameObject, duration);

            return laser;
        }
    }
}
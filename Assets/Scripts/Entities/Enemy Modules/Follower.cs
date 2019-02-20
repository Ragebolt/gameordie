using System.Collections;
using System.Collections.Generic;
using Generation;
using UnityEngine;

namespace Entities.EnemyModules
{
    [AddComponentMenu("Enemy Modules/Follower")]
    public class Follower : Enemy
    {
        [HelpBox("Поведение врага, заставляющее его преследовать игрока")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float stopDistance = 0f;
        [SerializeField] private bool radialMovementInRande = false;

        private Transform target;
        private bool isDisabled = false;

        [SerializeField] private Configuration config;
        [System.Serializable]
        public class Configuration
        {
            public float moveSpeed;
        }


        void Start()
        {
            target = PlayerController.Instance.Origin;
            if (transform.parent != null) transform.parent.rotation = Quaternion.identity;
        }


        void FixedUpdate()
        {
            if (!isDisabled)
            {
                if ((target.position - transform.position).sqrMagnitude > stopDistance * stopDistance) Move();
                else if (radialMovementInRande) RadialMove();
            }
        }

        private void Move()
        {
            Vector3 targetPos = target.position; targetPos.z = 0f;

            Vector3 dir = targetPos - transform.position;
            dir.Normalize();

            rb.position += (Vector2)(dir * config.moveSpeed * Time.fixedDeltaTime);
        }

        private void RadialMove()
        {
            Vector3 targetPos = target.position; targetPos.z = 0f;
            Vector3 dir = targetPos - transform.position;
            Vector3 radialDir = Quaternion.Euler(0f, 0f, 90f) * dir;

            rb.position += (Vector2)(radialDir.normalized * config.moveSpeed * Time.fixedDeltaTime);
        }


        public override void Disable()
        {
            isDisabled = true;
        }

        public override void Activate()
        {
            isDisabled = false;
        }
    }
}
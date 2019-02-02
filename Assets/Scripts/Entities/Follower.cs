using System.Collections;
using System.Collections.Generic;
using Generation;
using UnityEngine;

namespace Entities
{
    public class Follower : Enemy
    {
        [SerializeField] private ObservedZone viewZone;
        [SerializeField] private Rigidbody2D rb;

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
            transform.parent.rotation = Quaternion.identity;
        }


        void Update()
        {
            if (!isDisabled) Move();
        }

        private void Move()
        {
            Vector3 targetPos = target.position; targetPos.z = 0f;

            Vector3 dir = targetPos - transform.position;
            dir.Normalize();

            rb.position += (Vector2)(dir * config.moveSpeed * Time.fixedDeltaTime);
        }


        public override void Disable()
        {
            isDisabled = true;
        }

        public override void Activate()
        {
            isDisabled = false;
        }


        public string GetConfig()
        {
            return JsonUtility.ToJson(config);
        }

        public void SetConfig(string value)
        {
            config = JsonUtility.FromJson<Configuration>(value);
        }

        public System.Type GetJsonType()
        {
            return typeof(Configuration);
        }

        public RoomObject GetRoomObject()
        {
            return new RoomObjectFollower();
        }
    }
}
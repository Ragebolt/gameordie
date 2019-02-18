using System;
using System.Collections;
using System.Collections.Generic;
using Generation;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Damage;

namespace Entities.Legacy
{
    public class Turret : Enemy, IConfig
    {
        [HelpBox("[Устаревшее] Поведение турели")]
        [SerializeField] private Transform weapon;
        [SerializeField] private Transform shootingPoint;
        [SerializeField] private GameObject projectivePrefab;

        [Space]
        [SerializeField] private Configuration config;
        [System.Serializable]
        private class Configuration
        {
            public float weaponRotationSpeed;
            public float shootingRate;
            public ProjectiveBase.Config projectiveConfig;
        }

        [Space]
        [SerializeField] private ObservedZone viewZone;

        private Transform target;
        private Transform Target
        {
            get { return target; }
            set
            {
                target = value;

                if (target != null && !isShooting) StartShooting();
            }
        }
        private bool CanShoot
        {
            get
            {
                if (isDisabled) return false;

                RaycastHit2D[] hits = Physics2D.RaycastAll(shootingPoint.position, weapon.up);

                foreach (var hit in hits)
                {
                    if (hit.transform == target) return true;
                }

                return false;
            }
        }
        private bool isShooting = false;
        private bool isDisabled = false;


        void Start()
        {
            //viewZone.tags.Add("Player");
            //viewZone.OnEnter += (Transform t) => { if (Target == null) Target = t; };
            //viewZone.OnExit += (Transform t) => { if (Target == t) Target = viewZone.GetObject(); };

            Target = PlayerController.Instance.transform;
        }

        void Update()
        {
            if (Target != null && !isDisabled)
            {
                RotateWeapon();
            }
        }

        private void RotateWeapon()
        {
            Vector3 targetPos = Target.position;

            Vector3 dir = targetPos - weapon.position;

            float angle = Vector3.Angle(weapon.up, dir);
            float angleRight = Vector3.Angle(weapon.right, dir);
            float angleLeft = Vector3.Angle(-weapon.right, dir);

            float sign = 1;
            if (angleRight < angleLeft) sign = -1;

            weapon.Rotate(0f, 0f, sign * Mathf.Clamp(config.weaponRotationSpeed * Time.deltaTime, 0f, angle));
        }

        private void StartShooting()
        {
            isShooting = true;
            StartCoroutine(Shooting());
        }

        private IEnumerator Shooting()
        {
            while (Target != null)
            {
                while (!CanShoot) yield return 0;

                Shoot();

                yield return new WaitForSeconds(1f / config.shootingRate);
            }

            isShooting = false;
        }

        private void Shoot()
        {
            var bullet = Instantiate(projectivePrefab, shootingPoint.position, weapon.rotation);

            ProjectiveBase controller = bullet.GetComponent<ProjectiveBase>();

            controller.Creator = gameObject;
            controller.config = config.projectiveConfig;
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

        public Type GetJsonType()
        {
            return typeof(Configuration);
        }

#if UNITY_EDITOR
        private SerializedObject so;
        public SerializedProperty GetProperty()
        {
            if (so == null) so = new SerializedObject(this);
            return so.FindProperty("config");
        }
        public void ApplyProperty()
        {
            so.ApplyModifiedProperties();
        }
#endif
    }
}
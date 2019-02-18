using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using Damage;

namespace Entities.EnemyModules
{
    [AddComponentMenu("Enemy Modules/Shooting")]
    public class Shooting : Enemy
    {
        [HelpBox("Поведение, заставляющее врага стрелять (пулями или лазером-выстрелом)")]
        [SerializeField] private Transform shootingPoint;
        [SerializeField] private float shootingPointOffset;
        [SerializeField] private GameObject projectivePrefab;

        [SerializeField] private ShootingCondition shootingCondition;
        private enum ShootingCondition
        {
            Always,
            Custom
        }

        [SerializeField] private ProjectiveBase.Config projectiveConfig;
        public ProjectiveBase.Config ProjectiveConfig
        {
            get => projectiveConfig; set => projectiveConfig = value;
        }

        [SerializeField] private float shootingRate = 1f;
        public float ShootingRate
        {
            get => shootingRate; set => shootingRate = value;
        }

        [SerializeField, Range(0f, 180f)] private float spread = 0f;
        [SerializeField] private int bulletsPerShootCount = 1;
        [SerializeField] private int barrelsCount = 1;
        [SerializeField, Range(0f, 180f)] private float barrelsOffset = 0f;

        private bool canShoot = true;
        private bool isDisabled = false;


        private void Update()
        {
            if (shootingCondition == ShootingCondition.Always) OnShootCondition();
        }

        public void OnShootCondition()
        {
            if (!isDisabled && canShoot && shootingRate > 0f)
            {
                canShoot = false;
                Shoot();
                StartCoroutine(ShootWait());
            }
        }

        private void Shoot()
        {
            float side = -1f;
            for (int i = 0; i < barrelsCount; i++)
            {
                Quaternion barrelRot = Quaternion.Euler(0f, 0f, side * ((i + 1) / 2) * barrelsOffset);
                side *= -1;
                for (int j = 0; j < bulletsPerShootCount; j++)
                {
                    Quaternion spreadRot = Quaternion.Euler(0f, 0f, Random.Range(-spread, spread));
                    Quaternion rot = transform.rotation * spreadRot * barrelRot;
                    var bullet = Instantiate(projectivePrefab, shootingPoint.position + rot * Vector3.up * shootingPointOffset, rot);

                    ProjectiveBase controller = bullet.GetComponent<ProjectiveBase>();

                    controller.Creator = gameObject;
                    controller.config = projectiveConfig;
                }
            }
        }

        private IEnumerator ShootWait()
        {
            yield return new WaitForSeconds(1f / shootingRate);

            canShoot = true;
        }


        public override void Activate()
        {
            isDisabled = false;
        }

        public override void Disable()
        {
            isDisabled = true;
        }
    }
}
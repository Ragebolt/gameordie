using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Damage;

namespace Entities
{
    public class PatternBulletsSpawner : Enemy
    {
        public float safeSpawnRadius;
        public Pattern[] patterns;

        [System.Serializable]
        public class Pattern
        {
            public GameObject bulletPrefab;
            public int bulletsPerShoot;
            public float bulletAcceleration;
            public float bulletRotateSpeed;
            public float shootingRate = 1f;
            public float angleIncrement = 2f;

            [Space]
            public float angleIncrementIterations = 1f;
            public int iterationsToReset = 1;

            [Space]
            public Bullet.Settings bulletsSettings;

            private bool rightSpawn = true; // Спавнить пулю справа или слева
            private int lastShootId = 0;
            private int iteration = 0;


            public PatternBulletsSpawner Spawner { get; set; }

            public bool Pause { get; set; }



            public IEnumerator Shooting(Transform transform)
            {
                while (true)
                {
                    while (Pause) yield return 0;

                    for (int i = 0; i < bulletsPerShoot; i++)
                    {
                        Shoot(transform, i);
                    }

                    rightSpawn = true;

                    iteration++;

                    if (iterationsToReset == iteration) iteration = 0;

                    yield return new WaitForSeconds(1f / shootingRate);
                }
            }

            public void Shoot(Transform transform, int index)
            {
                float side = 1f;
                if (!rightSpawn) side = -1f;

                float angle = side * ((index + 1) / 2) * angleIncrement + iteration * angleIncrementIterations;

                var bullet = Instantiate(bulletPrefab, transform.position + (Quaternion.Euler(0f, 0f, angle) * transform.up * Spawner.safeSpawnRadius), Quaternion.identity);

                Bullet ctrl = bullet.GetComponent<Bullet>();

                ctrl.Acceleration = bulletAcceleration;
                ctrl.RotateSpeed = bulletRotateSpeed;
                ctrl.Creator = transform.gameObject;
                ctrl.settings = bulletsSettings;

                bullet.transform.Rotate(0f, 0f, angle);

                rightSpawn = !rightSpawn;
                lastShootId++;
            }
        }



        void Start()
        {
            foreach (Pattern pattern in patterns)
            {
                StartCoroutine(pattern.Shooting(transform));
                pattern.Spawner = this;
            }
        }


        public override void Disable()
        {
            foreach (Pattern pattern in patterns)
            {
                pattern.Pause = true;
            }
        }

        public override void Activate()
        {
            foreach (Pattern pattern in patterns)
            {
                pattern.Pause = false;
            }
        }
    }
}
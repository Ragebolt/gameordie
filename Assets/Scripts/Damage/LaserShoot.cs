using UnityEngine;
using System.Collections;

namespace Damage
{
    public class LaserShoot : LaserBase
    {
        private void Start()
        {
            StartPoint = Vector3.zero;
            SetToDirectionAndDamage(config.Damage);
            Destroy(gameObject, 0.2f);
        }


        public static LaserShoot Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float damage)
        {
            var laser = Instantiate(prefab, position, rotation).GetComponent<LaserShoot>();
            laser.config = new Config(damage);
            return laser;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

namespace Damage
{
    public class Explosion : MonoBehaviour
    {
        public GameObject Creator { get; set; }

        [SerializeField] private bool explodeOnStart = true;
        [SerializeField] private float damage = 1f;
        [SerializeField] private float radius = 1f;
        [SerializeField] private bool destroyAfterExplode = true;
        [SerializeField] private float destroyAfterExplodeTime = 0.2f;


        private void Start()
        {
            if (explodeOnStart) Explode();
        }

        public void Explode()
        {
            var results = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (var result in results)
            {
                Damagable damagable = result.GetComponent<Damagable>();
                if (damagable != null && damagable != Creator) damagable.GetDamage(damage);
            }

            if (destroyAfterExplode) Destroy(gameObject, destroyAfterExplodeTime);
        }
    }
}
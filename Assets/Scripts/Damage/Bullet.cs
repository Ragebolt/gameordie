using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Generation;
using Entities;
using Shields.Modules;

namespace Damage
{
    public class Bullet : ProjectiveBase
    {
        [SerializeField] private Rigidbody2D rb;
        public Rigidbody2D Rigidbody { get { return rb; } }

        [SerializeField] private Gradient gradient;
        [SerializeField] private new SpriteRenderer renderer;

        private int contactsCounter;
        private int ContactsCounter
        {
            get { return contactsCounter; }
            set
            {
                contactsCounter = value;
                if (value >= config.ContactsToDestroy + 1) Destroy(gameObject);
                renderer.color = gradient.Evaluate((float)contactsCounter / (config.ContactsToDestroy));
            }
        }

        private Vector2Int roomCoords;

        public bool IsCopy { get; set; } = false;

        public enum DestroyCondition
        {
            AnyHit,
            DamagableHit,
            NoShieldHit,
            HitCount
        }



        void Start()
        {
            ContactsCounter = 0;

            rb.velocity = transform.up * config.Speed;
        }


        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.gameObject == Creator) return;

            if (collision.collider.tag == "Shield")
            {
                ModuleReflectionBase reflection = collision.collider.GetComponent<ModuleReflectionBase>();

                if (Vector3.Dot(transform.up, reflection.Direction) > 0f) return;

                reflection.OnBullet(this, collision);
                return;
            }

            Damagable damagable = collision.collider.GetComponent<Damagable>();
            if (damagable != null)
            {
                damagable.GetDamage(config.Damage);
                if (config.DestroyCondition == DestroyCondition.DamagableHit) Destroy(gameObject);
            }

            if (config.DestroyCondition == DestroyCondition.NoShieldHit) Destroy(gameObject);

            OnContact();
        }

        public void OnContact()
        {
            switch (config.DestroyCondition)
            {
                case DestroyCondition.AnyHit:
                    Destroy(gameObject);
                    return;
                case DestroyCondition.HitCount:
                    ContactsCounter++;
                    break;
            }

            Creator = null;
        }

        private void OnDestroy()
        {
            if (config.DestroyEffect != null) Instantiate(config.DestroyEffect, transform.position, Quaternion.identity);
        }
    }
}
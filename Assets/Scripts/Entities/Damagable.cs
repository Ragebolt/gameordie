using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VisualComponents.Blink;

namespace Entities
{
    public class Damagable : MonoBehaviour
    {
        [SerializeField] private SpriteBlinkAnimation _animation;
        [SerializeField] private GameObject customObjectToDestroy;
        [SerializeField] private float health;
        public float Health { get { return health; } }

        [SerializeField] private float maxHealth;

        public event System.Action<float, float> OnHealthChanged = (f1, f2) => { };


        public void GetDamage(float amount)
        {
            health -= amount;

            if (_animation != null)
            {
                _animation.TryBlink();
            }

            OnHealthChanged(health, maxHealth);

            if (health <= 0f) Death();
        }

        public void Death()
        {
            if (customObjectToDestroy == null) Destroy(gameObject);
            else Destroy(customObjectToDestroy);
        }
    }
}
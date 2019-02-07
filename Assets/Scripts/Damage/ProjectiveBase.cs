using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damage
{
    public abstract class ProjectiveBase : MonoBehaviour
    {
        public GameObject Creator { get; set; }

        [HelpBox("Снаряд", 30f)]
        public Config config;


        [System.Serializable]
        public class Config
        {
            [SerializeField] private float damage = 1f;
            public float Damage
            {
                get { return damage; }
                set { damage = value; }
            }


            [Header("Bullets Only")]
            [SerializeField] private float speed = 10f;
            public float Speed
            {
                get { return speed; }
                set { speed = value; }
            }

            [SerializeField] private Bullet.DestroyCondition destroyCondition = Bullet.DestroyCondition.AnyHit;
            public Bullet.DestroyCondition DestroyCondition
            {
                get { return destroyCondition; }
                set { destroyCondition = value; }
            }

            [SerializeField] private int contactsToDestroy = 1;
            public int ContactsToDestroy
            {
                get { return contactsToDestroy; }
                set { contactsToDestroy = value; }
            }

            [SerializeField] private GameObject destroyEffect = null;
            public GameObject DestroyEffect
            {
                get { return destroyEffect; }
                set { destroyEffect = value; }
            }


            public Config(float damage)
            {
                this.damage = damage;
            }
        }
    }
}
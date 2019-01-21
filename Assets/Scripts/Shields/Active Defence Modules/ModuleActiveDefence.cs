using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    public class ModuleActiveDefence : ShieldModule
    {
        public ObservedZone DefenceZone { get; set; }
        public GameObject BulletsCreator { get; set; }
        public VisualComponents.Blink.SpriteBlinkAnimation Animation { get; set; }

        [SerializeField] private float chargePerBullet = 0.02f;
        [SerializeField] private float activeTime = 0.1f;
        [SerializeField] private Bullet.Settings newBulletSettings;

        private bool isActive = false;


        /// <summary>
        /// Старт активной защиты
        /// </summary>
        public void StartActiveDefence()
        {
            if (isActive) return;

            MonoBehaviourExtension.InvokeDelegate(this, StopActiveDefence, activeTime);
            DefenceZone.OnEnter += OnBullet;
            Animation.TryBlink();

            foreach (var obj in DefenceZone.GetObjects()) OnBullet(obj);

            OnStartDefence();
        }

        /// <summary>
        /// Действия после старта для переопределения
        /// </summary>
        protected virtual void OnStartDefence() { }


        /// <summary>
        /// Прервать защиту
        /// </summary>
        public void StopActiveDefence()
        {
            DefenceZone.OnEnter -= OnBullet;
            isActive = false;
        }


        /// <summary>
        /// Вызывается при попадании пули
        /// </summary>
        /// <param name="t"></param>
        protected virtual void OnBullet(Transform t)
        {
            Bullet bullet = t.GetComponent<Bullet>();

            bullet.Rigidbody.velocity = newBulletSettings.startSpeed * ShieldController.Direction;
            bullet.settings = newBulletSettings;
            bullet.Creator = BulletsCreator;

            OnAnyBullet();
        }

        protected void OnAnyBullet()
        {
            ShieldController.SpecialAbilityModule?.AddCharge(chargePerBullet);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Damage;

namespace Shields.Modules
{
    public abstract class ModuleReflectionBase : ShieldModule, ISpecialChargeProvider
    {
        public event System.Action<float> OnChargeGenerated = (f) => { };
        public event System.Action OnActivate = () => { };

        public bool CanUserControl { get; set; } = true; 
        public bool Active { get; private set; } = false; 

        [HelpBox("Модуль щита, отвечающий за пассивное отбивание снарядов при касании")]
        [SerializeField] private float specialChargePerBullet = 0.01f;
        [SerializeField] protected new Collider2D collider;

        [SerializeField] private float charge = 1f;
        private float Charge
        {
            get { return charge; }
            set
            {
                charge = Mathf.Clamp(value, 0f, maxCharge);
                UI.UIController.ShieldChargeBar.Refresh(charge, maxCharge);
            }
        }
        [SerializeField] private float maxCharge = 1f;
        [SerializeField] private float chargeSpendPerSecond = 0.15f;
        [SerializeField] private float chargeSpendPerBullet = 0.05f;
        [SerializeField] private float chargeFillPerSecond = 0.25f;
        [SerializeField] private float reloadTime = 0.25f;

        private bool isFullRecharge = false;
        private bool reloaded = true;

        protected SpriteRenderer shieldRenderer;


        private void Start()
        {
            shieldRenderer = ShieldRoot.GetComponent<ShieldMovement>().Renderer;
            shieldRenderer.enabled = false;
            collider.enabled = false;

            InputController.OnShieldActivateButton += () => { if (CanUserControl) Activate(); };
            InputController.OnShieldDisableButton += () => { if (CanUserControl) Disable(); };

            Charge = charge;
        }


        /// <summary>
        /// Активация защиты
        /// </summary>
        public void Activate()
        {
            if (Active || !reloaded || (isFullRecharge && charge < maxCharge)) return;
            isFullRecharge = false;
            Active = true;

            OnActivate();

            collider.enabled = true;
            shieldRenderer.enabled = true;

            this.InvokeWhile(() =>
            {
                Charge -= chargeSpendPerSecond * Time.deltaTime;
            }, () => (Charge > 0) && (Active), () =>
            {
                if (Charge == 0) isFullRecharge = true;
                Disable();
            });
        }

        /// <summary>
        /// Дизактивация защиты
        /// </summary>
        public void Disable()
        {
            if (!Active) return;
            Active = false;

            collider.enabled = false;
            shieldRenderer.enabled = false;

            this.InvokeWhile(() =>
            {
                Charge += chargeFillPerSecond * Time.deltaTime;
            }, () => !Active);

            reloaded = false;
            this.InvokeDelegate(() => reloaded = true, reloadTime);
        }


        /// <summary>
        /// Вызывать при касании пули щита
        /// </summary>
        /// <param name="bullet"> Пуля, которая каснулась щита </param>
        /// <param name="collision"> Информация о касании </param>
        public abstract void OnBullet(Bullet bullet, Collision2D collision);

        /// <summary>
        /// Вызывать при отбивании любой пули
        /// </summary>
        protected void OnAnyBullet()
        {
            Charge -= chargeSpendPerBullet;
            OnChargeGenerated(specialChargePerBullet);
        }
    }
}
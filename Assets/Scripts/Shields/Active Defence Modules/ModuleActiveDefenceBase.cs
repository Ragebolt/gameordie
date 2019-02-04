using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualComponents.Blink;
using Entities;

namespace Shields.Modules
{
    public abstract class ModuleActiveDefenceBase : ShieldModule, ISpecialChargeProvider
    {
        public event System.Action<float> OnChargeGenerated = (f) => { };

        [HelpBox("Модуль щита, отвечающий за активное отбивание снарядов (на ЛКМ) при попадании в зону")]
        [SerializeField] private float chargePerBullet = 0.02f;
        [SerializeField] protected float activeTime = 0.1f;
        [SerializeField] private ObservedZone activeDefenceZone;
        [SerializeField] private SpriteBlinkAnimation activeDefenceAnimation;

        private bool isActive = false;



        private void Start()
        {
            GetAnotherModules();
        }


        /// <summary>
        /// Старт активной защиты
        /// </summary>
        public void StartActiveDefence()
        {
            if (isActive || !gameObject.activeSelf) return;

            this.InvokeDelegate(StopActiveDefence, activeTime);
            activeDefenceZone.OnEnter += OnBullet;
            activeDefenceAnimation.TryBlink();

            foreach (var obj in activeDefenceZone.GetObjects()) OnBullet(obj);

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
            activeDefenceZone.OnEnter -= OnBullet;
            isActive = false;
        }


        /// <summary>
        /// Вызывается при попадании пули
        /// </summary>
        /// <param name="t"></param>
        protected abstract void OnBullet(Transform t);


        /// <summary>
        /// Вызывать при отбивании любой пули
        /// </summary>
        protected void OnAnyBullet()
        {
            OnChargeGenerated(chargePerBullet);
        }


        protected override void OnModuleGetted(ShieldModule module)
        {
            ModuleReflectionBase moduleReflection = module as ModuleReflectionBase;
            if (moduleReflection != null) moduleReflection.OnActivate += StartActiveDefence;
        }
    }
}
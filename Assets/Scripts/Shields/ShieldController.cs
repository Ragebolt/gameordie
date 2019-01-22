using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    /// <summary>
    /// Основа щита
    /// </summary>
    public sealed class ShieldController : MonoBehaviour
    {
        [SerializeField] private ModuleReflection reflectionModule;
        public ModuleReflection ReflectionModule
        {
            get { return reflectionModule; }
            set { reflectionModule = value; }
        }

        [SerializeField] private ModuleActiveDefence activeDefenceModule;
        public ModuleActiveDefence ActiveDefenceModule
        {
            get { return activeDefenceModule; }
            set { activeDefenceModule = value; }
        }

        [SerializeField] private ModuleSpecialAbilityBase specialAbilityModule;
        public ModuleSpecialAbilityBase SpecialAbilityModule
        {
            get { return specialAbilityModule; }
            set { specialAbilityModule = value; }
        }

        public Transform ShieldRoot { get { return transform; } }
        public Vector3 Direction { get; private set; }

        public event System.Action<float, float> OnSpecialAbilityChargeChanged = (f1, f2) => { };

        [SerializeField] private GameObject owner;
        [SerializeField] private Transform visualShieldTransform;
        [SerializeField] private Transform colliderShieldTransform;
        [SerializeField] private new Renderer renderer;
        [SerializeField] private ObservedZone activeDefenceZone;
        [SerializeField] private VisualComponents.Blink.SpriteBlinkAnimation activeDefenceAnimation;


        private void Start()
        {
            reflectionModule.ShieldController = this;
            activeDefenceModule.ShieldController = this;
            if (specialAbilityModule != null) specialAbilityModule.ShieldController = this;

            activeDefenceZone.tags.Add("Bullet");
            activeDefenceModule.DefenceZone = activeDefenceZone;
            activeDefenceModule.BulletsCreator = owner;
            activeDefenceModule.Animation = activeDefenceAnimation;
        }


        public void SetDirection(Vector3 direction, int sortingOrder)
        {
            Direction = direction;

            Quaternion rot = Quaternion.LookRotation(Vector3.forward, direction);

            visualShieldTransform.localRotation = Quaternion.Euler(0f, -rot.eulerAngles.z + 270f, 0f);
            colliderShieldTransform.rotation = rot * Quaternion.Euler(0f, 0f, 90f);

            float angle = Mathf.Repeat(rot.eulerAngles.z, 360f);
            if (angle > 270f || angle < 90f) renderer.sortingOrder = sortingOrder - 1;
            else renderer.sortingOrder = sortingOrder + 1;
        }
    
        public void SpecialAbilityChargeChanged(float value)
        {
            OnSpecialAbilityChargeChanged(value, 1f);
        }
    }
}
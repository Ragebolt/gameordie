using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Entities.EnemyModules
{
    [AddComponentMenu("Enemy Modules/Rotation")]
    public class Rotation : Enemy
    {
        [HelpBox("Поведение, позволяющее вращаться")]
        [SerializeField] private Mode mode = Mode.Constant;
        private enum Mode
        {
            Constant,
            Aiming
        }

        [SerializeField] private float rotationSpeed = 360f;
        public float RotationSpeed
        { get => rotationSpeed; set => rotationSpeed = value; }

        [SerializeField] private TargetInFrontEvent targetInFrontEvent;

        private Transform Transform => transform;
        private Transform Target => PlayerController.Instance.transform;
        private bool isDisabled = false;


        private void Update()
        {
            if (!isDisabled)
            {
                if (mode == Mode.Aiming) Aim();
                else Transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            }
        }

        private void Aim()
        {
            if (Target == null) return;

            Vector3 dir = (Target.position - Transform.position).normalized;

            float angle = Vector3.Cross(Transform.up, dir).z * 90f;

            Transform.Rotate(0f, 0f, Mathf.Sign(angle) * Mathf.Min(Mathf.Abs(rotationSpeed) * Time.deltaTime, Mathf.Abs(angle)));

            if (Mathf.Abs(angle) < 5f) targetInFrontEvent.Invoke();
        }


        public override void Activate()
        {
            isDisabled = false;
        }

        public override void Disable()
        {
            isDisabled = true;
        }


        [System.Serializable]
        class TargetInFrontEvent : UnityEvent { }
    }
}
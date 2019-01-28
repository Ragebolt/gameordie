using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

namespace Shields
{
    public class ShieldMovement : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private new SpriteRenderer renderer;

        public SpriteRenderer Renderer { get { return renderer; } }


        private void Start()
        {
            InputController.OnDirectionChaged += () => { SetDirection(InputController.Direction, PlayerController.Instance.Renderer.sortingOrder); };
        }

        public void SetDirection(Vector3 direction, int sortingOrder)
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, direction);

            transform.rotation = rot * Quaternion.Euler(0f, 0f, 90f);

            float angle = Mathf.Repeat(-rot.eulerAngles.z, 360f);
            bool left = angle > 180f;
            float localAngle = Mathf.Repeat(angle, 180f);
            if (left) localAngle = 180f - localAngle;

            //Debug.Log(angle + "  " + localAngle);

            animator.SetFloat("Rotation", localAngle / 180f);

            renderer.flipX = left;

            if (angle > 270f || angle < 90f) renderer.sortingOrder = sortingOrder - 1;
            else renderer.sortingOrder = sortingOrder + 1;
        }
    }
}
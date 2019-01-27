using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields
{
    public class ShieldMovement : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private new Renderer renderer;

        public Renderer Renderer { get { return renderer; } }


        private void Start()
        {
            InputController.OnDirectionChaged += () => { SetDirection(InputController.Direction, PlayerController.Instance.Renderer.sortingOrder); };
        }

        public void SetDirection(Vector3 direction, int sortingOrder)
        {
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, direction);

            transform.rotation = rot * Quaternion.Euler(0f, 0f, 90f);

            float angle = Mathf.Repeat(-rot.eulerAngles.z, 360f);

            animator.SetFloat("Rotation", angle / 360f);

            if (angle > 270f || angle < 90f) renderer.sortingOrder = sortingOrder - 1;
            else renderer.sortingOrder = sortingOrder + 1;
        }
    }
}
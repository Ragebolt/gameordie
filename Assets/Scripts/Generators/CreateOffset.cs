using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    public class CreateOffset : MonoBehaviour
    {
        [HelpBox("Добавлет отступ при создании и уничтожается")]
        public Vector3 positionOffset;

        private void Start()
        {
            transform.position += positionOffset;
            Destroy(this);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damage
{
    public abstract class ProjectiveBase : MonoBehaviour
    {
        public GameObject Creator { get; set; }
        public abstract float Damage { get; set; }
    }
}
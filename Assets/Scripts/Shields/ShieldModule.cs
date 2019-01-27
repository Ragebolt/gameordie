using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    /// <summary>
    /// Базовый класс для любых модулей щита
    /// </summary>
    public abstract class ShieldModule : MonoBehaviour
    {
        /// <summary>
        /// Основа щита
        /// </summary>
        public ShieldController ShieldController { get; set; }
    }
}
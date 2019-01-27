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
<<<<<<< HEAD
        /// <summary>
        /// Основа щита
        /// </summary>
        public ShieldController ShieldController { get; set; }
=======
        public Transform ShieldRoot { get { return transform.parent; } }
        public Vector3 Direction { get { return ShieldRoot.right; } }

        private bool isModulesGetted = false;


        protected void GetAnotherModules()
        {
            if (isModulesGetted) return;
            var modules = ShieldRoot.GetComponentsInChildren<ShieldModule>();
            foreach(var module in modules) OnModuleGetted(module);
            isModulesGetted = true;
        }

        protected virtual void OnModuleGetted(ShieldModule module) { }
>>>>>>> master
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    public class ModuleReflectionDefault : ModuleReflectionBase
    {
        public override void OnBullet(Bullet bullet, Collision2D collision)
        {
            bullet.OnContact();

            OnAnyBullet();
        }
    }
<<<<<<< HEAD:Assets/Scripts/HPBar.cs

}
=======
}
>>>>>>> Stalin:Assets/Scripts/Shields/Reflection Modules/ModuleReflectionDefault.cs

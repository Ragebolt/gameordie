using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shields.Modules
{
    /// <summary>
    /// Предоставляет событие создания заряда для специальной способности
    /// </summary>
    public interface ISpecialChargeProvider
    {
        event System.Action<float> OnChargeGenerated;
    }
}
using UnityEngine;
using ElectricDrill.SimpleRpgCore.Health;
using ElectricDrill.SimpleRpgCore;

namespace ElectricDrill.SimpleRpgCore.Events
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "PreHeal Game Event", menuName = "Simple RPG/Events/Generated/PreHeal")]
    public class PreHealGameEvent : GameEventGeneric2<PreHealInfo, EntityCore>
    {
    }
}
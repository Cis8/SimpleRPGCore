using UnityEngine;
using ElectricDrill.SimpleRpgCore;

namespace ElectricDrill.SimpleRpgCore.Events
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "Healed Game Event", menuName = "Simple RPG/Events/Generated/Healed")]
    public class HealedGameEvent : GameEventGeneric2<EntityHealth, long>
    {
    }
}
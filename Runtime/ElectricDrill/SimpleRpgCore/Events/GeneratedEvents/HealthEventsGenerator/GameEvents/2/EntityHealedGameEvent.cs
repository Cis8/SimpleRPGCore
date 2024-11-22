using UnityEngine;
using ElectricDrill.SimpleRpgCore;

namespace ElectricDrill.SimpleRpgCore.Events
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "EntityHealed Game Event", menuName = "Simple RPG/Events/Generated/EntityHealed")]
    public class EntityHealedGameEvent : GameEventGeneric2<EntityHealth, long>
    {
    }
}
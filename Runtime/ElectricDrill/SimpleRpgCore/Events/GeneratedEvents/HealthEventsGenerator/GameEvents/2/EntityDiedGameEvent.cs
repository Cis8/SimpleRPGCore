using UnityEngine;
using ElectricDrill.SimpleRpgCore;
using ElectricDrill.SimpleRpgCore.Health;

namespace ElectricDrill.SimpleRpgCore.Events
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "EntityDied Game Event", menuName = "Simple RPG/Events/Generated/EntityDied")]
    public class EntityDiedGameEvent : GameEventGeneric2<EntityHealth, TakenDmgInfo>
    {
    }
}
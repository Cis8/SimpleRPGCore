using UnityEngine;
using ElectricDrill.SimpleRpgCore.Health;

namespace ElectricDrill.SimpleRpgCore.Events
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "EntityHealed Game Event", menuName = "Simple RPG/Events/Generated/EntityHealed")]
    public class EntityHealedGameEvent : GameEventGeneric1<ReceivedHealInfo>
    {
    }
}
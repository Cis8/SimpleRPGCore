using UnityEngine;
using ElectricDrill.SimpleRpgCore;

namespace ElectricDrill.SimpleRpgCore.Events
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "LostHealth Game Event", menuName = "Simple RPG/Events/Generated/LostHealth")]
    public class LostHealthGameEvent : GameEventGeneric2<EntityHealth, long>
    {
    }
}
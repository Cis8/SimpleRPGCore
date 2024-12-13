using UnityEngine;
using ElectricDrill;

namespace ElectricDrill.SimpleRpgCore.Events
{
    /// <summary>
    /// The stat that changed, the stat's previous value, and the stat's new value
    /// </summary>
    [CreateAssetMenu(fileName = "StatChanged Game Event", menuName = "Simple RPG Core/Events/Generated/StatChanged")]
    public class StatChangedGameEvent : GameEventGeneric1<StatChangeInfo>
    {
    }
}
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    [CreateAssetMenu(fileName = "New Stats Scaling Component", menuName = "Simple RPG/Scaling/Stats Component")]
    public class StatsScalingComponent : SoSetScalingComponentBase<StatSet, Stat>
    {
        protected override StatSet GetEntitySet(EntityCore entity) => entity.Stats.StatSet;

        protected override long GetEntityValue(EntityCore entity, Stat key) => entity.Stats.Get(key);
        protected override IEnumerable<Stat> GetSetItems() => _set.Stats;
    }
}
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    [CreateAssetMenu(fileName = "New Stats Scaling Component", menuName = "Simple RPG Core/Scaling/Stats Component")]
    public class StatsScalingComponent : SoSetScalingComponentBase<StatSet, Stat>
    {
        protected override StatSet GetEntitySet(EntityCore entity) => entity.Stats.StatSet;

        protected override long GetEntityValue(EntityCore entity, Stat key) => entity.Stats.Get(key);
        protected override IEnumerable<Stat> GetSetItems() => _set.Stats;
        
#if UNITY_EDITOR
        protected override void OnEnable() {
            base.OnEnable();
            Stat.OnStatDeleted += HandleStatDeleted;
        }

        protected override void OnDisable() {
            base.OnDisable();
            Stat.OnStatDeleted -= HandleStatDeleted;
        }

        private void HandleStatDeleted(Stat deletedStat) {
            if (_scalingAttributeValues.Keys.Contains(deletedStat)) {
                _scalingAttributeValues.Remove(deletedStat);
            }
        }
#endif
    }
}
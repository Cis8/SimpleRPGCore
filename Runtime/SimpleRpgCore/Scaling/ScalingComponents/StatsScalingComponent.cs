using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    public class StatsScalingComponent : SoSetScalingComponentBase<StatSet, Stat>
    {
        protected override StatSet GetEntitySet(IEntityCore entity) => entity.Stats.StatSet;

        protected override long GetEntityValue(IEntityCore entity, Stat key) => entity.Stats.Get(key);
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
    
    public static class StatScalingComponentMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Scaling/Stat Scaling Component %&S", false, 2)]
        public static void CreateStatScalingComponent()
        {
            var asset = ScriptableObject.CreateInstance<StatsScalingComponent>();
            ProjectWindowUtil.CreateAsset(asset, "New Stat Scaling Component.asset");
        }
    }
}
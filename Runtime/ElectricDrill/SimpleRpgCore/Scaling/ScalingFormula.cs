using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace ElectricDrill.SimpleRpgCore.Scaling {
    [CreateAssetMenu(fileName = "New Scaling Formula", menuName = "Simple RPG/Scaling/Scaling Formula")]
    public class ScalingFormula : ScriptableObject
    {
        [SerializeField] private bool useScalingBaseValue;
        [SerializeField] private GrowthFormula scalingBaseValue;
        [SerializeField] private long fixedBaseValue;
        [SerializeField] private List<ScalingComponent> selfScalingComponents;
        [SerializeField] private List<ScalingComponent> targetScalingComponents;
        
        public long CalculateValue(EntityCore entity) {
            Assert.IsTrue(targetScalingComponents.Count == 0, "This formula requires a target entity to calculate the value");
            return CalculateBaseValue() +
                   selfScalingComponents.Sum(component => component.CalculateValue(entity));
        }
        
        public long CalculateValue(EntityCore self, int level) {
            Assert.IsTrue(targetScalingComponents.Count == 0, "This formula requires a target entity to calculate the value");
            return CalculateBaseValue(level) +
                   selfScalingComponents.Sum(component => component.CalculateValue(self));
        }
        
        public long CalculateValue(EntityCore self, EntityCore target) {
            return CalculateBaseValue() +
                   selfScalingComponents.Sum(component => component.CalculateValue(self)) +
                   targetScalingComponents.Sum(component => component.CalculateValue(target));
        }
        
        public long CalculateValue(EntityCore self, EntityCore target, int level) {
            return CalculateBaseValue(level) +
                   selfScalingComponents.Sum(component => component.CalculateValue(self)) +
                   targetScalingComponents.Sum(component => component.CalculateValue(target));
        }

        private long CalculateBaseValue(int level = 0) {
            Assert.IsTrue(!useScalingBaseValue || scalingBaseValue != null,
                "Scaling base value growth formula shall be set when Use Scaling Base Value is set true");
            Assert.IsTrue(!useScalingBaseValue || level > 0,
                "Level shall be greater than 0 when Use Scaling Base Value is set true");
            return useScalingBaseValue ? scalingBaseValue.GetGrowthValue(level) : fixedBaseValue;
        }
    }
}

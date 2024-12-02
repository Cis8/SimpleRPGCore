using System;
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
        
        // though for being updated at runtime, these lists are not serialized. The Reset method should be called to
        // erase the temporary scaling components
        private readonly List<ScalingComponent> _tempSelfScalingComponents = new();
        private readonly List<ScalingComponent> _tempTargetScalingComponents = new();

        public List<ScalingComponent> TempSelfScalingComponents => _tempSelfScalingComponents;
        public List<ScalingComponent> TempTargetScalingComponents => _tempTargetScalingComponents;


        public long CalculateValue(EntityCore self) {
            Assert.IsTrue(targetScalingComponents.Count == 0, "This formula requires a target entity to calculate the value");
            return CalculateBaseValue() +
                   CalculateSelfScalingComponents(self);
        }
        
        public long CalculateValue(EntityCore self, int level) {
            Assert.IsTrue(targetScalingComponents.Count == 0, "This formula requires a target entity to calculate the value");
            Assert.IsTrue(useScalingBaseValue, "This formula does not require a level to calculate the value");
            return CalculateBaseValue(level) +
                   CalculateSelfScalingComponents(self);
        }
        
        public long CalculateValue(EntityCore self, EntityCore target) {
            return CalculateBaseValue() +
                   CalculateSelfScalingComponents(self) +
                   CalculateTargetScalingComponents(target);
        }
        
        public long CalculateValue(EntityCore self, EntityCore target, int level) {
            Assert.IsTrue(useScalingBaseValue, "This formula does not require a level to calculate the value");
            return CalculateBaseValue(level) +
                   CalculateSelfScalingComponents(self) +
                   CalculateTargetScalingComponents(target);
        }

        public void ResetTmpScalings() {
            _tempSelfScalingComponents.Clear();
            _tempTargetScalingComponents.Clear();
        }

        private long CalculateBaseValue(int level = 0) {
            Assert.IsTrue(!useScalingBaseValue || scalingBaseValue != null,
                "Scaling base value growth formula shall be set when Use Scaling Base Value is set true");
            Assert.IsTrue(!useScalingBaseValue || level > 0,
                "Level shall be greater than 0 when Use Scaling Base Value is set true");
            return useScalingBaseValue ? scalingBaseValue.GetGrowthValue(level) : fixedBaseValue;
        }
        
        private long CalculateSelfScalingComponents(EntityCore self) {
            return selfScalingComponents.Sum(component => component.CalculateValue(self)) +
                   _tempSelfScalingComponents.Sum(component => component.CalculateValue(self));
        }
        
        private long CalculateTargetScalingComponents(EntityCore target) {
            return targetScalingComponents.Sum(component => component.CalculateValue(target)) +
                   _tempTargetScalingComponents.Sum(component => component.CalculateValue(target));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace ElectricDrill.SimpleRpgCore.Scaling {
    public class ScalingFormula : ScriptableObject
    {
        [SerializeField] private bool useScalingBaseValue;
        [SerializeField] private GrowthFormula scalingBaseValue;
        [SerializeField] private long fixedBaseValue;
        [SerializeField] private List<ScalingComponent> selfScalingComponents;
        [SerializeField] private List<ScalingComponent> targetScalingComponents;
        
        // thought for being updated at runtime, these lists are not serialized. The Reset method should be called to
        // erase the temporary scaling components
        private readonly List<ScalingComponent> _tmpSelfScalingComponents = new();
        private readonly List<ScalingComponent> _tmpTargetScalingComponents = new();

        public List<ScalingComponent> TmpSelfScalingComponents => _tmpSelfScalingComponents;
        public List<ScalingComponent> TmpTargetScalingComponents => _tmpTargetScalingComponents;


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
            _tmpSelfScalingComponents.Clear();
            _tmpTargetScalingComponents.Clear();
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
                   _tmpSelfScalingComponents.Sum(component => component.CalculateValue(self));
        }
        
        private long CalculateTargetScalingComponents(EntityCore target) {
            return targetScalingComponents.Sum(component => component.CalculateValue(target)) +
                   _tmpTargetScalingComponents.Sum(component => component.CalculateValue(target));
        }
    }
    
    public static class ScalingFormulaMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Scaling/Scaling Formula #S", false, 0)]
        public static void CreateScalingFormula()
        {
            var asset = ScriptableObject.CreateInstance<ScalingFormula>();
            ProjectWindowUtil.CreateAsset(asset, "New Scaling Formula.asset");
        }
    }
}

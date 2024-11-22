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
        [SerializeField] private long _baseValue;
        [SerializeField] private List<ScalingComponent> _selfScalingComponents;
        [SerializeField] private List<ScalingComponent> _targetScalingComponents;
        
        public long CalculateValue(EntityCore entity) {
            Assert.IsTrue(_targetScalingComponents.Count == 0, "This formula requires a target entity to calculate the value");
            return _baseValue +
                   _selfScalingComponents.Sum(component => component.CalculateValue(entity));
        }
        
        public long CalculateValue(EntityCore self, EntityCore target) {
            return _baseValue +
                   _selfScalingComponents.Sum(component => component.CalculateValue(self)) +
                   _targetScalingComponents.Sum(component => component.CalculateValue(target));
        }
    }
}

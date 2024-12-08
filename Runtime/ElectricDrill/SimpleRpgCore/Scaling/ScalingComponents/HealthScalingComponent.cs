using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling {
    [CreateAssetMenu(fileName = "New Health Scaling Component", menuName = "Simple RPG/Scaling/Health Component")]
    public class HealthScalingComponent : ScalingComponent
    {
        private enum HealthScalingAttributes
        {
            HP,
            MAX_HP,
            MISSING_HP
        }
        
        [SerializeField] private List<HealthScalingAttributeValue> _scalingAttributeValues = new();
        
        public override long CalculateValue(EntityCore entity) {
            long value = 0;
            if (entity.TryGetComponent<EntityHealth>(out var health))
            {
                foreach (var attributeMapping in _scalingAttributeValues) {
                    value += (long)Math.Round(attributeMapping.Attribute switch {
                        HealthScalingAttributes.HP => health.Hp * attributeMapping.Value,
                        HealthScalingAttributes.MAX_HP => health.MaxHp * attributeMapping.Value,
                        HealthScalingAttributes.MISSING_HP => (health.MaxHp - health.Hp) * attributeMapping.Value,
                        _ => throw new ArgumentOutOfRangeException()
                    });
                }
            }
            else {
                Debug.LogWarning("Entity does not have a Health component");
            }
            
            return value;
        }

        [Serializable]
        struct HealthScalingAttributeValue
        {
            public HealthScalingAttributes Attribute;
            public float Value;
        }
    }
}

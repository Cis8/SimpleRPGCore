using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Attributes;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    [CreateAssetMenu(fileName = "New Attributes Scaling Component", menuName = "Simple RPG/Scaling/Attributes Component")]
    public class AttributesScalingComponent : SoSetScalingComponentBase<AttributeSet, Attribute>
    {
        protected override AttributeSet GetEntitySet(EntityCore entity) => entity.Attributes.AttributeSet;

        protected override long GetEntityValue(EntityCore entity, Attribute key) => entity.Attributes.Get(key);
        
        protected override IEnumerable<Attribute> GetSetItems() => _set.Attributes;
        
#if UNITY_EDITOR
        protected override void OnEnable() {
            Attribute.OnAttributeDeleted += HandleAttributeDeleted;
        }

        protected override void OnDisable() {
            Attribute.OnAttributeDeleted -= HandleAttributeDeleted;
        }

        private void HandleAttributeDeleted(Attribute deletedAttribute) {
            if (_scalingAttributeValues.Keys.Contains(deletedAttribute)) {
                _scalingAttributeValues.Remove(deletedAttribute);
            }
        }
#endif
    }
}
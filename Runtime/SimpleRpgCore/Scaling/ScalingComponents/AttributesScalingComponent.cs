using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Attributes;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    public class AttributesScalingComponent : SoSetScalingComponentBase<AttributeSet, Attribute>
    {
        protected override AttributeSet GetEntitySet(IEntityCore entity) => entity.Attributes.AttributeSet;

        protected override long GetEntityValue(IEntityCore entity, Attribute key) => entity.Attributes.Get(key);
        
        protected override IEnumerable<Attribute> GetSetItems() => _set.Attributes;
        
#if UNITY_EDITOR
        protected override void OnEnable() {
            base.OnEnable();
            Attribute.OnAttributeDeleted += HandleAttributeDeleted;
        }

        protected override void OnDisable() {
            base.OnDisable();
            Attribute.OnAttributeDeleted -= HandleAttributeDeleted;
        }

        private void HandleAttributeDeleted(Attribute deletedAttribute) {
            if (_scalingAttributeValues.Keys.Contains(deletedAttribute)) {
                _scalingAttributeValues.Remove(deletedAttribute);
            }
        }
#endif
    }
    
    public static class AttributeScalingComponentMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Scaling/Attribute Scaling Component %&A", false, 1)]
        public static void CreateAttributeScalingComponent()
        {
            var asset = ScriptableObject.CreateInstance<AttributesScalingComponent>();
            ProjectWindowUtil.CreateAsset(asset, "New Attribute Scaling Component.asset");
        }
    }
}
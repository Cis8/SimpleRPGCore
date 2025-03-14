using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Attributes
{
    public class AttributeSet : ScriptableObject, IAttributeContainer
    {
        [SerializeField] private SerializableHashSet<Attribute> _attributes = new();

        public IReadOnlyList<Attribute> Attributes => _attributes.ToList();

        public Attribute Get(Attribute attribute) {
            return _attributes.First(c => c == attribute);
        }

        public bool Contains(Attribute attribute) {
            return _attributes.Contains(attribute);
        }
        
        internal void SetAttributes(SerializableHashSet<Attribute> attributes) {
            _attributes = attributes;
        }
        
#if UNITY_EDITOR
        private void OnEnable() {
            Attribute.OnAttributeDeleted += HandleAttributeDeleted;
        }

        private void OnDisable() {
            Attribute.OnAttributeDeleted -= HandleAttributeDeleted;
        }

        private void HandleAttributeDeleted(Attribute deletedAttribute) {
            if (_attributes.Contains(deletedAttribute)) {
                _attributes.Remove(deletedAttribute);
            }
        }
#endif
    }
    
    public static class AttributeSetMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Attribute Set &A", false, 1)]
        public static void CreateAttributeSet()
        {
            var asset = ScriptableObject.CreateInstance<AttributeSet>();
            ProjectWindowUtil.CreateAsset(asset, "New Attribute Set.asset");
        }
    }
}
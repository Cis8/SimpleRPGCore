using System;
using System.Diagnostics.CodeAnalysis;
using ElectricDrill.SimpleRPGCore.SimpleRpgCore.Utils;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Attributes
{
    [RequireComponent(typeof(EntityCore))]
    public class EntityAttributes : MonoBehaviour
    {
        private EntityCore _entityCore;
        
        [SerializeField] private IntRef attrPointsPerLevel;
        [SerializeField, HideInInspector] AttributePointsTracker attrPointsTracker;


        [SerializeField, HideInInspector] internal bool useClassBaseAttributes;
        
        // dynamic attributes
        private EntityClass _entityClass;
        
        private AttributeSetInstance _flatModifiers;
        
        private AttributeSetInstance _percentageModifiers;
        
        // Fixed base attributes
        [SerializeField, HideInInspector] private AttributeSet fixedBaseAttributeSet;
        [SerializeField, HideInInspector] internal SerializableDictionary<Attribute, long> fixedBaseAttributes;
        
        // Cache
        private Cache<Attribute, long> _attributesCache = new();

        public Cache<Attribute, long> AttributesCache => _attributesCache;

        private EntityCore EntityCore {
            get {
                if (!_entityCore) {
                    _entityCore = GetComponent<EntityCore>();
                }
                return _entityCore;
            }
        }
        internal AttributePointsTracker AttrPointsTracker => attrPointsTracker;
        
        public void SpendOn(Attribute attribute, int amount) {
            attrPointsTracker.SpendOn(attribute, amount);
            AttributesCache.Invalidate(attribute);
        }
        
        public AttributeSet AttributeSet {
            get {
                // Assert that the AttributeSet is not null
                if (useClassBaseAttributes) {
                    if (!TryGetComponent(out _entityClass)) {
                        Debug.LogError(
                            $"EntityClass component must be attached to the {gameObject.name} GameObject if useClassBaseAttributes is set true");
                        return null;
                    }

                    if (_entityClass.Class.AttributeSet == null) {
                        Debug.LogError($"AttributeSet of the Class {_entityClass.Class} is null");
                        return null;
                    }
                }
                else {
                    if (fixedBaseAttributeSet == null) {
                        Debug.LogError($"FixedBaseAttributeAttrSet of {name}'s EntityAttributes is null");
                        return null;
                    }
                }
                
                // Return the AttributeSet
                return useClassBaseAttributes ? _entityClass.Class.AttributeSet : fixedBaseAttributeSet;
            }
        }

        protected AttributeSetInstance FlatModifiers => _flatModifiers ??= new AttributeSetInstance(AttributeSet);
        
        protected AttributeSetInstance PercentageModifiers => _percentageModifiers ??= new AttributeSetInstance(AttributeSet);

        private void Awake() {

        }

        public long Get(Attribute attribute) {
            Assert.IsTrue(AttributeSet.Contains(attribute), $"Attribute {attribute} is not in the {name}'s AttributeSet ({AttributeSet.name})");
            if (AttributesCache.TryGet(attribute, out var value)) {
                return value;
            }
            else {
                var finalValue = attribute.Clamp(CalculateFinalAttribute(attribute));
                AttributesCache[attribute] = finalValue;
                return finalValue;
            }
        }
        
        private long CalculateFinalAttribute(Attribute attribute) {
            var attrValue = GetBase(attribute) + FlatModifiers[attribute];

            if (PercentageModifiers.Contains(attribute)) {
                // apply percentage modifiers
                var percentageModifier = (long)Math.Round(attrValue * PercentageModifiers.GetAsPercentage(attribute));
                attrValue += percentageModifier;
            }
            
            // Add spent points
            attrValue += attrPointsTracker.GetSpentOn(attribute);
            
            return attrValue;
        }
        
        public void AddFlatModifier(Attribute attribute, long value) {
            FlatModifiers.AddValue(attribute, value);
            AttributesCache.Invalidate(attribute);
        }
        
        public void AddPercentageModifier(Attribute attribute, Percentage value) {
            PercentageModifiers.AddValue(attribute, (long)value);
            AttributesCache.Invalidate(attribute);
        }
        
        public long GetBase(Attribute attribute) {
            return GetBaseAt(attribute, EntityCore.Level);
        }

        private long GetBaseAt(Attribute attribute, int level) {
            Assert.IsTrue(AttributeSet.Contains(attribute), $"Attribute {attribute.name} is not in the {name}'s AttributeSet ({AttributeSet.name})");
            long baseValue = GetRawBaseAt(attribute, level);
            return attribute.Clamp(baseValue);
        }
        
        private long GetRawBaseAt(Attribute attribute, int level) {
            Assert.IsTrue(AttributeSet.Contains(attribute), $"Attribute {attribute.name} is not in the {name}'s AttributeSet ({AttributeSet.name})");            
            return useClassBaseAttributes ? _entityClass.Class.GetAttributeAt(attribute, level) : fixedBaseAttributes[attribute];
        }

        // EVENTS and EDITOR
        private void OnLevelUp(int _) {
            AttributesCache.InvalidateAll();
            attrPointsTracker.AddPoints(attrPointsPerLevel);
        }
        
        internal void OnValidate() {
#if UNITY_EDITOR
            // -1 since we want to add them starting from level 2
            attrPointsTracker.Init(attrPointsPerLevel * (GetComponent<EntityCore>().Level - 1));
            attrPointsTracker.Validate();
            
            lock (fixedBaseAttributes) {
                InitializationUtils.RefreshInspectorReservedValues(
                    ref fixedBaseAttributes.inspectorReservedPairs,
                    fixedBaseAttributeSet?.Attributes);
                fixedBaseAttributes.OnAfterDeserialize();
            }

            lock (attrPointsTracker.SpentAttributePoints.inspectorReservedPairs) {
                InitializationUtils.RefreshInspectorReservedValues(
                    ref attrPointsTracker.SpentAttributePoints.inspectorReservedPairs,
                    AttributeSet?.Attributes);
                attrPointsTracker.SpentAttributePoints.OnAfterDeserialize();
            }
            
            AttributesCache.InvalidateAll();
#endif
        }

        private void OnEnable() {
            EntityCore.Level.OnLevelUp += OnLevelUp;
            // this is required as the entity may have levelled up while being disabled
            AttributesCache.InvalidateAll();
#if UNITY_EDITOR
            OnValidate();
#endif
        }
        
        private void OnDisable() {
            EntityCore.Level.OnLevelUp -= OnLevelUp;
            AttributesCache.InvalidateAll();
#if UNITY_EDITOR
            OnValidate();
#endif
        }
    }
}
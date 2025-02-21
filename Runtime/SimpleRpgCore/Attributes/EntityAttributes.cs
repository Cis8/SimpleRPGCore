using System;
using System.Diagnostics.CodeAnalysis;
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
        
        protected AttributeSetInstance _flatModifiers;
        
        protected AttributeSetInstance _percentageModifiers;
        
        // Fixed base attributes
        [SerializeField, HideInInspector] private AttributeSet fixedBaseAttributeSet;
        [SerializeField, HideInInspector] internal SerializableDictionary<Attribute, long> fixedBaseAttributes;
        
        private EntityCore EntityCore {
            get {
                if (!_entityCore) {
                    _entityCore = GetComponent<EntityCore>();
                }
                return _entityCore;
            }
        }
        public AttributePointsTracker AttrPointsTracker => attrPointsTracker;
        
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

        protected AttributeSetInstance FlatModifiers {
            get {
                InitializeAttributeFlatModifiersIfNull();
                return FlatModifiers;
            }
        }
        
        protected AttributeSetInstance PercentageModifiers {
            get {
                InitializeAttributePercentageModifiersIfNull();
                return PercentageModifiers;
            }
        }

        private void Awake() {
            InitializeAttributeFlatModifiersIfNull();
            InitializeAttributePercentageModifiersIfNull();
        }

        public long Get(Attribute attribute) {
            Assert.IsTrue(AttributeSet.Contains(attribute), $"Attribute {attribute} is not in the {name}'s AttributeSet ({AttributeSet.name})");
            return attribute.Clamp(CalculateFinalAttribute(attribute));
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
        }
        
        public void AddPercentageModifier(Attribute attribute, Percentage value) {
            PercentageModifiers.AddValue(attribute, (long)value);
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
            attrPointsTracker.AddPoints(attrPointsPerLevel);
        }
        
        private void OnValidate() {
#if UNITY_EDITOR
            attrPointsTracker.Init(attrPointsPerLevel * GetComponent<EntityCore>().Level);
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
#endif
        }

        private void OnEnable() {
            EntityCore.Level.OnLevelUp += OnLevelUp;
#if UNITY_EDITOR
            OnValidate();
            Selection.selectionChanged += OnSelectionChanged;
#endif
        }
        
        private void OnDisable() {
            EntityCore.Level.OnLevelUp -= OnLevelUp;
#if UNITY_EDITOR
            OnValidate();
            Selection.selectionChanged -= OnSelectionChanged;
#endif
        }
        
        // UTILS
#if UNITY_EDITOR
        static EntityAttributes()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }
        
        private static void OnSelectionChanged() {
            if (Selection.activeObject is GameObject selectedObject && selectedObject.TryGetComponent<EntityAttributes>(out var entityAttributes)) {
                entityAttributes.OnValidate();
            }
        }
#endif
        
        internal void InitializeAttributeFlatModifiersIfNull() {
            _flatModifiers ??= new AttributeSetInstance(AttributeSet);
        }
        
        internal void InitializeAttributePercentageModifiersIfNull() {
            _percentageModifiers ??= new AttributeSetInstance(AttributeSet);
        }
    }
}
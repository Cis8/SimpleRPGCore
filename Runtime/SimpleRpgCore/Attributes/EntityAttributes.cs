using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

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

        private void Awake() {
        }

        public long Get(Attribute attribute) {
            Assert.IsTrue(AttributeSet.Contains(attribute), $"Attribute {attribute} is not in the {name}'s AttributeSet ({AttributeSet.name})");
            long finalValue = 0;
            if (useClassBaseAttributes) {
                finalValue += _entityClass.Class.GetAttributeAt(attribute, EntityCore.Level);
            }
            else {
                finalValue += fixedBaseAttributes[attribute];
            }
            // Add spent points
            finalValue += attrPointsTracker.GetSpentOn(attribute);

            return finalValue;
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
    }
}
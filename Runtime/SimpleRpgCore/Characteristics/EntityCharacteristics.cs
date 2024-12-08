using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [RequireComponent(typeof(EntityCore))]
    public class EntityCharacteristics : MonoBehaviour
    {
        private EntityCore _entityCore;
        
        [SerializeField] private IntRef charPointsPerLevel;
        [SerializeField, HideInInspector] CharacteristicPointsTracker charPointsTracker;


        [SerializeField, HideInInspector] internal bool useClassBaseCharacteristics;
        
        // dynamic characteristics
        private EntityClass _entityClass;
        
        // Fixed base characteristics
        [SerializeField, HideInInspector] private CharacteristicSet fixedBaseCharacteristicCharSet;
        [SerializeField, HideInInspector] internal SerializableDictionary<Characteristic, long> fixedBaseCharacteristics;
        
        public CharacteristicPointsTracker CharPointsTracker => charPointsTracker;
        
        public CharacteristicSet CharacteristicSet {
            get {
                // Assert that the CharacteristicSet is not null
                if (useClassBaseCharacteristics) {
                    if (!TryGetComponent(out _entityClass)) {
                        Debug.LogError(
                            $"EntityClass component must be attached to the {gameObject.name} GameObject if useClassBaseCharacteristics is set true");
                        return null;
                    }

                    if (_entityClass.Class.CharacteristicSet == null) {
                        Debug.LogError($"CharacteristicSet of the Class {_entityClass.Class} is null");
                        return null;
                    }
                }
                else {
                    if (fixedBaseCharacteristicCharSet == null) {
                        Debug.LogError($"FixedBaseCharacteristicCharSet of {name}'s EntityCharacteristics is null");
                        return null;
                    }
                }
                
                // Return the CharacteristicSet
                return useClassBaseCharacteristics ? _entityClass.Class.CharacteristicSet : fixedBaseCharacteristicCharSet;
            }
        }

        public long Get(Characteristic characteristic) {
            Assert.IsTrue(CharacteristicSet.Contains(characteristic), $"Characteristic {characteristic} is not in the {name}'s CharacteristicSet ({CharacteristicSet.name})");
            long finalValue = 0;
            if (useClassBaseCharacteristics) {
                finalValue += _entityClass.Class.GetCharacteristicAt(characteristic, _entityCore.Level);
            }
            else {
                finalValue += fixedBaseCharacteristics[characteristic];
            }
            // Add spent points
            finalValue += charPointsTracker.GetSpentOn(characteristic);

            return finalValue;
        }

        // EVENTS and EDITOR
        private void OnLevelUp(int _) {
            charPointsTracker.AddPoints(charPointsPerLevel);
        }
        
        private void OnValidate() {
#if UNITY_EDITOR
            charPointsTracker.Init(charPointsPerLevel * GetComponent<EntityCore>().Level);
            charPointsTracker.Validate();
            
            lock (fixedBaseCharacteristics) {
                InitializationUtils.RefreshInspectorReservedValues(
                    ref fixedBaseCharacteristics.inspectorReservedPairs,
                    fixedBaseCharacteristicCharSet?.Characteristics);
                fixedBaseCharacteristics.OnAfterDeserialize();
            }

            lock (charPointsTracker.SpentCharacteristicPoints.inspectorReservedPairs) {
                InitializationUtils.RefreshInspectorReservedValues(
                    ref charPointsTracker.SpentCharacteristicPoints.inspectorReservedPairs,
                    CharacteristicSet?.Characteristics);
                charPointsTracker.SpentCharacteristicPoints.OnAfterDeserialize();
            }
#endif
        }

        private void OnEnable() {
            _entityCore = GetComponent<EntityCore>();
            _entityCore.Level.OnLevelUp += OnLevelUp;
#if UNITY_EDITOR
            OnValidate();
            Selection.selectionChanged += OnSelectionChanged;
#endif
        }
        
        private void OnDisable() {
            _entityCore.Level.OnLevelUp -= OnLevelUp;
#if UNITY_EDITOR
            OnValidate();
            Selection.selectionChanged -= OnSelectionChanged;
#endif
        }
        
        // UTILS
#if UNITY_EDITOR
        static EntityCharacteristics()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }
        
        private static void OnSelectionChanged() {
            if (Selection.activeObject is GameObject selectedObject && selectedObject.TryGetComponent<EntityCharacteristics>(out var entityCharacteristics)) {
                entityCharacteristics.OnValidate();
            }
        }
#endif
    }
}
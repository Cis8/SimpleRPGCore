using System;
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
        // todo add check to stabilize the value of available points considering the spent points, the points per level, and the level
        [SerializeField] CharacteristicPointsTracker charPointsTracker;


        [SerializeField] private bool useClassBaseCharacteristics;
        
        // dynamic characteristics
        private EntityClass _entityClass;
        private CharacteristicSetInstance _classCharacteristics;
        
        // Fixed base characteristics
        [SerializeField] private CharacteristicSet fixedBaseCharacteristicCharSet;
        [SerializeField] private SerializableDictionary<Characteristic, long> fixedBaseCharacteristics;

        public CharacteristicPointsTracker CharPointsTracker => charPointsTracker;
        
        public CharacteristicSet CharacteristicSet {
            get {
                // Assert that the CharacteristicSet is not null
                if (useClassBaseCharacteristics) {
                    CheckInitializeClassBaseCharacteristics();
                    if (_entityClass == null) {
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
                finalValue += _classCharacteristics[characteristic];
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
            lock (fixedBaseCharacteristics) {
                InitializationUtils.RefreshInspectorReservedValues(
                    ref fixedBaseCharacteristics.inspectorReservedPairs,
                    fixedBaseCharacteristicCharSet?.Characteristics);
                fixedBaseCharacteristics.OnAfterDeserialize();
            }

            lock (charPointsTracker.SpentCharacteristicPoints.inspectorReservedPairs) {
                InitializationUtils.RefreshInspectorReservedValues(ref charPointsTracker.SpentCharacteristicPoints.inspectorReservedPairs, CharacteristicSet?.Characteristics);
                charPointsTracker.SpentCharacteristicPoints.OnAfterDeserialize();
            }
#endif
        }

        private void OnEnable() {
            _entityCore = GetComponent<EntityCore>();
            CheckInitializeClassBaseCharacteristics();
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
        private void OnSelectionChanged() {
            Debug.Log($"Selection changed, active object: {Selection.activeObject}");
            if (Selection.activeObject == this) {
                OnValidate();
            }
        }
#endif

        private void CheckInitializeClassBaseCharacteristics() {
            CheckInitializeEntityCoreRef();
            if (useClassBaseCharacteristics && !_entityClass) {
                if (TryGetComponent(out _entityClass)) {
                    _classCharacteristics = _entityClass.Class.CreateCharacteristicSetInstanceAt(_entityCore.Level);
                }
                else {
                    Debug.LogError($"EntityClass component must be attached to the {gameObject.name} GameObject if useClassBaseCharacteristics is set false");
                }
            }
        }

        private void CheckInitializeEntityCoreRef() {
            if (!_entityCore) {
                _entityCore = GetComponent<EntityCore>();
            }
        }
    }
}
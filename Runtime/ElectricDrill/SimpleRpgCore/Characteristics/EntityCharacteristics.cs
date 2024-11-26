using System;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [RequireComponent(typeof(EntityCore))]
    public class EntityCharacteristics : MonoBehaviour
    {
        private EntityCore _entityCore;
        
        [SerializeField] private bool useClassBaseCharacteristics;

        [SerializeField] private IntRef charPointsPerLevel;
        [SerializeField] AvailableCharacteristicPoints availableCharPoints;
        
        // dynamic characteristics
        private EntityClass _entityClass;
        private CharacteristicSetInstance _classCharacteristics;
        
        // Fixed base characteristics
        [SerializeField] private CharacteristicSet fixedBaseCharacteristicCharSet;
        [SerializeField] private List<SerKeyValPair<Characteristic, int>> inspectorReservedFixedBaseCharacteristics;
        private CharacteristicSetInstance _fixedBaseCharacteristics;

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
                finalValue += _fixedBaseCharacteristics[characteristic];
            }
            // Add spent points
            finalValue += availableCharPoints.SpentCharacteristicPoints[characteristic];

            return finalValue;
        }

        // EVENTS and EDITOR
        private void OnLevelUp(int _) {
            availableCharPoints.Add(charPointsPerLevel);
        }
        
        private void OnValidate() {
#if UNITY_EDITOR
            if (!useClassBaseCharacteristics) {
                InitializationUtils.RefreshInspectorReservedValues(ref inspectorReservedFixedBaseCharacteristics, fixedBaseCharacteristicCharSet?.Characteristics);
                InitializeFixedBaseCharacteristics();
            }
            
            /*InitializationUtils.RefreshInspectorReservedValues(ref availableCharPoints.inspectorReservedSpentCharacteristicPoints, CharacteristicSet?.Characteristics);
            availableCharPoints.InitializeSpentCharacteristicPoints(CharacteristicSet);*/
#endif
        }

        private void OnEnable() {
            _entityCore = GetComponent<EntityCore>();
            CheckInitializeClassBaseCharacteristics();
            _entityCore.Level.OnLevelUp += OnLevelUp;
            if (!useClassBaseCharacteristics)
                InitializeFixedBaseCharacteristics();
#if UNITY_EDITOR
            OnValidate();
#endif
        }
        
        private void OnDisable() {
            _entityCore.Level.OnLevelUp -= OnLevelUp;
#if UNITY_EDITOR
            OnValidate();
#endif
        }
        
        // UTILS

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

        private void InitializeFixedBaseCharacteristics() {
            if (!useClassBaseCharacteristics) {
                _fixedBaseCharacteristics = new CharacteristicSetInstance(fixedBaseCharacteristicCharSet);
                foreach (var statValuePair in inspectorReservedFixedBaseCharacteristics) {
                    _fixedBaseCharacteristics.AddValue(statValuePair.Key, statValuePair.Value);
                }
            }
        }
    }
}
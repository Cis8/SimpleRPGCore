using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Characteristics;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore
{
    [CreateAssetMenu(fileName = "New Class", menuName = "Simple RPG/Class")]
    public class Class : ScriptableObject
    {
        [SerializeField] protected GrowthFormula _maxHpGrowthFormula;

        [SerializeField] protected StatSet _statSet;
        
        // can be null
        [SerializeField] protected CharacteristicSet _characteristicSet;
        [SerializeField] private List<SerKeyValPair<Characteristic, GrowthFormula>> characteristicGrowthFormulas = new();

        // todo change and us List<RPGKeyValuePair<Stat, GrowthFormula>>
        [SerializeField] protected StatGrowthFnPair[] _statGrowthFnPairs = Array.Empty<StatGrowthFnPair>();
        
        public CharacteristicSet CharacteristicSet => _characteristicSet;
        public StatSet StatSet => _statSet;

        public long GetMaxHpAt(int level) {
            return _maxHpGrowthFormula.GetGrowthValue(level);
        }

        // function called on level-up to increase stats
        public StatSetInstance CreateStatSetInstanceAt(int level) {
            var statSetInstance = new StatSetInstance(_statSet);
            foreach (var statGrowthFnPair in _statGrowthFnPairs) {
                statSetInstance.AddValue(
                    statGrowthFnPair.Stat,
                    statGrowthFnPair.growthFormula.GetGrowthValue(level));
            }

            return statSetInstance;
        }
        
        public CharacteristicSetInstance CreateCharacteristicSetInstanceAt(int level) {
            var characteristicSetInstance = new CharacteristicSetInstance(_characteristicSet);
            foreach (var characteristicGrowthFormula in characteristicGrowthFormulas) {
                characteristicSetInstance.AddValue(
                    characteristicGrowthFormula.Key,
                    characteristicGrowthFormula.Value.GetGrowthValue(level));
            }

            return characteristicSetInstance;
        }

        [Serializable]
        public struct StatGrowthFnPair
        {
            public Stat Stat;
            public GrowthFormula growthFormula;
        }
        
        // EDITOR
        private void OnValidate() {
            if (_statSet != null) {
                _statGrowthFnPairs = _statSet.Stats.Select(stat => {
                    if (_statGrowthFnPairs == null) {
                        return new StatGrowthFnPair {
                            Stat = stat,
                            growthFormula = null
                        };
                    }
                    else {
                        var existingStat = _statGrowthFnPairs.FirstOrDefault(s => s.Stat.name == stat.name);
                        if (existingStat.Stat == null) {
                            return new StatGrowthFnPair {
                                Stat = stat,
                                growthFormula = null
                            };
                        }
                        return existingStat;
                    }
                }).ToArray();
            }
            else {
                _statGrowthFnPairs = Array.Empty<StatGrowthFnPair>();
            }
            
            if (_characteristicSet != null) {
                characteristicGrowthFormulas = _characteristicSet.Characteristics.Select(characteristic => {
                    if (characteristicGrowthFormulas == null) {
                        return new SerKeyValPair<Characteristic, GrowthFormula>(characteristic, null);
                    }
                    else {
                        var existingCharacteristic = characteristicGrowthFormulas.FirstOrDefault(s => s.Key.name == characteristic.name);
                        if (existingCharacteristic.Key == null) {
                            return new SerKeyValPair<Characteristic, GrowthFormula>(characteristic, null);
                        }
                        return existingCharacteristic;
                    }
                }).ToList();
            }
            else {
                characteristicGrowthFormulas = new List<SerKeyValPair<Characteristic, GrowthFormula>>();
            }
        }

#if UNITY_EDITOR
        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable() {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged() {
            if (Selection.activeObject == this) {
                OnValidate();
            }
        }
#endif
    }
}
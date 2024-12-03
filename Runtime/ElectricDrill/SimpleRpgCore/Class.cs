using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Characteristics;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using NUnit.Framework;
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
        
        // can be null
        [SerializeField] protected CharacteristicSet _characteristicSet;
        [SerializeField] private SerializableDictionary<Characteristic, GrowthFormula> characteristicGrowthFormulas = new();

        // todo change and us List<RPGKeyValuePair<Stat, GrowthFormula>>
        [SerializeField] protected StatSet _statSet;
        [SerializeField] protected SerializableDictionary<Stat, GrowthFormula> _statGrowthFunctions = new();
        
        public CharacteristicSet CharacteristicSet => _characteristicSet;
        public StatSet StatSet => _statSet;

        public long GetMaxHpAt(int level) {
            return _maxHpGrowthFormula.GetGrowthValue(level);
        }

        public long GetCharacteristicAt(Characteristic characteristic, int level) {
            Assert.IsNotNull(characteristicGrowthFormulas[characteristic], $"Growth formula for {characteristic.name} is null");
            return characteristicGrowthFormulas[characteristic].GetGrowthValue(level);
        }
        
        public long GetStatAt(Stat stat, int level) {
            return _statGrowthFunctions.First(s => s.Key == stat).Value.GetGrowthValue(level);
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
                _statGrowthFunctions = _statSet.Stats
                    .Where(stat => stat)
                    .Select(stat => {
                    if (_statGrowthFunctions == null) {
                        return new KeyValuePair<Stat, GrowthFormula>(stat, null);
                    }
                    else {
                        var existingStat = _statGrowthFunctions.FirstOrDefault(s => s.Key.name == stat.name);
                        if (existingStat.Key == null) {
                            return new KeyValuePair<Stat, GrowthFormula>(stat, null);
                        }
                        return existingStat;
                    }
                    }).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            else {
                _statGrowthFunctions = new();
            }
            
            if (_characteristicSet != null) {
                characteristicGrowthFormulas = _characteristicSet.Characteristics.Select(characteristic => {
                    if (characteristicGrowthFormulas == null) {
                        return new KeyValuePair<Characteristic, GrowthFormula>(characteristic, null);
                    }
                    else {
                        var existingCharacteristic = characteristicGrowthFormulas.FirstOrDefault(s => s.Key.name == characteristic.name);
                        if (existingCharacteristic.Key == null) {
                            return new KeyValuePair<Characteristic, GrowthFormula>(characteristic, null);
                        }
                        return existingCharacteristic;
                    }
                }).ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value);
            }
            else {
                characteristicGrowthFormulas = new();
            }
        }

#if UNITY_EDITOR
        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChanged;
            Stat.OnStatDeleted += HandleStatDeleted;
        }

        private void OnDisable() {
            Selection.selectionChanged -= OnSelectionChanged;
            Stat.OnStatDeleted -= HandleStatDeleted;
        }

        private void OnSelectionChanged() {
            if (Selection.activeObject == this) {
                OnValidate();
            }
        }

        private void HandleStatDeleted(Stat deletedStat) {
            if (_statGrowthFunctions.Keys.Contains(deletedStat)) {
                _statGrowthFunctions.Remove(deletedStat);
            }
        }
#endif
    }
}
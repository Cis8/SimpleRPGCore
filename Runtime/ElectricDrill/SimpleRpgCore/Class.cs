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
        [SerializeField, HideInInspector] internal SerializableDictionary<Characteristic, GrowthFormula> characteristicGrowthFormulas = new();

        // todo change and us List<RPGKeyValuePair<Stat, GrowthFormula>>
        [SerializeField] protected StatSet _statSet;
        [SerializeField, HideInInspector] internal SerializableDictionary<Stat, GrowthFormula> _statGrowthFormulas = new();
        
        public CharacteristicSet CharacteristicSet { get => _characteristicSet; internal set => _characteristicSet = value; }
        public StatSet StatSet { get => _statSet; internal set => _statSet = value; }

        public long GetMaxHpAt(int level) {
            return _maxHpGrowthFormula.GetGrowthValue(level);
        }

        public long GetCharacteristicAt(Characteristic characteristic, int level) {
            Assert.IsNotNull(characteristicGrowthFormulas[characteristic], $"Growth formula for {characteristic.name} is null");
            return characteristicGrowthFormulas[characteristic].GetGrowthValue(level);
        }
        
        public long GetStatAt(Stat stat, int level) {
            return _statGrowthFormulas.First(s => s.Key == stat).Value.GetGrowthValue(level);
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
                _statGrowthFormulas = _statSet.Stats
                    .Where(stat => stat)
                    .Select(stat => {
                    if (_statGrowthFormulas == null) {
                        return new KeyValuePair<Stat, GrowthFormula>(stat, null);
                    }
                    else {
                        var existingStat = _statGrowthFormulas.FirstOrDefault(s => s.Key.name == stat.name);
                        if (existingStat.Key == null) {
                            return new KeyValuePair<Stat, GrowthFormula>(stat, null);
                        }
                        return existingStat;
                    }
                    }).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            else {
                _statGrowthFormulas = new();
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
            Characteristic.OnCharacteristicDeleted += HandleCharacteristicDeleted;
        }

        private void OnDisable() {
            Selection.selectionChanged -= OnSelectionChanged;
            Stat.OnStatDeleted -= HandleStatDeleted;
            Characteristic.OnCharacteristicDeleted -= HandleCharacteristicDeleted;
        }

        private void OnSelectionChanged() {
            if (Selection.activeObject == this) {
                OnValidate();
            }
        }

        private void HandleStatDeleted(Stat deletedStat) {
            if (_statGrowthFormulas.Keys.Contains(deletedStat)) {
                _statGrowthFormulas.Remove(deletedStat);
            }
        }
        
        private void HandleCharacteristicDeleted(Characteristic deletedCharacteristic) {
            if (characteristicGrowthFormulas.Keys.Contains(deletedCharacteristic)) {
                characteristicGrowthFormulas.Remove(deletedCharacteristic);
            }
        }
#endif
    }
}
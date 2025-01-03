using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.Serialization;
using Attribute = ElectricDrill.SimpleRpgCore.Attributes.Attribute;

namespace ElectricDrill.SimpleRpgCore
{
    [CreateAssetMenu(fileName = "New Class", menuName = "Simple RPG Core/Class")]
    public class Class : ScriptableObject, IStatSet
    {
        [SerializeField] protected GrowthFormula _maxHpGrowthFormula;
        
        // can be null
        [SerializeField] protected AttributeSet attributeSet;
        [SerializeField, HideInInspector] internal SerializableDictionary<Attribute, GrowthFormula> attributeGrowthFormulas = new();

        [SerializeField] protected StatSet _statSet;
        [SerializeField, HideInInspector] internal SerializableDictionary<Stat, GrowthFormula> _statGrowthFormulas = new();
        
        public AttributeSet AttributeSet { get => attributeSet; internal set => attributeSet = value; }
        public virtual StatSet StatSet { get => _statSet; internal set => _statSet = value; }

        public long GetMaxHpAt(int level) {
            return _maxHpGrowthFormula.GetGrowthValue(level);
        }

        public long GetAttributeAt(Attribute attribute, int level) {
            Assert.IsNotNull(attributeGrowthFormulas[attribute], $"Growth formula for {attribute.name} is null");
            return attributeGrowthFormulas[attribute].GetGrowthValue(level);
        }
        
        public virtual long GetStatAt(Stat stat, int level) {
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
            
            if (attributeSet != null) {
                attributeGrowthFormulas = attributeSet.Attributes.Select(attribute => {
                    if (attributeGrowthFormulas == null) {
                        return new KeyValuePair<Attribute, GrowthFormula>(attribute, null);
                    }
                    else {
                        var existingAttribute = attributeGrowthFormulas.FirstOrDefault(s => s.Key.name == attribute.name);
                        if (existingAttribute.Key == null) {
                            return new KeyValuePair<Attribute, GrowthFormula>(attribute, null);
                        }
                        return existingAttribute;
                    }
                }).ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value);
            }
            else {
                attributeGrowthFormulas = new();
            }
        }

#if UNITY_EDITOR
        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChanged;
            Stat.OnStatDeleted += HandleStatDeleted;
            Attribute.OnAttributeDeleted += HandleAttributeDeleted;
        }

        private void OnDisable() {
            Selection.selectionChanged -= OnSelectionChanged;
            Stat.OnStatDeleted -= HandleStatDeleted;
            Attribute.OnAttributeDeleted -= HandleAttributeDeleted;
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
        
        private void HandleAttributeDeleted(Attribute deletedAttribute) {
            if (attributeGrowthFormulas.Keys.Contains(deletedAttribute)) {
                attributeGrowthFormulas.Remove(deletedAttribute);
            }
        }
#endif
    }
}
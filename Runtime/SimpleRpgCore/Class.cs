using System;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEngine;
using Attribute = ElectricDrill.SimpleRpgCore.Attributes.Attribute;

namespace ElectricDrill.SimpleRpgCore
{
    public class Class : ScriptableObject, IStatSetSource
    {
        [SerializeField] protected GrowthFormula _maxHpGrowthFormula;
        [SerializeField] protected AttributeSet attributeSet;
        [SerializeField] protected StatSet _statSet;
        
        [SerializeField, HideInInspector] 
        internal SerializableDictionary<Attribute, GrowthFormula> attributeGrowthFormulas = new();
        
        [SerializeField, HideInInspector] 
        internal SerializableDictionary<Stat, GrowthFormula> _statGrowthFormulas = new();
        
        public AttributeSet AttributeSet { get => attributeSet; internal set => attributeSet = value; }
        public virtual StatSet StatSet { get => _statSet; internal set => _statSet = value; }

        public long GetMaxHpAt(int level) {
            Assert.IsNotNull(_maxHpGrowthFormula, "Max HP growth formula is not set");
            return _maxHpGrowthFormula.GetGrowthValue(level);
        }

        public long GetAttributeAt(Attribute attribute, int level) {
            Assert.IsNotNull(attributeGrowthFormulas[attribute], $"Growth formula for {attribute.name} is null");
            return attributeGrowthFormulas[attribute].GetGrowthValue(level);
        } 
        
        public virtual long GetStatAt(Stat stat, int level) => 
            _statGrowthFormulas.TryGetValue(stat, out var formula) && formula != null ? 
            formula.GetGrowthValue(level) : 
            throw new ArgumentException($"Growth formula for {stat.name} is not set");
        
        private void OnValidate()
        {
            if (StatSet == null)
                Debug.LogWarning("StatSet of class is null");
            UpdateGrowthFormulas(_statSet?.Stats, _statGrowthFormulas);
            UpdateGrowthFormulas(attributeSet?.Attributes, attributeGrowthFormulas);
        }

        private void UpdateGrowthFormulas<T>(IEnumerable<T> items, SerializableDictionary<T, GrowthFormula> formulas) where T : ScriptableObject
        {
            if (items == null)
            {
                formulas.Clear();
                return;
            }

            var updated = items
                .Where(item => item != null)
                .ToDictionary(
                    item => item,
                    item => formulas.TryGetValue(item, out var formula) ? formula : null
                );

            formulas.Clear();
            foreach (var kvp in updated)
                formulas[kvp.Key] = kvp.Value;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            Stat.OnStatDeleted += RemoveStat;
            Attribute.OnAttributeDeleted += RemoveAttribute;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            Stat.OnStatDeleted -= RemoveStat;
            Attribute.OnAttributeDeleted -= RemoveAttribute;
        }
        
        void RemoveStat(Stat stat) => _statGrowthFormulas.Remove(stat);
        
        void RemoveAttribute(Attribute attribute) => attributeGrowthFormulas.Remove(attribute);

        private void OnSelectionChanged()
        {
            if (Selection.activeObject == this) OnValidate();
        }
#endif
    }
    
    public static class ClassMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Class _C", false, 5)]
        public static void CreateClass()
        {
            var asset = ScriptableObject.CreateInstance<Class>();
            ProjectWindowUtil.CreateAsset(asset, "New Class.asset");
        }
    }
}
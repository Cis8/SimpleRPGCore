using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [SerializeField] protected StatGrowthFnPair[] _statGrowthFnPairs = Array.Empty<StatGrowthFnPair>();
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
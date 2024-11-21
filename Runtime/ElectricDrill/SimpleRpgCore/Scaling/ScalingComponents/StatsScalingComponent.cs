using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    [CreateAssetMenu(fileName = "New Stats Scaling Component", menuName = "Simple RPG/Scaling/Stats Component")]
    public class StatsScalingComponent : ScalingComponent
    {
        [SerializeField] private StatSet _statSet;

        [SerializeField]
        private StatValuePair<double>[] _scalingAttributeValues = Array.Empty<StatValuePair<double>>();

        public override long CalculateValue(EntityCore entity) {
            Assert.AreEqual(_statSet, entity.Stats.StatSet, $"StatSet of {name} StatsScalingComponent ({_statSet.name}) does not match {entity.name}'s StatSet ({entity.Stats.StatSet.name})");
            return _scalingAttributeValues.Sum(attributeMapping => {
                var statValue = entity.Stats.Get(attributeMapping.Stat);
                return (long) Math.Round(statValue * attributeMapping.Value);
            });
        }

        private void OnValidate() {
            if (_statSet != null) {
                _scalingAttributeValues = _statSet.Stats.Select(stat => {
                    if (_scalingAttributeValues == null) {
                        return new StatValuePair<double> {
                            Stat = stat,
                            Value = 0
                        };
                    }
                    else {
                        var existingStat = _scalingAttributeValues.FirstOrDefault(s => s.Stat.name == stat.name);
                        if (existingStat.Stat == null) {
                            return new StatValuePair<double> {
                                Stat = stat,
                                Value = 0
                            };
                        }
                        return existingStat;
                    }
                }).ToArray();
            }
            else {
                _scalingAttributeValues = Array.Empty<StatValuePair<double>>();
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
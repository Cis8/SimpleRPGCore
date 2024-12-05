using System;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    [CreateAssetMenu(fileName = "New StatSet", menuName = "Simple RPG/Stat Set")]
    public class StatSet : ScriptableObject, IStatContainer
    {
        [SerializeField] private SerializableHashSet<Stat> _stats = new();

        public IReadOnlyList<Stat> Stats => _stats.ToList();

        public Stat Get(Stat stat) {
            return _stats.First(s => s == stat);
        }
        
        public bool Contains(Stat stat) {
            return _stats.Contains(stat);
        }

        internal void SetStats(SerializableHashSet<Stat> stats) {
            _stats = stats;
        }
        
#if UNITY_EDITOR
        private void OnEnable() {
            Stat.OnStatDeleted += HandleStatDeleted;
        }

        private void OnDisable() {
            Stat.OnStatDeleted -= HandleStatDeleted;
        }

        private void HandleStatDeleted(Stat deletedStat) {
            if (_stats.Contains(deletedStat)) {
                _stats.Remove(deletedStat);
            }
        }
#endif
    }
}
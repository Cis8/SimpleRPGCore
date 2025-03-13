using System;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    public class StatSet : ScriptableObject, IStatContainer
    {
        [SerializeField] internal SerializableHashSet<Stat> _stats = new();

        public IReadOnlyList<Stat> Stats => _stats.ToList();

        public Stat Get(Stat stat) {
            return _stats.First(s => s == stat);
        }
        
        public virtual bool Contains(Stat stat) {
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
    
    public static class StatSetMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Stat Set &S", false, 3)]
        public static void CreateStatSet()
        {
            var asset = ScriptableObject.CreateInstance<StatSet>();
            ProjectWindowUtil.CreateAsset(asset, "New Stat Set.asset");
        }
    }
}
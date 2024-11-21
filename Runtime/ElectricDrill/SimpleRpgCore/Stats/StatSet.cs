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

        public SerializableHashSet<Stat> Stats => _stats;

        public Stat Get(Stat stat) {
            return _stats.First(s => s == stat);
        }
        
        public bool Contains(Stat stat) {
            return _stats.Contains(stat);
        }
    }
}
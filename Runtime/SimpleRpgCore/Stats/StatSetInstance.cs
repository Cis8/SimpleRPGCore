using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats {
    public class StatSetInstance : IEnumerable<KeyValuePair<Stat, long>>, IStatContainer
    {
        private Dictionary<Stat, long> _stats = new();

        private StatSetInstance() {}
        
        public StatSetInstance(StatSet statSet) {
            foreach (var stat in statSet.Stats) {
                _stats.Add(stat, 0);
            }
        }

        public Dictionary<Stat, long> Stats => _stats;
        

        /// <summary>
        /// Adds <paramref name="value"/> to <paramref name="stat"/>. If the stat does not exist, it will be created and
        /// initialized with <paramref name="value"/>. Use negative values to subtract from the stat.
        /// </summary>
        /// <param name="stat">The stat to add the value to.</param>
        /// <param name="value">The value to add to the stat.</param>
        public void AddValue(Stat stat, long value) {
            if (!_stats.TryAdd(stat, value))
                _stats[stat] += value;
        }
        
        /// <summary>
        /// </summary>
        /// <param name="stat">The stat to be retrieved.</param>
        /// <returns>The value of the <paramref name="stat"/></returns>
        public long Get(Stat stat) {
            return _stats[stat];
        }
        
        // overload of the [] operator
        public long this[Stat stat] {
            get => Get(stat);
            set => AddValue(stat, value);
        }
        
        public StatSetInstance Clone() {
            var clone = new StatSetInstance();
            foreach (var stat in _stats) {
                clone.AddValue(stat.Key, stat.Value);
            }
            return clone;
        }
        
        public bool Contains(Stat stat) {
            return _stats.ContainsKey(stat);
        }
        
        public Percentage GetAsPercentage(Stat stat) {
            return new Percentage(Get(stat));
        }

        public IEnumerator<KeyValuePair<Stat, long>> GetEnumerator() {
            return _stats.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        /// <summary>
        /// The addition operator for StatSetInstance. Considered the stats present in the StatSetInstance
        /// <paramref name="a"/>, their values will be summed with the values of the respective stats in the StatSetInstance
        /// <paramref name="b"/>. If a stat is present in the StatSetInstance <paramref name="a"/> but not in the
        /// StatSetInstance <paramref name="b"/>, an exception will be thrown.
        /// </summary>
        /// <param name="a">The first StatSetInstance</param>
        /// <param name="b">The second StatSetInstance</param>
        /// <returns>A new StatSetInstance with the sum of the stats of <paramref name="a"/> to the respective values of
        /// the stats of <paramref name="b"/></returns> 
        public static StatSetInstance operator +(StatSetInstance a, StatSetInstance b) {
            StatSetInstance result = new();
            
            foreach (var stat in a) {
                result.AddValue(stat.Key, stat.Value + b[stat.Key]);
            }
            return result;
        }
    }
}

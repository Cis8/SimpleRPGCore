using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats {
    [CreateAssetMenu(fileName = "New Stat to Stat Modifier", menuName = "Simple RPG Core/Stat to Stat Modifier")]
    public class StatToStatModifier : ScriptableObject
    {
        [SerializeField] private Stat targetStat;
        [SerializeField] private Stat sourceStat;
        [SerializeField] private Percentage percentage;
        
        public Stat SourceStat => sourceStat;
        public Stat TargetStat => targetStat;
        public Percentage Percentage => percentage;
    }
}

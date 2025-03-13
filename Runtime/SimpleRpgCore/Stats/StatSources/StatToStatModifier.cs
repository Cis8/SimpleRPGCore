using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats {
    public class StatToStatModifier : ScriptableObject
    {
        [SerializeField] private Stat targetStat;
        [SerializeField] private Stat sourceStat;
        [SerializeField] private Percentage percentage;
        
        public Stat SourceStat => sourceStat;
        public Stat TargetStat => targetStat;
        public Percentage Percentage => percentage;
    }
    
    public static class StatToStatModifierMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Stat to Stat Modifier &M", false, 4)]
        public static void CreateStatToStatModifier()
        {
            var asset = ScriptableObject.CreateInstance<StatToStatModifier>();
            ProjectWindowUtil.CreateAsset(asset, "New Stat to Stat Modifier.asset");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Damage;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Damage {
    [CreateAssetMenu(fileName = "FlatDefReductionFn", menuName = "Simple RPG/Def Reduction Functions/Flat Def Reduction")]
    public class FlatDefReductionFn : DefReductionFn
    {
        [SerializeField] private double constant = 1.0;
        
        public override long ReducedDef(long piercingStatValue, long piercedStatValue) {
            var reducedValue = piercedStatValue - piercingStatValue * constant;
            return (long)(reducedValue < 0 ? 0 : reducedValue);
        }
    }
}

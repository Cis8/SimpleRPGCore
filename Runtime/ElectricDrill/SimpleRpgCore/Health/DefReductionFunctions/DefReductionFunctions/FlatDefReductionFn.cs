using System;
using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Health;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Health {
    [CreateAssetMenu(fileName = "FlatDefReductionFn", menuName = "Simple RPG/Def Reduction Functions/Flat Def Reduction")]
    public class FlatDefReductionFn : DefReductionFn
    {
        [SerializeField] private double constant = 1.0;
        
        public override double ReducedDef(long piercingStatValue, long piercedStatValue) {
            var reducedValue = piercedStatValue - piercingStatValue * constant;
            return (long)Math.Round(reducedValue < 0 ? 0 : reducedValue);
        }
    }
}

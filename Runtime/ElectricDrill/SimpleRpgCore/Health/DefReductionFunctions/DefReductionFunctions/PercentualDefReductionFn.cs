using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Health;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Health {
    [CreateAssetMenu(fileName = "PercentualDefReductionFn", menuName = "Simple RPG/Def Reduction Functions/Percentual Def Reduction")]
    public class PercentualDefReductionFn : DefReductionFn
    {
        public override long ReducedDef(long piercingStatValue, long piercedStatValue) {
            piercedStatValue -= (long)(piercedStatValue * (piercingStatValue / 100.0d));
            return piercedStatValue < 0 ? 0 : piercedStatValue;
        }
    }
}

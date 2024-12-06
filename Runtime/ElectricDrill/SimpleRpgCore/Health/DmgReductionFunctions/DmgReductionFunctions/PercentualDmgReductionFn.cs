using System;
using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Health;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Health {
    [CreateAssetMenu(fileName = "PercentualDmgReductionFn", menuName = "Simple RPG/Dmg Reduction Functions/Percentual Dmg Reduction")]
    public class PercentualDmgReductionFn : DmgReductionFn
    {
        public override long ReducedDmg(long amount, double defensiveStatValue) {
            var reducedAmount = amount - amount * defensiveStatValue / 100.0d;
            return reducedAmount < 0 ? 0 : (long)Math.Round(reducedAmount);
        }
    }
}

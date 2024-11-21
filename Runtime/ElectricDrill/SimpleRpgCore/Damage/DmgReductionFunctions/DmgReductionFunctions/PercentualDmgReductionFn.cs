using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Damage;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Damage {
    [CreateAssetMenu(fileName = "PercentualDmgReductionFn", menuName = "Simple RPG/Dmg Reduction Functions/Percentual Dmg Reduction")]
    public class PercentualDmgReductionFn : DmgReductionFn
    {
        public override long ReducedDmg(long amount, long defensiveStatValue) {
            amount -= (long)(amount * (defensiveStatValue / 100.0d));
            return amount < 0 ? 0 : amount;
        }
    }
}

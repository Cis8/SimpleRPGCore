using System;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Health
{
    [CreateAssetMenu(fileName = "FlatDmgReductionFn", menuName = "Simple RPG/Dmg Reduction Functions/Flat Dmg Reduction")]
    public class FlatDmgReductionFn : DmgReductionFn
    {
        
        // the constant that will be multiplied by the defensive statistic to reduce the damage
        [SerializeField] private double constant = 1.0;

        /// <summary>
        /// Subtracts constant * <paramref name="defensiveStatValue"/> to the amount of damage
        /// </summary>
        /// <returns>The reduced amount of damage</returns>
        public override long ReducedDmg(long amount, double defensiveStatValue) {
            var reducedValue = amount - defensiveStatValue * constant;
            return (long)Math.Round(reducedValue < 0 ? 0 : reducedValue);
        }
    }
}
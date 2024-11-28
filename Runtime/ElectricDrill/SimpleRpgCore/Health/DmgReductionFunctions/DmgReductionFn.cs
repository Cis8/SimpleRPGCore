using System;
using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Health
{
    public abstract class DmgReductionFn : ScriptableObject
    {
        /// <summary>
        /// Reduces the <paramref name="amount"/> of damage by using <paramref name="defensiveStatValue"/>.
        /// </summary>
        /// <param name="amount">The amount of damage to be reduced</param>
        /// <param name="defensiveStatValue">Value of the defensive statistic</param>
        /// <returns>The reduced amount of damage</returns>
        public abstract long ReducedDmg(long amount, long defensiveStatValue);
    }
}
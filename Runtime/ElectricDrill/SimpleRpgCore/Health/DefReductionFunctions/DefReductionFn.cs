using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Health {
    public abstract class DefReductionFn : ScriptableObject
    {
        public abstract double ReducedDef(long piercingStatValue, long piercedStatValue);
    }
}

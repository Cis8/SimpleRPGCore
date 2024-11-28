using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Health {
    public abstract class DefReductionFn : ScriptableObject
    {
        public abstract long ReducedDef(long piercingStatValue, long piercedStatValue);
    }
}

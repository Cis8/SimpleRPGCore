using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats {
    [Serializable]
    public struct StatValuePair<T>
    {
        public Stat Stat;
        public T Value;
    }
}

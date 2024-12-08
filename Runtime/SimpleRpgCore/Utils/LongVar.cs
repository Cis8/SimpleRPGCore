using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    [CreateAssetMenu]
    public class LongVar : ScriptableObject
    {
        public long Value;
        
        // implicit conversion from IntVar to int
        public static implicit operator long(LongVar var) {
            return var.Value;
        }
    }
}
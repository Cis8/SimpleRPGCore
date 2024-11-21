using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    [CreateAssetMenu]
    public class IntVar : ScriptableObject
    {
        public int Value;
        
        // implicit conversion from IntVar to int
        public static implicit operator int(IntVar var) {
            return var.Value;
        }
    }
}

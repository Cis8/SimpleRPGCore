using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    [CreateAssetMenu]
    public class LongVar : ScriptableObject
    {
        [SerializeField] private long _value;
        public long Value {
            get => _value;
            set => _value = value;
        }
        
        // implicit conversion from IntVar to int
        public static implicit operator long(LongVar var) {
            return var.Value;
        }
    }
}
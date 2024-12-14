using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    [CreateAssetMenu]
    public class IntVar : ScriptableObject
    {
        [SerializeField] private int _value;
        public int Value {
            get => _value;
            set => _value = value;
        }
        
        // implicit conversion from IntVar to int
        public static implicit operator int(IntVar var) {
            return var.Value;
        }
    }
}

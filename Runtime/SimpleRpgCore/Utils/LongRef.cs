using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    [Serializable]
    public class LongRef
    {
        public bool UseConstant = true;
        public long ConstantValue;
        public LongVar Variable;

        public long Value {
            get => UseConstant ? ConstantValue : Variable;
            set {
                if (UseConstant) {
                    ConstantValue = value;
                } else {
                    Variable.Value = value;
                }
            }
        }
        
        // implicit conversion from LongRef to long
        public static implicit operator long(LongRef reference) => reference.Value;
    }
}
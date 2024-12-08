using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    [Serializable]
    public class IntRef
    {
        public bool UseConstant = true;
        public int ConstantValue;
        public IntVar Variable;

        public int Value {
          get => UseConstant ? ConstantValue : Variable;
          set {
            if (UseConstant) {
              ConstantValue = value;
            } else {
              Variable.Value = value;
            }
          }
        }
        
        public static implicit operator int(IntRef reference) => reference.Value;
        
        public static implicit operator IntRef(int value) => new IntRef { UseConstant = true, ConstantValue = value };
    }
}

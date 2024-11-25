using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    [Serializable]
    public struct SerKeyValPair<T, U>
    {
        public T Key;
        public U Value;
        
        public SerKeyValPair(T key, U value) {
            Key = key;
            Value = value;
        }
        
        public static implicit operator KeyValuePair<T, U>(SerKeyValPair<T, U> serKeyValPair) {
            return new KeyValuePair<T, U>(serKeyValPair.Key, serKeyValPair.Value);
        }
        
        public static implicit operator SerKeyValPair<T, U>(KeyValuePair<T, U> keyValuePair) {
            return new SerKeyValPair<T, U>(keyValuePair.Key, keyValuePair.Value);
        }
    }
}

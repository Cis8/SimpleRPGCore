using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    [Serializable]
    public class Percentage : IComparable<Percentage>
    {
        [SerializeField] long _value;
        
        // implicit conversion from Percentage to double
        public Percentage(long value) {
            _value = value;
        }

        // implicit conversion from Percentage to double
        public static implicit operator double(Percentage percentage) {
            return percentage._value / 100.0d;
        }
        
        // implicit conversion from long to Percentage
        public static implicit operator Percentage(long value) {
            return new Percentage(value);
        }

        // explicit conversion from Percentage to long
        public static explicit operator long(Percentage percentage) {
            return percentage._value;
        }
        
        // override + operator
        public static Percentage operator +(Percentage a, Percentage b) {
            return new Percentage(a._value + b._value);
        }
        
        // override - operator
        public static Percentage operator -(Percentage a, Percentage b) {
            return new Percentage(a._value - b._value);
        }
        
        public static Percentage operator -(Percentage a) {
            return new Percentage(-a._value);
        }

        public int CompareTo(Percentage other) {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return _value.CompareTo(other._value);
        }
        
        public override string ToString() {
            return _value.ToString();
        }
    }
}

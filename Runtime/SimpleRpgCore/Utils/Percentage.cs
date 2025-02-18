using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils {
    /// <summary>
    /// The Percentage class represents a percentage value and provides various operators and conversions.<br/>
    /// Implicit long to Percentage value conversion is available. To express a 100% value, use 100L.<br/>
    /// Implicit Percentage to double conversion is available. When doing so, the percentage is automatically
    /// divided by 100.<br/>
    /// </summary>
    [Serializable]
    public class Percentage : IComparable<Percentage>
    {
        /// <summary>
        /// The internal value of the percentage.
        /// </summary>
        [SerializeField] long _value;

        /// <summary>
        /// Initializes a new instance of the Percentage class with the specified value.
        /// To express a 100% value, use 100L.
        /// </summary>
        /// <param name="value">The value of the percentage.</param>
        public Percentage(long value) {
            _value = value;
        }

        /// <summary>
        /// Implicit conversion from Percentage to double. The conversion automatically divides the value by 100.
        /// </summary>
        /// <param name="percentage">The percentage to convert.</param>
        public static implicit operator double(Percentage percentage) {
            return percentage._value / 100.0d;
        }

        /// <summary>
        /// Implicit conversion from long to Percentage. To express a 100% value, use 100L.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator Percentage(long value) {
            return new Percentage(value);
        }

        /// <summary>
        /// Explicit conversion from Percentage to long. The conversion does not divide the value by 100.
        /// </summary>
        /// <param name="percentage">The percentage to convert.</param>
        public static explicit operator long(Percentage percentage) {
            return percentage._value;
        }

        /// <summary>
        /// Overrides the + operator to add two Percentage instances.
        /// </summary>
        /// <param name="a">The first percentage.</param>
        /// <param name="b">The second percentage.</param>
        /// <returns>A new Percentage instance representing the sum.</returns>
        public static Percentage operator +(Percentage a, Percentage b) {
            return new Percentage(a._value + b._value);
        }

        /// <summary>
        /// Overrides the - operator to subtract one Percentage from another.
        /// </summary>
        /// <param name="a">The first percentage.</param>
        /// <param name="b">The second percentage.</param>
        /// <returns>A new Percentage instance representing the difference.</returns>
        public static Percentage operator -(Percentage a, Percentage b) {
            return new Percentage(a._value - b._value);
        }

        /// <summary>
        /// Overrides the unary - operator to negate a Percentage.
        /// </summary>
        /// <param name="a">The percentage to negate.</param>
        /// <returns>A new Percentage instance representing the negated value.</returns>
        public static Percentage operator -(Percentage a) {
            return new Percentage(-a._value);
        }

        /// <summary>
        /// Compares the current Percentage instance with another Percentage instance.
        /// </summary>
        /// <param name="other">The other percentage to compare to.</param>
        /// <returns>An integer indicating the relative order of the percentages.</returns>
        public int CompareTo(Percentage other) {
            if (ReferenceEquals(this, other)) return 0;
            if (other is null) return 1;
            return _value.CompareTo(other._value);
        }

        /// <summary>
        /// Returns a string representation of the percentage value.
        /// </summary>
        /// <returns>A string representing the percentage value.</returns>
        public override string ToString() {
            return _value + "%";
        }
    }
}
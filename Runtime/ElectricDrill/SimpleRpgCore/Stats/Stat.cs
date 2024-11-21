using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    [CreateAssetMenu(fileName = "New Stat", menuName = "Simple RPG/Stat")]
    public class Stat : ScriptableObject
    {
        [SerializeField] private bool isPercentage = false;
        [SerializeField] private bool hasMaxValue = false;
        [SerializeField] private long maxValue;
        [SerializeField] private bool hasMinValue = true;
        [SerializeField] private int minValue = 0;
        
        public bool IsPercentage => isPercentage;
        public bool HasMaxValue => hasMaxValue;
        public long MaxValue => maxValue;
        public bool HasMinValue => hasMinValue;
        public int MinValue => minValue;

        internal long Clamp(long value) {
            if (hasMaxValue && value > maxValue) {
                return maxValue;
            }

            if (hasMinValue && value < minValue) {
                return minValue;
            }

            return value;
        }
        
        private bool Equals(Stat other) {
            return name == other.name;
        }
        
        public override bool Equals(object obj) {
            return obj is Stat other && Equals(other);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public static bool operator ==(Stat a, Stat b) {
            // a not null XOR b not null
            if (ReferenceEquals(a, null) ^ ReferenceEquals(b, null)) {
                return false;
            }
            // if here a is null, also b is null
            return ReferenceEquals(a, null) || a!.Equals(b!);
        }
        
        public static bool operator !=(Stat a, Stat b) {
            return !(a == b);
        }
    }
}
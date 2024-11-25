using System;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [CreateAssetMenu(fileName = "New Characteristic", menuName = "Simple RPG/Characteristic")]
    [Serializable]
    public class Characteristic : BoundedValue
    {
        private bool Equals(Characteristic other) {
            return name == other.name;
        }
        
        public override bool Equals(object obj) {
            return obj is Characteristic other && Equals(other);
        }
        
        public override int GetHashCode() {
            return base.GetHashCode();
        }
        
        public static bool operator ==(Characteristic a, Characteristic b) {
            // a not null XOR b not null
            if (ReferenceEquals(a, null) ^ ReferenceEquals(b, null)) {
                return false;
            }
            // if here a is null, also b is null
            return ReferenceEquals(a, null) || a!.Equals(b!);
        }
        
        public static bool operator !=(Characteristic a, Characteristic b) {
            return !(a == b);
        }
    }
}

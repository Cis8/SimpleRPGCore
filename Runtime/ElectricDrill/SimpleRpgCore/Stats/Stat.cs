using ElectricDrill.SimpleRpgCore.Scaling;
using JetBrains.Annotations;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    [CreateAssetMenu(fileName = "New Stat", menuName = "Simple RPG/Stat")]
    public class Stat : BoundedValue
    {
        [SerializeField] [CanBeNull] private CharacteristicsScalingComponent characteristicsScaling;

        [CanBeNull]
        public CharacteristicsScalingComponent CharacteristicsScaling => characteristicsScaling;

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

using System;
using ElectricDrill.SimpleRpgCore.Scaling;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    public class Stat : BoundedValue
    {
        [SerializeField] [CanBeNull] private AttributesScalingComponent attributesScaling;

        [CanBeNull] public AttributesScalingComponent AttributesScaling {
            get => attributesScaling;
            internal set => attributesScaling = value;
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
        
#if UNITY_EDITOR
        public static event Action<Stat> OnStatDeleted;

        public static void OnWillDeleteStat(Stat stat) {
            OnStatDeleted?.Invoke(stat);
        }
#endif
    }
    
    public static class StatMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Stat _S", false, 2)]
        public static void CreateStat()
        {
            var asset = ScriptableObject.CreateInstance<Stat>();
            ProjectWindowUtil.CreateAsset(asset, "New Stat.asset");
        }
    }
}
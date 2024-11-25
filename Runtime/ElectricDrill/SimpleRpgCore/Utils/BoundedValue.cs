using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    public abstract class BoundedValue : ScriptableObject
    {
        [SerializeField] private bool hasMaxValue = false;
        [SerializeField] private long maxValue;
        [SerializeField] private bool hasMinValue = true;
        [SerializeField] private int minValue = 0;
        
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
    }
}
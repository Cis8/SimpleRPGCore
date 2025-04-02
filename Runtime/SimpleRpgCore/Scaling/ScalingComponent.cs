using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling {
    public abstract class ScalingComponent : ScriptableObject
    {
        public abstract long CalculateValue(EntityCore entity);
    }
}

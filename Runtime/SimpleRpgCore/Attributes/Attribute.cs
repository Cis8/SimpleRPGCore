using System;
using UnityEngine;


namespace ElectricDrill.SimpleRpgCore.Attributes
{
    [CreateAssetMenu(fileName = "New Attribute", menuName = "Simple RPG Core/Attribute")]
    [Serializable]
    public class Attribute : BoundedValue
    {
#if UNITY_EDITOR
        public static event Action<Attribute> OnAttributeDeleted;

        public static void OnWillDeleteAttribute(Attribute attribute) {
            OnAttributeDeleted?.Invoke(attribute);
        }
#endif
    }
}

using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace ElectricDrill.SimpleRpgCore.Attributes
{
    [CreateAssetMenu(fileName = "New Attribute", menuName = "Simple RPG/Attribute")]
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

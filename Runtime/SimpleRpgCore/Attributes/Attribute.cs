using System;
using UnityEditor;
using UnityEngine;


namespace ElectricDrill.SimpleRpgCore.Attributes
{
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
    
    public static class AttributeMenuItems
    {
        [MenuItem("Assets/Create/Simple RPG Core/Attribute _A", false, 0)]
        public static void CreateAttribute()
        {
            var asset = ScriptableObject.CreateInstance<Attribute>();
            ProjectWindowUtil.CreateAsset(asset, "New Attribute.asset");
        }
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Scaling;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(AttributesScalingComponent))]
    public class AttributesScalingComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Get the target object
            AttributesScalingComponent component = (AttributesScalingComponent)target;
            
            // Draw the script field as disabled
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject(component), typeof(MonoScript), false);
            }

            // Draw the required set field
            SerializedProperty setProperty = serializedObject.FindProperty("_set");
            InspectorTypography.RequiredProperty(setProperty, "Attribute Set", "The attribute set that defines which attributes can be used to configure their scaling in this component");

            // Draw the custom inspector for the serializable dictionary
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Scaling Attribute Values", EditorStyles.boldLabel);

            // Iterate through the dictionary and draw each key-value pair
            var keys = new List<Attribute>(component._scalingAttributeValues.Keys);
            foreach (var key in keys) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20); // Indentation
                EditorGUILayout.LabelField(key.name, GUILayout.Width(150));
                component._scalingAttributeValues[key] = EditorGUILayout.DoubleField(component._scalingAttributeValues[key], GUILayout.Width(EditorGUIUtility.currentViewWidth / 4));
                EditorGUILayout.EndHorizontal();
            }

            // Info box
            EditorGUILayout.HelpBox("Use double values. For example: 0.8 means 80%, 1.6 means 160%.", MessageType.Info);

            EditorGUILayout.EndVertical();

            // Apply changes to the serialized object
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Characteristics;
using ElectricDrill.SimpleRpgCore.Scaling;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(CharacteristicsScalingComponent))]
    public class CharacteristicsScalingComponentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Get the target object
            CharacteristicsScalingComponent component = (CharacteristicsScalingComponent)target;

            // Draw the custom inspector for the serializable dictionary
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Scaling Attribute Values", EditorStyles.boldLabel);

            // Iterate through the dictionary and draw each key-value pair
            var keys = new List<Characteristic>(component._scalingAttributeValues.Keys);
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
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
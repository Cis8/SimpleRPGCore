using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling {
    [CustomEditor(typeof(ScalingFormula))]
    public class ScalingFormulaEditor : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            ScalingFormula scalingFormula = (ScalingFormula)target;

            // Base value box
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Base value", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            SerializedProperty useScalingBaseValueProp = serializedObject.FindProperty("useScalingBaseValue");
            EditorGUILayout.PropertyField(useScalingBaseValueProp, new GUIContent("Use a scaling base value"));

            if (useScalingBaseValueProp.boolValue) {
                SerializedProperty scalingBaseValueProp = serializedObject.FindProperty("scalingBaseValue");
                EditorGUILayout.PropertyField(scalingBaseValueProp);
            } else {
                SerializedProperty fixedBaseValueProp = serializedObject.FindProperty("fixedBaseValue");
                EditorGUILayout.PropertyField(fixedBaseValueProp);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            // Entity scalings box
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Entity scalings", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            SerializedProperty selfScalingComponentsProp = serializedObject.FindProperty("selfScalingComponents");
            EditorGUILayout.PropertyField(selfScalingComponentsProp, true);

            SerializedProperty targetScalingComponentsProp = serializedObject.FindProperty("targetScalingComponents");
            EditorGUILayout.PropertyField(targetScalingComponentsProp, true);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
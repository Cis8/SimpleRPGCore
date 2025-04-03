using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    public static class InspectorTypography
    {
        static GUIStyle requiredStyle = new(EditorStyles.label);

        static InspectorTypography() {
            requiredStyle.normal.textColor = Color.red;
        }

        public static void RequiredProperty(SerializedProperty property, string title, string tooltip = default) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("*", "This field is required"), requiredStyle, GUILayout.Width(10));
            EditorGUILayout.PropertyField(property, new GUIContent(title, tooltip));
            EditorGUILayout.EndHorizontal();
        }
    }
}
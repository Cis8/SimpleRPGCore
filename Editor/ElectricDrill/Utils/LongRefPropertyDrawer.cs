using UnityEditor;
using UnityEngine;
using ElectricDrill.SimpleRpgCore.Utils;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomPropertyDrawer(typeof(LongRef))]
    public class LongRefPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var useConstant = property.FindPropertyRelative("UseConstant");
            var constantValue = property.FindPropertyRelative("ConstantValue");
            var variable = property.FindPropertyRelative("Variable");

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var toggleRect = new Rect(position.x, position.y, 20, position.height);
            var labelRect = new Rect(position.x + 25, position.y, 100, position.height);
            var valueRect = new Rect(position.x + 130, position.y, position.width - 130, position.height);

            useConstant.boolValue = EditorGUI.Toggle(toggleRect, useConstant.boolValue);
            EditorGUI.LabelField(labelRect, "Use Constant");

            if (useConstant.boolValue)
            {
                constantValue.longValue = EditorGUI.LongField(valueRect, constantValue.longValue);
            }
            else
            {
                EditorGUI.PropertyField(valueRect, variable, GUIContent.none);
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}
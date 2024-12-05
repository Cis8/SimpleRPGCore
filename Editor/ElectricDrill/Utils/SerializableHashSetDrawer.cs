using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomPropertyDrawer(typeof(SerializableHashSet<>), true)]
    public class SerializableHashSetDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 18f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var valuesProperty = property.FindPropertyRelative("inspectorReservedValues");
            var missingValueProperty = property.FindPropertyRelative("missingValue");

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < valuesProperty.arraySize; i++)
                {
                    var valueProperty = valuesProperty.GetArrayElementAtIndex(i);

                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    var fieldWidth = position.width - ButtonWidth;
                    var valueRect = new Rect(position.x, position.y, fieldWidth, position.height);
                    var removeButtonRect = new Rect(position.x + fieldWidth, position.y, ButtonWidth, position.height);

                    EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

                    if (GUI.Button(removeButtonRect, "-"))
                    {
                        valuesProperty.DeleteArrayElementAtIndex(i);
                    }
                }

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var addButtonRect = new Rect(position.x, position.y, position.width, position.height);
                if (GUI.Button(addButtonRect, "Add"))
                {
                    valuesProperty.arraySize++;
                    var newValueProperty = valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1);
                    if (newValueProperty.propertyType == SerializedPropertyType.ObjectReference) {
                        newValueProperty.objectReferenceValue = EditorGUI.ObjectField(new Rect(position.x, position.y, position.width, position.height), "Select Value", null, typeof(Object), true);
                    }
                    missingValueProperty.boolValue = true;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            var valuesProperty = property.FindPropertyRelative("inspectorReservedValues");
            return (valuesProperty.arraySize + 2) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
    }
}

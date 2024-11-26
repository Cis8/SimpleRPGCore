using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 18f;
        private const float ValueWidth = 100f; // Adjust this value to resize the value box

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keysProperty = property.FindPropertyRelative("inspectorReservedPairs");
            var missingKeyPairProperty = property.FindPropertyRelative("missingKeyPair");

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < keysProperty.arraySize; i++)
                {
                    var pairProperty = keysProperty.GetArrayElementAtIndex(i);
                    var keyProperty = pairProperty.FindPropertyRelative("Key");
                    var valueProperty = pairProperty.FindPropertyRelative("Value");

                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    var keyRect = new Rect(position.x, position.y, position.width - ButtonWidth - ValueWidth, position.height);
                    var valueRect = new Rect(position.x + position.width - ButtonWidth - ValueWidth, position.y, ValueWidth, position.height);
                    var removeButtonRect = new Rect(position.x + position.width - ButtonWidth, position.y, ButtonWidth, position.height);

                    EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
                    EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

                    if (GUI.Button(removeButtonRect, "-"))
                    {
                        keysProperty.DeleteArrayElementAtIndex(i);
                    }
                }

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var addButtonRect = new Rect(position.x, position.y, position.width, position.height);
                if (GUI.Button(addButtonRect, "Add"))
                {
                    keysProperty.arraySize++;
                    var newPairProperty = keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);
                    var keyProperty = newPairProperty.FindPropertyRelative("Key");
                    var valueProperty = newPairProperty.FindPropertyRelative("Value");

                    if (keyProperty.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        keyProperty.objectReferenceValue = EditorGUI.ObjectField(new Rect(position.x, position.y, position.width, position.height), "Select Key", null, typeof(Object), true);
                    }

                    if (valueProperty.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        valueProperty.objectReferenceValue = EditorGUI.ObjectField(new Rect(position.x, position.y, position.width, position.height), "Select Value", null, typeof(Object), true);
                    }

                    missingKeyPairProperty.boolValue = true;
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

            var keysProperty = property.FindPropertyRelative("inspectorReservedPairs");
            return (keysProperty.arraySize + 2) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
    }
}
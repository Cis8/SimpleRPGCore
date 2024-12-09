using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const float ButtonWidth = 18f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var pairsProperty = property.FindPropertyRelative("inspectorReservedPairs");
            var missingKeyPairProperty = property.FindPropertyRelative("missingKeyPair");

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < pairsProperty.arraySize; i++)
                {
                    var pairProperty = pairsProperty.GetArrayElementAtIndex(i);
                    var keyProperty = pairProperty.FindPropertyRelative("Key");
                    var valueProperty = pairProperty.FindPropertyRelative("Value");

                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    var fieldWidth = (position.width - ButtonWidth) / 2;
                    var keyRect = new Rect(position.x, position.y, fieldWidth, position.height);
                    var valueRect = new Rect(position.x + fieldWidth, position.y, fieldWidth, position.height);
                    var removeButtonRect = new Rect(position.x + 2 * fieldWidth, position.y, ButtonWidth, position.height);

                    EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
                    EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

                    if (GUI.Button(removeButtonRect, "-"))
                    {
                        pairsProperty.DeleteArrayElementAtIndex(i);
                    }
                }

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var addButtonRect = new Rect(position.x, position.y, position.width, position.height);
                if (GUI.Button(addButtonRect, "Add"))
                {
                    pairsProperty.arraySize++;
                    var newPairProperty = pairsProperty.GetArrayElementAtIndex(pairsProperty.arraySize - 1);
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
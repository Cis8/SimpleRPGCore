using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const float KeyColumnWidth = 0.3f; // 30% of the total width for the key
        private const float ValueColumnWidth = 0.7f; // 70% of the total width for the value
        private const float ButtonWidth = 18f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var pairsProperty = property.FindPropertyRelative("inspectorReservedPairs");
            var missingKeyPairProperty = property.FindPropertyRelative("missingKeyPair");

            Rect headerRect = position;
            headerRect.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, label);

            if (property.isExpanded) {
                EditorGUI.indentLevel++;
                float yPos = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                for (int i = 0; i < pairsProperty.arraySize; i++) {
                    var pairProperty = pairsProperty.GetArrayElementAtIndex(i);
                    var keyProperty = pairProperty.FindPropertyRelative("Key");
                    var valueProperty = pairProperty.FindPropertyRelative("Value");

                    float valueHeight = EditorGUI.GetPropertyHeight(valueProperty, true);
                    float rowHeight = Mathf.Max(EditorGUIUtility.singleLineHeight, valueHeight);

                    var keyRect = new Rect(position.x, yPos, position.width * KeyColumnWidth,
                        EditorGUIUtility.singleLineHeight);
                    var valueRect = new Rect(position.x + position.width * KeyColumnWidth, yPos,
                        position.width * ValueColumnWidth - ButtonWidth, rowHeight);
                    var removeButtonRect =
                        new Rect(
                            position.x + position.width * KeyColumnWidth + position.width * ValueColumnWidth -
                            ButtonWidth, yPos, ButtonWidth, EditorGUIUtility.singleLineHeight);

                    EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
                    EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

                    if (GUI.Button(removeButtonRect, "-")) {
                        pairsProperty.DeleteArrayElementAtIndex(i);
                    }

                    yPos += rowHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                var addButtonRect = new Rect(position.x, yPos, position.width, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(addButtonRect, "Add")) {
                    pairsProperty.arraySize++;
                    var newPairProperty = pairsProperty.GetArrayElementAtIndex(pairsProperty.arraySize - 1);
                    var keyProperty = newPairProperty.FindPropertyRelative("Key");
                    var valueProperty = newPairProperty.FindPropertyRelative("Value");

                    if (keyProperty.propertyType == SerializedPropertyType.ObjectReference) {
                        keyProperty.objectReferenceValue = null;
                    }

                    if (valueProperty.propertyType == SerializedPropertyType.ObjectReference) {
                        valueProperty.objectReferenceValue = null;
                    }

                    missingKeyPairProperty.boolValue = true;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            var pairsProperty = property.FindPropertyRelative("inspectorReservedPairs");
            float totalHeight = EditorGUIUtility.singleLineHeight; // Header height

            for (int i = 0; i < pairsProperty.arraySize; i++) {
                var pairProperty = pairsProperty.GetArrayElementAtIndex(i);
                var valueProperty = pairProperty.FindPropertyRelative("Value");

                // Get the actual height of the value using its property drawer
                float valueHeight = EditorGUI.GetPropertyHeight(valueProperty, true);
                totalHeight += Mathf.Max(EditorGUIUtility.singleLineHeight, valueHeight) +
                               EditorGUIUtility.standardVerticalSpacing;
            }

            // Add button height
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return totalHeight;
        }
    }
}
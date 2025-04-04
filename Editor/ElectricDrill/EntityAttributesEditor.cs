using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Attributes;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(EntityAttributes))]
    public class EntityAttributesEditor : Editor
    {
        SerializedProperty useClassBaseAttributes;
        SerializedProperty fixedBaseAttributeSet;

        void OnEnable()
        {
            useClassBaseAttributes = serializedObject.FindProperty("useClassBaseAttributes");
            fixedBaseAttributeSet = serializedObject.FindProperty("fixedBaseAttributeSet");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the default inspector fields
            DrawDefaultInspector();

            EntityAttributes entityAttributes = (EntityAttributes)target;
            AttributePointsTracker tracker = entityAttributes.AttrPointsTracker;

            if (tracker == null)
            {
                EditorGUILayout.HelpBox("AttributePointsTracker is not assigned.", MessageType.Warning);
                return;
            }

            // Calculate the width for the longest attribute name
            float minLabelOffset = 30;
            float maxLabelWidth = minLabelOffset;
            var fixedBaseAttributes = entityAttributes.fixedBaseAttributes;
            var attributes = new List<Attribute>(tracker.SpentAttributesKeys);
            attributes.AddRange(fixedBaseAttributes.Keys);

            foreach (var attribute in attributes)
            {
                float labelWidth = GUI.skin.label.CalcSize(new GUIContent(attribute.name)).x + minLabelOffset;
                if (labelWidth > maxLabelWidth)
                {
                    maxLabelWidth = labelWidth;
                }
            }

            // POINTS TRACKER
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Attribute Points Tracker", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Available Points: " + tracker.Available);

            // Collect keys in a separate list
            List<Attribute> spentAttributes = new List<Attribute>(tracker.SpentAttributesKeys);

            foreach (var attribute in spentAttributes)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(attribute.name, GUILayout.Width(maxLabelWidth));
                long spentPoints = tracker.GetSpentOn(attribute);
                long newSpentPoints = EditorGUILayout.LongField(spentPoints, GUILayout.Width(50));
                if (newSpentPoints != spentPoints)
                {
                    Undo.RecordObject(entityAttributes, "Modify Spent Attribute Points");
                    entityAttributes.SpendOn(attribute, (int)(newSpentPoints - spentPoints));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            
            // Check if useClassBaseAttributes is true
            EditorGUILayout.PropertyField(useClassBaseAttributes, new GUIContent(
                "Use Class Base Attributes",
                "If true, base attributes are derived from the attached IClassSource component. If false, uses the fixedBaseAttributeSet and fixedBaseAttributes values."));

            if (useClassBaseAttributes.boolValue == false)
            {
                // FIXED BASE ATTRIBUTES
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Fixed Base Attributes", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                // Draw fixedBaseAttributeAttrSet field with custom label
                EditorGUILayout.PropertyField(fixedBaseAttributeSet, new GUIContent("Attributes Set"));

                // Collect keys in a separate list
                List<Attribute> fixedBaseKeys = new List<Attribute>(fixedBaseAttributes.Keys);

                foreach (var attribute in fixedBaseKeys)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(attribute.name, GUILayout.Width(maxLabelWidth));
                    long value = fixedBaseAttributes[attribute];
                    long newValue = EditorGUILayout.LongField(value, GUILayout.Width(50));
                    if (newValue != value)
                    {
                        Undo.RecordObject(entityAttributes, "Modify Fixed Base Attribute");
                        fixedBaseAttributes[attribute] = newValue;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
            
            if (GUI.changed) {
                EditorUtility.SetDirty(entityAttributes);
            }
        }
        
        // UTILS
#if UNITY_EDITOR
        static EntityAttributesEditor()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }
        
        private static void OnSelectionChanged() {
            if (Selection.activeObject is GameObject selectedObject && selectedObject.TryGetComponent<EntityAttributes>(out var entityAttributes)) {
                entityAttributes.OnValidate();
            }
        }
#endif
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [CustomEditor(typeof(EntityCharacteristics))]
    public class EntityCharacteristicsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the default inspector fields
            DrawDefaultInspector();

            EntityCharacteristics entityCharacteristics = (EntityCharacteristics)target;
            CharacteristicPointsTracker tracker = entityCharacteristics.CharPointsTracker;

            if (tracker == null)
            {
                EditorGUILayout.HelpBox("CharacteristicPointsTracker is not assigned.", MessageType.Warning);
                return;
            }

            // Calculate the width for the longest characteristic name
            float minLabelOffset = 30;
            float maxLabelWidth = minLabelOffset;
            var fixedBaseCharacteristics = entityCharacteristics.fixedBaseCharacteristics;
            var characteristics = new List<Characteristic>(tracker.SpentCharacteristicsKeys);
            characteristics.AddRange(fixedBaseCharacteristics.Keys);

            foreach (var characteristic in characteristics)
            {
                float labelWidth = GUI.skin.label.CalcSize(new GUIContent(characteristic.name)).x + minLabelOffset;
                if (labelWidth > maxLabelWidth)
                {
                    maxLabelWidth = labelWidth;
                }
            }

            // POINTS TRACKER
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Characteristic Points Tracker", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("Available Points: " + tracker.Available);

            // Collect keys in a separate list
            List<Characteristic> spentCharacteristics = new List<Characteristic>(tracker.SpentCharacteristicsKeys);

            foreach (var characteristic in spentCharacteristics)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(characteristic.name, GUILayout.Width(maxLabelWidth));
                long spentPoints = tracker.GetSpentOn(characteristic);
                long newSpentPoints = EditorGUILayout.LongField(spentPoints, GUILayout.Width(50));
                if (newSpentPoints != spentPoints)
                {
                    Undo.RecordObject(entityCharacteristics, "Modify Spent Characteristic Points");
                    tracker.SpendOn(characteristic, (int)(newSpentPoints - spentPoints));
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            
            // Check if useClassBaseCharacteristics is true
            SerializedProperty useClassBaseCharacteristicsProp = serializedObject.FindProperty("useClassBaseCharacteristics");
            EditorGUILayout.PropertyField(useClassBaseCharacteristicsProp, new GUIContent("Use Class' Base Characteristics"));
            if (useClassBaseCharacteristicsProp.boolValue == false)
            {
                // FIXED BASE CHARACTERISTICS
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Fixed Base Characteristics", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                // Draw fixedBaseCharacteristicCharSet field with custom label
                SerializedProperty fixedBaseCharacteristicCharSetProp = serializedObject.FindProperty("fixedBaseCharacteristicCharSet");
                EditorGUILayout.PropertyField(fixedBaseCharacteristicCharSetProp, new GUIContent("Characteristics Set"));

                // Collect keys in a separate list
                List<Characteristic> fixedBaseKeys = new List<Characteristic>(fixedBaseCharacteristics.Keys);

                foreach (var characteristic in fixedBaseKeys)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(characteristic.name, GUILayout.Width(maxLabelWidth));
                    long value = fixedBaseCharacteristics[characteristic];
                    long newValue = EditorGUILayout.LongField(value, GUILayout.Width(50));
                    if (newValue != value)
                    {
                        Undo.RecordObject(entityCharacteristics, "Modify Fixed Base Characteristic");
                        fixedBaseCharacteristics[characteristic] = newValue;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
            
            if (GUI.changed) {
                EditorUtility.SetDirty(entityCharacteristics);
            }
        }
    }
}
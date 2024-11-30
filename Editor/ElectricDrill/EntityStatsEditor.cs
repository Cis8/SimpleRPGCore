using ElectricDrill.SimpleRpgCore.Stats;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace ElectricDrill.SimpleRpgCore
{
    [CustomEditor(typeof(EntityStats))]
    public class EntityStatsEditor : Editor
    {
        SerializedProperty useClassBaseStats;
        SerializedProperty fixedBaseStatsStatSet;
        SerializedProperty fixedBaseStats;
        SerializedProperty onStatChanged;

        void OnEnable()
        {
            useClassBaseStats = serializedObject.FindProperty("_useClassBaseStats");
            fixedBaseStatsStatSet = serializedObject.FindProperty("fixedBaseStatsStatSet");
            fixedBaseStats = serializedObject.FindProperty("_fixedBaseStats");
            onStatChanged = serializedObject.FindProperty("_onStatChanged");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the _useClassBaseStats property
            EditorGUILayout.PropertyField(useClassBaseStats);

            // Conditionally hide fixedBaseStatsStatSet and _fixedBaseStats based on _useClassBaseStats
            if (!useClassBaseStats.boolValue)
            {
                EditorGUILayout.PropertyField(fixedBaseStatsStatSet);

                // Draw _fixedBaseStats as editable long fields
                EntityStats entityStats = (EntityStats)target;
                EditorGUILayout.LabelField("Fixed Base Stats", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                // Collect keys in a list to avoid modifying the collection while iterating
                List<Stat> keys = new List<Stat>(entityStats.FixedBaseStatsKeys);
                foreach (var stat in keys)
                {
                    long value = entityStats._fixedBaseStats[stat];
                    long newValue = EditorGUILayout.LongField(stat.name, value);
                    if (newValue != value)
                    {
                        Undo.RecordObject(entityStats, "Modify Fixed Base Stat");
                        entityStats._fixedBaseStats[stat] = newValue;
                        EditorUtility.SetDirty(entityStats);
                    }
                }
                EditorGUI.indentLevel--;
            }

            // Draw the _onStatChanged property
            EditorGUILayout.PropertyField(onStatChanged);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
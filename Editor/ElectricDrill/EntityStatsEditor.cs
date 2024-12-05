using ElectricDrill.SimpleRpgCore.Stats;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
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
            
            DrawDefaultInspector();

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
                
                // get the non-null keys from the dictionary
                List<Stat> nonNullKeys = entityStats.FixedBaseStatsKeys.Where(k => 
                    k != null && k)
                    .ToList();
                Dictionary<Stat, long> newFixedBaseStats = new Dictionary<Stat, long>();
                foreach (var stat in nonNullKeys)
                {
                    long value = entityStats._fixedBaseStats[stat];
                    long newValue = EditorGUILayout.LongField(stat.name, value);
                    if (newValue != value)
                    {
                        newFixedBaseStats[stat] = newValue;
                    }
                    else {
                        newFixedBaseStats[stat] = value;
                    }
                }
                entityStats._fixedBaseStats = newFixedBaseStats;
                
                EditorGUI.indentLevel--;
            }

            // Draw the _onStatChanged property
            EditorGUILayout.PropertyField(onStatChanged);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
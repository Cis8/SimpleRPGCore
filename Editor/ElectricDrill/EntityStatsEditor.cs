using ElectricDrill.SimpleRpgCore.Stats;
using static ElectricDrill.SimpleRpgCore.CstmEditor.InspectorTypography;
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
            useClassBaseStats = serializedObject.FindProperty("useBaseStatsFromClass");
            fixedBaseStatsStatSet = serializedObject.FindProperty("fixedBaseStatsStatSet");
            fixedBaseStats = serializedObject.FindProperty("_fixedBaseStats");
            onStatChanged = serializedObject.FindProperty("_onStatChanged");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            // Draw the _useClassBaseStats property
            RequiredProperty(useClassBaseStats,
                "Use Base Stats From Class",
                "If true, base stats are derived from the attached IClassSource component. If false, uses the fixedBaseStatsStatSet and fixedBaseStats values.");
            
            // Conditionally hide fixedBaseStatsStatSet and _fixedBaseStats based on useBaseStatsFromClass
            if (!useClassBaseStats.boolValue)
            {
                RequiredProperty(fixedBaseStatsStatSet,
                    "Fixed Base Stats Set",
                    "The StatSet that defines which stats can be modified in the Fixed Base Stats section below.");

                // Draw _fixedBaseStats as editable long fields
                EntityStats entityStats = (EntityStats)target;
                EditorGUILayout.LabelField("Fixed Base Stats", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                // get the non-null keys from the dictionary
                List<Stat> nonNullKeys = entityStats.FixedBaseStatsKeys.Where(k =>
                        k != null && k)
                    .ToList();
                foreach (var stat in nonNullKeys)
                {
                    long value = entityStats.FixedBaseStats[stat];
                    long newValue = EditorGUILayout.LongField(stat.name, value);
                    if (newValue != value)
                    {
                        entityStats.FixedBaseStats[stat] = newValue;
                        EditorUtility.SetDirty(target);
                    }
                }
                
                EditorGUI.indentLevel--;
            }

            // Draw the _onStatChanged property
            EditorGUILayout.PropertyField(onStatChanged, new GUIContent(
                "On Stat Changed",
                "Event raised when any stat value changes. Provides the old and new values of the changed stat."));

            serializedObject.ApplyModifiedProperties();
        }
        
        // UTILS
#if UNITY_EDITOR
        static EntityStatsEditor()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }
        
        private static void OnSelectionChanged() {
            if (Selection.activeObject is GameObject selectedObject && selectedObject.TryGetComponent<EntityStats>(out var entityStats)) {
                entityStats.OnValidate();
            }
        }
#endif
    }
}
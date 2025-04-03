using UnityEditor;
using UnityEngine;
using ElectricDrill.SimpleRpgCore;
using static ElectricDrill.SimpleRpgCore.CstmEditor.InspectorTypography;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(EntityCore))]
    public class EntityCoreEditor : Editor
    {
        SerializedProperty level;
        SerializedProperty spawnedEntityEvent;

        void OnEnable()
        {
            level = serializedObject.FindProperty("_level");
            spawnedEntityEvent = serializedObject.FindProperty("spawnedEntityEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((EntityCore)target), typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Level and experience", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(level);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            RequiredProperty(spawnedEntityEvent,
                "Spawned Entity Event",
                "Event raised when this entity is spawned in the game.");

            serializedObject.ApplyModifiedProperties();
        }
    }
}

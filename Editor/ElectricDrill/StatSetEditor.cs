using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    /*[CustomEditor(typeof(StatSet))]
    public class StatSetEditor : Editor
    {
        private SerializedProperty _statsProperty;

        private void OnEnable()
        {
            _statsProperty = serializedObject.FindProperty("_stats");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);

            for (int i = 0; i < _statsProperty.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(_statsProperty.GetArrayElementAtIndex(i), GUIContent.none);
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _statsProperty.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Add Stat"))
            {
                _statsProperty.InsertArrayElementAtIndex(_statsProperty.arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }*/
    }
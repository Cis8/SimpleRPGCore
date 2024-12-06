using ElectricDrill.SimpleRpgCore.Health;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(DmgType))]
    public class DmgTypeEditor : Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            DmgType dmgType = (DmgType)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("dmgReducedBy"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dmgReductionFn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defensiveStatPiercedBy"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defReductionFn"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ignoresBarrier"));

            if (dmgType.ReducedBy != null && dmgType.DmgReductionFn != null) {
                if (GUILayout.Button("Make a Damage Reduction Simulation")) {
                    DmgReductionGraphWindow.ShowWindow(dmgType);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
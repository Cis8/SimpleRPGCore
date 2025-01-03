using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Stats;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(Class))]
    public class ClassEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Class classTarget = (Class)target;
    
            DrawDefaultInspector();
    
            // Display Stat Growth Functions
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Stat Growth Formulas", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            var statKeys = new List<Stat>(classTarget._statGrowthFormulas.Keys);
            for (int i = 0; i < statKeys.Count; i++)
            {
                var key = statKeys[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(key.name, GUILayout.Width(150));
                classTarget._statGrowthFormulas[key] = (GrowthFormula)EditorGUILayout.ObjectField(classTarget._statGrowthFormulas[key], typeof(GrowthFormula), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
    
            // Display Attribute Growth Formulas
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Attribute Growth Formulas", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            var attributeKeys = new List<Attribute>(classTarget.attributeGrowthFormulas.Keys);
            for (int i = 0; i < attributeKeys.Count; i++)
            {
                var key = attributeKeys[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(key.name, GUILayout.Width(150));
                classTarget.attributeGrowthFormulas[key] = (GrowthFormula)EditorGUILayout.ObjectField(classTarget.attributeGrowthFormulas[key], typeof(GrowthFormula), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
    
            if (GUI.changed)
            {
                EditorUtility.SetDirty(classTarget);
            }
        }
    }    
}

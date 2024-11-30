using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    [CustomEditor(typeof(EntityHealth))]
    public class EntityHealthEditor : Editor
    {
        SerializedProperty healthCanBeNegative;
        SerializedProperty deathThreshold;
        SerializedProperty useClassMaxHp;
        SerializedProperty maxHp;
        SerializedProperty healAmountModifier;
        SerializedProperty preDmgInfoEvent;
        SerializedProperty takenDmgInfoEvent;
        SerializedProperty gainedHealthEvent;
        SerializedProperty lostHealthEvent;
        SerializedProperty entityDiedEvent;
        SerializedProperty preHealEvent;
        SerializedProperty entityHealedEvent;

        void OnEnable()
        {
            healthCanBeNegative = serializedObject.FindProperty("healthCanBeNegative");
            deathThreshold = serializedObject.FindProperty("deathThreshold");
            useClassMaxHp = serializedObject.FindProperty("useClassMaxHp");
            maxHp = serializedObject.FindProperty("maxHp");
            healAmountModifier = serializedObject.FindProperty("healAmountModifier");
            preDmgInfoEvent = serializedObject.FindProperty("preDmgInfoEvent");
            takenDmgInfoEvent = serializedObject.FindProperty("takenDmgInfoEvent");
            gainedHealthEvent = serializedObject.FindProperty("gainedHealthEvent");
            lostHealthEvent = serializedObject.FindProperty("lostHealthEvent");
            entityDiedEvent = serializedObject.FindProperty("entityDiedEvent");
            preHealEvent = serializedObject.FindProperty("preHealEvent");
            entityHealedEvent = serializedObject.FindProperty("entityHealedEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw the healthCanBeNegative property
            EditorGUILayout.PropertyField(healthCanBeNegative);

            // Conditionally hide deathThreshold based on healthCanBeNegative
            if (healthCanBeNegative.boolValue)
            {
                EditorGUILayout.PropertyField(deathThreshold);
            }

            // Draw the useClassMaxHp property
            EditorGUILayout.PropertyField(useClassMaxHp);

            // Conditionally draw maxHp based on useClassMaxHp
            if (useClassMaxHp.boolValue)
            {
                DrawMaxHpProperty(maxHp);
            }
            else
            {
                EditorGUILayout.PropertyField(maxHp);
            }

            // Draw the healAmountModifier property with a label
            EditorGUILayout.PropertyField(healAmountModifier, new GUIContent("(o) Heal Amount Modifier"));

            // Group the game events in a foldout group
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(preDmgInfoEvent);
            EditorGUILayout.PropertyField(takenDmgInfoEvent);
            EditorGUILayout.PropertyField(gainedHealthEvent);
            EditorGUILayout.PropertyField(lostHealthEvent);
            EditorGUILayout.PropertyField(entityDiedEvent);
            EditorGUILayout.PropertyField(preHealEvent);
            EditorGUILayout.PropertyField(entityHealedEvent);
            EditorGUI.indentLevel--;

            EditorGUILayout.HelpBox("(o): optional", MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawMaxHpProperty(SerializedProperty property)
        {
            var useConstant = property.FindPropertyRelative("UseConstant");
            var constantValue = property.FindPropertyRelative("ConstantValue");
            var variable = property.FindPropertyRelative("Variable");

            EditorGUILayout.BeginHorizontal();
            var position = EditorGUILayout.GetControlRect();

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), new GUIContent("Max Hp"));

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var toggleRect = new Rect(position.x, position.y, 20, position.height);
            var labelRect = new Rect(position.x + 25, position.y, 100, position.height);
            var valueRect = new Rect(position.x + 130, position.y, position.width - 130, position.height);

            useConstant.boolValue = EditorGUI.Toggle(toggleRect, useConstant.boolValue);
            EditorGUI.LabelField(labelRect, "Use Constant");

            if (useConstant.boolValue) {
                valueRect.position = new Vector2(valueRect.position.x + 2, valueRect.position.y);
                EditorGUI.LabelField(valueRect, constantValue.longValue.ToString());
            }
            else
            {
                EditorGUI.PropertyField(valueRect, variable, GUIContent.none);
            }

            EditorGUI.indentLevel = indent;
            EditorGUILayout.EndHorizontal();
        }
    }
}
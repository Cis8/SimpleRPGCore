using System;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor {
    [CustomPropertyDrawer(typeof(EntityLevel))]
    public class EntityLevelDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.standardVerticalSpacing;

            var props = new[] {
                property.FindPropertyRelative("_level"),
                property.FindPropertyRelative("_maxLevel"),
                property.FindPropertyRelative("_experienceGrowthFormula"),
                property.FindPropertyRelative("experienceGainedModifierStat"),
                property.FindPropertyRelative("_onLevelUpEditor"),
                property.FindPropertyRelative("currentTotalExperience")
            };

            foreach (var prop in props)
            {
                height += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float yPos = position.y;

            // Get properties
            SerializedProperty levelProp = property.FindPropertyRelative("_level");
            SerializedProperty maxLevelProp = property.FindPropertyRelative("_maxLevel");
            SerializedProperty experienceFormulaProp = property.FindPropertyRelative("_experienceGrowthFormula");
            SerializedProperty expGainModifierProp = property.FindPropertyRelative("experienceGainedModifierStat");
            SerializedProperty onLevelUpProp = property.FindPropertyRelative("_onLevelUpEditor");
            SerializedProperty currentTotalExperienceProp = property.FindPropertyRelative("currentTotalExperience");

            // Draw each property
            var props = new[] {
                (levelProp, "Level"),
                (currentTotalExperienceProp, "Current Total Experience"),
                (maxLevelProp, "Max Level"),
                (experienceFormulaProp, "Experience Formula"),
                (expGainModifierProp, "Experience Gain Modifier"),
                (onLevelUpProp, "On Level Up")
            };

            foreach (var (prop, lbl) in props)
            {
                var propHeight = EditorGUI.GetPropertyHeight(prop, true);
                var propRect = new Rect(position.x, yPos, position.width, propHeight);

                EditorGUI.PropertyField(propRect, prop, new GUIContent(lbl), true);
                yPos += propHeight + EditorGUIUtility.standardVerticalSpacing;
            }
            
            // Set isReadOnly to true for currentTotalExperience
            currentTotalExperienceProp.FindPropertyRelative("isReadOnly").boolValue = true;

            // Update experience when level changes
            if (GUI.changed && levelProp.hasMultipleDifferentValues == false)
            {
                GrowthFormula growthFormula = experienceFormulaProp.objectReferenceValue as GrowthFormula;
                if (growthFormula != null)
                {
                    long currentTotalExperience = GetLongRefValue(currentTotalExperienceProp);
                    
                    int levelFromCurrentExp;
                    if (currentTotalExperience < growthFormula.GrowthFoValues[0])
                        levelFromCurrentExp = 1;
                    else
                        levelFromCurrentExp = Array.FindIndex(growthFormula.GrowthFoValues, v => v >= currentTotalExperience) + 2;
                    
                    int levelFromField = GetIntRefValue(levelProp);

                    if (levelFromCurrentExp != levelFromField) {
                        if (levelFromField == 1)
                        {
                            SetLongRefValue(currentTotalExperienceProp, 0);
                        }
                        else {
                            SetLongRefValue(currentTotalExperienceProp, growthFormula.GetGrowthValue(levelFromField - 1));
                        }
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        private int GetIntRefValue(SerializedProperty intRefProp)
        {
            SerializedProperty useConstantProp = intRefProp.FindPropertyRelative("UseConstant");
            SerializedProperty constantValueProp = intRefProp.FindPropertyRelative("ConstantValue");
            SerializedProperty variableProp = intRefProp.FindPropertyRelative("Variable");

            if (useConstantProp.boolValue)
            {
                return constantValueProp.intValue;
            }
            else if (variableProp.objectReferenceValue != null)
            {
                SerializedObject intVarObject = new SerializedObject(variableProp.objectReferenceValue);
                SerializedProperty valueProp = intVarObject.FindProperty("_value");
                return valueProp.intValue;
            }

            return 0;
        }

        private long GetLongRefValue(SerializedProperty longRefProp)
        {
            SerializedProperty useConstantProp = longRefProp.FindPropertyRelative("UseConstant");
            SerializedProperty constantValueProp = longRefProp.FindPropertyRelative("ConstantValue");
            SerializedProperty variableProp = longRefProp.FindPropertyRelative("Variable");

            if (useConstantProp.boolValue)
            {
                return constantValueProp.longValue;
            }
            else if (variableProp.objectReferenceValue != null)
            {
                SerializedObject longVarObject = new SerializedObject(variableProp.objectReferenceValue);
                SerializedProperty valueProp = longVarObject.FindProperty("_value");
                return valueProp.longValue;
            }

            return 0;
        }

        private void SetLongRefValue(SerializedProperty longRefProp, long value)
        {
            SerializedProperty useConstantProp = longRefProp.FindPropertyRelative("UseConstant");
            SerializedProperty constantValueProp = longRefProp.FindPropertyRelative("ConstantValue");
            SerializedProperty variableProp = longRefProp.FindPropertyRelative("Variable");

            if (useConstantProp.boolValue)
            {
                constantValueProp.longValue = value;
            }
            else if (variableProp.objectReferenceValue != null)
            {
                SerializedObject longVarObject = new SerializedObject(variableProp.objectReferenceValue);
                SerializedProperty valueProp = longVarObject.FindProperty("_value");
                valueProp.longValue = value;
                longVarObject.ApplyModifiedProperties();
            }
        }
    }
}
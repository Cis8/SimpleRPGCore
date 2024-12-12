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
                property.FindPropertyRelative("_onLevelUpEditor")
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

            // Draw each property
            var props = new[] {
                (levelProp, "Level"),
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

            // Update experience when level changes
            if (GUI.changed && levelProp.hasMultipleDifferentValues == false)
            {
                SerializedProperty currentTotalExperienceProp = property.FindPropertyRelative("_currentTotalExperience");
               
                GrowthFormula growthFormula = experienceFormulaProp.objectReferenceValue as GrowthFormula;
                if (growthFormula != null)
                {
                    int levelFromCurrentExp = Array.FindIndex(growthFormula.GrowthFoValues, v => v >= currentTotalExperienceProp.longValue);
                    int levelFromField = GetIntRefValue(levelProp);

                    if (levelFromCurrentExp != levelFromField) {
                        if (levelFromField == 1)
                        {
                            currentTotalExperienceProp.longValue = 0;
                        }
                        else {
                            currentTotalExperienceProp.longValue = growthFormula.GetGrowthValue(levelFromField - 1);
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
                SerializedProperty valueProp = intVarObject.FindProperty("Value");
                return valueProp.intValue;
            }

            return 0;
        }
    }
}
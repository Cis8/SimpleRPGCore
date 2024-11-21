using UnityEditor;
using UnityEngine;
using static ElectricDrill.SimpleRpgCore.Events.GameEventGenerator;

[CustomPropertyDrawer(typeof(EventParameter))]
public class EventParameterDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var parameterType = property.FindPropertyRelative("parameterType");
        var nativeType = property.FindPropertyRelative("nativeType");
        var monoScript = property.FindPropertyRelative("monoScript");

        Rect typeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect valueRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(typeRect, parameterType);

        if ((EventParameter.ParameterType)parameterType.enumValueIndex == EventParameter.ParameterType.Native)
        {
            EditorGUI.PropertyField(valueRect, nativeType);
        }
        else if ((EventParameter.ParameterType)parameterType.enumValueIndex == EventParameter.ParameterType.MonoScript)
        {
            EditorGUI.PropertyField(valueRect, monoScript);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2 + 2;
    }
}
using UnityEditor;
using static ElectricDrill.SimpleRpgCore.CstmEditor.InspectorTypography;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(EntityClass))]
    public class EntityClassEditor : Editor
    {
        private SerializedProperty classProperty;

        private void OnEnable()
        {
            classProperty = serializedObject.FindProperty("_class");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawDefaultInspector();

            RequiredProperty(classProperty,
                "Class",
                "The Class scriptable object that defines the entity's class");

            serializedObject.ApplyModifiedProperties();
        }
    }
}
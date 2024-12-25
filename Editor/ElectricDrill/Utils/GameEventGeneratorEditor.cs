using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(Events.GameEventGenerator))]
    public class GameEventGeneratorEditor : Editor
    {
        private GUIStyle boxStyle;
        private bool showWarning = false;
        private List<string> ungeneratedEvents = new List<string>();

        public override void OnInspectorGUI()
        {
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.border = new RectOffset(2, 2, 2, 2);
            boxStyle.margin = new RectOffset(10, 10, 10, 10);
            boxStyle.padding = new RectOffset(10, 10, 10, 10);

            Events.GameEventGenerator generator =
                (Events.GameEventGenerator)target;

            serializedObject.Update();

            SerializedProperty eventsToGenerate = serializedObject.FindProperty("eventsToGenerate");
            for (int i = 0; i < eventsToGenerate.arraySize; i++)
            {
                SerializedProperty gameEvent = eventsToGenerate.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical(boxStyle);
                EditorGUI.BeginChangeCheck();
                string oldEventName = gameEvent.FindPropertyRelative("eventName").stringValue;
                EditorGUILayout.PropertyField(gameEvent.FindPropertyRelative("eventName"));
                if (EditorGUI.EndChangeCheck() && showWarning)
                {
                    string newEventName = gameEvent.FindPropertyRelative("eventName").stringValue;
                    int index = ungeneratedEvents.IndexOf(oldEventName);
                    if (index != -1)
                    {
                        ungeneratedEvents[index] = newEventName;
                    }
                }

                EditorGUILayout.LabelField("Documentation");
                var documentationProperty = gameEvent.FindPropertyRelative("documentation");
                documentationProperty.stringValue = EditorGUILayout.TextArea(documentationProperty.stringValue, GUILayout.Height(60));
                EditorGUILayout.PropertyField(gameEvent.FindPropertyRelative("parameters"), true);

                if (GUILayout.Button("Remove Event"))
                {
                    string eventName = gameEvent.FindPropertyRelative("eventName").stringValue;
                    int parameterCount = gameEvent.FindPropertyRelative("parameters").arraySize;

                    // Check if the event has been generated before attempting to delete files
                    if (gameEvent.FindPropertyRelative("isGenerated").boolValue)
                    {
                        if (EditorUtility.DisplayDialog("Confirm Deletion",
                                "Are you sure you want to delete this event and its generated files?", "Yes", "No"))
                        {
                            generator.RemoveGeneratedEventFiles(eventName, parameterCount);

                            eventsToGenerate.DeleteArrayElementAtIndex(i);
                            ungeneratedEvents.Remove(eventName);
                            if (ungeneratedEvents.Count <= 0)
                            {
                                showWarning = false;
                            }
                        }
                    }
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("menuBasePath"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseSaveLocation"));

            if (GUILayout.Button("Add New Event"))
            {
                eventsToGenerate.InsertArrayElementAtIndex(eventsToGenerate.arraySize);
                showWarning = true;
                SerializedProperty newEvent = eventsToGenerate.GetArrayElementAtIndex(eventsToGenerate.arraySize - 1);
                newEvent.FindPropertyRelative("eventName").stringValue = "New Game Event";
                string newEventName = newEvent.FindPropertyRelative("eventName").stringValue;
                ungeneratedEvents.Add(newEventName);
            }

            if (showWarning && ungeneratedEvents.Count > 0)
            {
                EditorGUILayout.HelpBox(
                    "The following events have been added but not generated yet:\n" + string.Join("\n", ungeneratedEvents),
                    MessageType.Warning);
            }

            // Add the informational label here
            EditorGUILayout.HelpBox(
                "Pressing the \"Generate Game Events\" button will corrupt the instances of the GameEvents generated" +
                " through this GameEventGenerator if the respective GameEvent has been renamed. Changing the save path will " +
                "corrupt all instances of all GameEvents created with this GameEventGenerator.",
                MessageType.Info);

            if (GUILayout.Button("Generate Game Events"))
            {
                generator.GenerateGameEvents();
                showWarning = false;
                ungeneratedEvents.Clear();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}

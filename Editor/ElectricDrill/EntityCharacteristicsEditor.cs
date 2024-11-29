using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [CustomEditor(typeof(EntityCharacteristics))]
    public class EntityCharacteristicsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            EntityCharacteristics entityCharacteristics = (EntityCharacteristics)target;
            CharacteristicPointsTracker tracker = entityCharacteristics.CharPointsTracker;

            if (tracker == null)
            {
                EditorGUILayout.HelpBox("CharacteristicPointsTracker is not assigned.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Characteristic Points Tracker", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Available Points: " + tracker.Available);

            // Collect keys in a separate list
            List<Characteristic> characteristics = new List<Characteristic>(tracker.SpentCharacteristics);

            foreach (var characteristic in characteristics)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(characteristic.name, GUILayout.Width(150));
                long spentPoints = tracker.GetSpentOn(characteristic);
                long newSpentPoints = EditorGUILayout.LongField(spentPoints, GUILayout.Width(50));
                if (newSpentPoints != spentPoints)
                {
                    Undo.RecordObject(entityCharacteristics, "Modify Spent Characteristic Points");
                    tracker.SpendOn(characteristic, (int)(newSpentPoints - spentPoints));
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUI.changed) {
                EditorUtility.SetDirty(entityCharacteristics);
            }
        }
    }
}
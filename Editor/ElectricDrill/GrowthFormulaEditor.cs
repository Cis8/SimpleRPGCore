using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    [CustomEditor(typeof(GrowthFormula))]
    public class GrowthFormulaEditor : Editor
    {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            GrowthFormula growthFormula = (GrowthFormula)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLevel"));

            // ===== Draw use constant at level 1 ====================================================
            SerializedProperty useConstantAtLvl1 = serializedObject.FindProperty("useConstantAtLvl1");
            EditorGUILayout.PropertyField(useConstantAtLvl1);

            if (useConstantAtLvl1.boolValue) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("constantAtLvl1"));
            }

            // ===== Draw level to growth formulas ====================================================
            SerializedProperty levelToGrowthFormulas = serializedObject.FindProperty("levelToGrowthFormulas");
            for (int i = 0; i < levelToGrowthFormulas.arraySize; i++) {
                SerializedProperty element = levelToGrowthFormulas.GetArrayElementAtIndex(i);
                int fromLevel = element.FindPropertyRelative("fromLevel").intValue;
                int toLevel = (i + 1 < levelToGrowthFormulas.arraySize)
                    ? levelToGrowthFormulas.GetArrayElementAtIndex(i + 1).FindPropertyRelative("fromLevel").intValue - 1
                    : growthFormula.MaxLevel.Value;

                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"From level {fromLevel} to level {toLevel}");
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(element.FindPropertyRelative("fromLevel"));
                EditorGUILayout.PropertyField(element.FindPropertyRelative("growthFormula"));
                EditorGUI.indentLevel--;
                if (GUILayout.Button("Remove")) {
                    levelToGrowthFormulas.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Level Growth Formula")) {
                levelToGrowthFormulas.InsertArrayElementAtIndex(levelToGrowthFormulas.arraySize);
            }

            // ===== Draw growth graph ===============================================================
            GUILayout.Space(20);
            GUILayout.Label("Growth Values for each level - graph", EditorStyles.boldLabel);
            DrawGrowthGraph(growthFormula);

            // ===== Draw growth values in two columns with borders ==================================
            GUILayout.Label("Growth Values for each level - table", EditorStyles.boldLabel);
            SerializedProperty growthFoValues = serializedObject.FindProperty("growthFoValues");
            int half = Mathf.CeilToInt(growthFoValues.arraySize / 2f);

            EditorGUILayout.BeginHorizontal();
            DrawTableColumn(growthFoValues, 0, half);
            DrawTableColumn(growthFoValues, half, growthFoValues.arraySize);
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawTableColumn(SerializedProperty growthFoValues, int start, int end) {
            EditorGUILayout.BeginVertical("box");
            for (int i = start; i < end; i++) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"lvl {i + 1}", GUILayout.Width(50));
                EditorGUILayout.LabelField(((long)growthFoValues.GetArrayElementAtIndex(i).doubleValue).ToString(),
                    GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private const int GraphWidth = 250;
        private const int GraphHeight = 300;

        private void DrawGrowthGraph(GrowthFormula growthFormula) {
            GUILayout.Space(20);

            float maxValue;
            if (growthFormula.GrowthFoValues.Length > 0) {
                maxValue = (float)growthFormula.GrowthFoValues.Max();
            }
            else {
                maxValue = 0;
            }
            int maxDigits = maxValue.ToString("N0").Length;
            int leftSpace = maxDigits * 7; // Calculate left space based on the number of digits

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(leftSpace); // Insert dynamic space to the left of the graph

            Rect graphRect = GUILayoutUtility.GetRect(GraphWidth, GraphHeight);
            Handles.DrawSolidRectangleWithOutline(graphRect, Color.black, Color.white);

            float stepX = graphRect.width / (growthFormula.GrowthFoValues.Length - 1);
            float stepY = graphRect.height / maxValue;

            Vector3 previousPoint;
            if (growthFormula.GrowthFoValues.Length > 0)
                previousPoint = new Vector3(graphRect.x, graphRect.yMax - (float)growthFormula.GrowthFoValues[0] * stepY);
            else
                previousPoint = new Vector3(graphRect.x, graphRect.yMax);

            for (int i = 1; i < growthFormula.GrowthFoValues.Length; i++) {
                Vector3 currentPoint = new Vector3(graphRect.x + i * stepX,
                    graphRect.yMax - (float)growthFormula.GrowthFoValues[i] * stepY);
                Handles.color = Color.green;
                Handles.DrawLine(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            // Draw x-axis labels (levels)
            int maxLabels = 10;
            int step = Mathf.Max(1, growthFormula.GrowthFoValues.Length / maxLabels);
            for (int i = 0; i < growthFormula.GrowthFoValues.Length; i += step) {
                Vector3 labelPosition = new Vector3(graphRect.x + i * stepX, graphRect.yMax + 10);
                Handles.Label(labelPosition, (i + 1).ToString());
            }

            // Ensure the last label is always shown
            Vector3 lastLabelPosition = new Vector3(graphRect.x + (growthFormula.GrowthFoValues.Length - 1) * stepX,
                graphRect.yMax + 10);
            Handles.Label(lastLabelPosition, growthFormula.GrowthFoValues.Length.ToString());

            // Draw y-axis labels (reference values)
            int numYLabels = 5;
            for (int i = 0; i <= numYLabels; i++) {
                float yValue = maxValue * i / numYLabels;
                Vector3 labelPosition = new Vector3(graphRect.x - leftSpace, graphRect.yMax - yValue * stepY);
                Handles.Label(labelPosition, yValue.ToString("N0"));
            }

            // Show values on hover and draw square
            Vector2 mousePosition = Event.current.mousePosition;
            if (graphRect.Contains(mousePosition)) {
                float mouseX = mousePosition.x - graphRect.x;
                float mouseY = graphRect.yMax - mousePosition.y;
                int index = Mathf.RoundToInt(mouseX / stepX);
                if (index >= 0 && index < growthFormula.GrowthFoValues.Length) {
                    float valueY = (float)growthFormula.GrowthFoValues[index];
                    Vector3 squarePosition = new Vector3(graphRect.x + index * stepX, graphRect.yMax - valueY * stepY);
                    Handles.color = Color.red;
                    Handles.DrawSolidDisc(squarePosition, Vector3.forward, 3);

                    // Draw label next to the square
                    string labelText = $"lvl: {index + 1}, value: {valueY:N0}";
                    Vector2 labelSize = GUI.skin.label.CalcSize(new GUIContent(labelText));
                    Vector3 labelPosition = new Vector3(squarePosition.x + 5, squarePosition.y);

                    // Adjust label position to the left if it exceeds the graph's width
                    if (squarePosition.x + labelSize.x > graphRect.xMax) {
                        labelPosition.x = squarePosition.x - labelSize.x - 5;
                    }

                    // Adjust label position if it exceeds the top or bottom of the graph
                    if (labelPosition.y - labelSize.y < graphRect.y) {
                        labelPosition.y = graphRect.y + labelSize.y;
                    }
                    else if (labelPosition.y > graphRect.yMax) {
                        labelPosition.y = graphRect.yMax;
                    }

                    Handles.Label(labelPosition, labelText);
                }

                Repaint();
            }

            GUILayout.Space(10);

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.HelpBox(
                "Hold the mouse for a moment on top of the graph to see the value at a certain level",
                MessageType.Info);

            GUILayout.Space(20);
        }
    }
}
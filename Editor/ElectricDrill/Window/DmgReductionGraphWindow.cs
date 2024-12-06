using System;
using ElectricDrill.SimpleRpgCore.Health;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    public class DmgReductionGraphWindow : EditorWindow
    {
        private DmgType dmgType;
        private Stat defensiveStat;
        private long minValue = 0;
        private long maxValue = 100;
        private long damageAmount = 100;

        public static void ShowWindow(DmgType dmgType) {
            var window = GetWindow<DmgReductionGraphWindow>("Damage Reduction Graph");
            window.dmgType = dmgType;
            window.defensiveStat = dmgType.ReducedBy;
            window.minValue = 0;
            window.maxValue = 100;
            window.damageAmount = 100;
        }

        private void OnGUI() {
            if (dmgType == null) {
                EditorGUILayout.HelpBox("No DmgType selected.", MessageType.Error);
                return;
            }

            // 200 fits the "Defensive Stat Min Value (set in )" string
            int labelWidth = 205 + defensiveStat.name.Length * 8;

            EditorGUILayout.BeginHorizontal();
            if (!defensiveStat.HasMinValue) {
                minValue = EditorGUILayout.LongField(minValue);
                EditorGUILayout.LabelField($"Defensive Stat Min Value", GUILayout.Width(labelWidth));
            }
            else {
                minValue = defensiveStat.MinValue;
                EditorGUILayout.LabelField($"Defensive Stat Min Value (set in {defensiveStat.name})",
                    GUILayout.Width(labelWidth));
                EditorGUILayout.LabelField(defensiveStat.MinValue.ToString());
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (!defensiveStat.HasMaxValue) {
                EditorGUILayout.LabelField("Defensive Stat Max Value", GUILayout.Width(labelWidth));
                maxValue = EditorGUILayout.LongField(maxValue);
            }
            else {
                maxValue = defensiveStat.MaxValue;
                EditorGUILayout.LabelField($"Defensive Stat Max Value (set in {defensiveStat.name})",
                    GUILayout.Width(labelWidth));
                EditorGUILayout.LabelField(defensiveStat.MaxValue.ToString());
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Damage Amount", GUILayout.Width(labelWidth));
            damageAmount = EditorGUILayout.LongField(damageAmount);
            EditorGUILayout.EndHorizontal();

            if (minValue >= maxValue) {
                EditorGUILayout.HelpBox("Min Value should be less than Max Value", MessageType.Error);
            }
            else {
                DrawDmgReductionGraph();
            }
        }

        private void DrawDmgReductionGraph() {
    GUILayout.Space(20);

    Rect graphRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.ExpandWidth(true),
        GUILayout.ExpandHeight(true));
    int leftSpace = 50;
    int rightSpace = 50;

    Handles.DrawSolidRectangleWithOutline(
        new Rect(graphRect.x + leftSpace, graphRect.y, graphRect.width - leftSpace - rightSpace,
            graphRect.height), Color.black, Color.white);

    float stepX = (graphRect.width - leftSpace - rightSpace) / (float)(maxValue - minValue);
    float maxY = damageAmount; // Use the damage amount as the max Y value
    float stepY = graphRect.height / maxY;

    Vector3 previousPoint = new Vector3(graphRect.x + leftSpace,
        graphRect.yMax - (float)dmgType.DmgReductionFn.ReducedDmg(damageAmount, minValue) * stepY);

    for (long i = minValue + 1; i <= maxValue; i++) {
        float reductionValue = (float)dmgType.DmgReductionFn.ReducedDmg(damageAmount, i);
        Vector3 currentPoint = new Vector3(graphRect.x + leftSpace + (i - minValue) * stepX,
            graphRect.yMax - reductionValue * stepY);
        Handles.color = Color.green;
        Handles.DrawLine(previousPoint, currentPoint);
        previousPoint = currentPoint;
    }

    // Draw x-axis labels (stat values)
    int maxLabels = 10;
    long step = Math.Max(1, (long)((maxValue - minValue) / (double)maxLabels));
    for (long i = minValue; i <= maxValue; i += step) {
        Vector3 labelPosition =
            new Vector3(graphRect.x + leftSpace + (i - minValue) * stepX, graphRect.yMax + 10);
        Handles.Label(labelPosition, i.ToString());
    }

    // Ensure the last label is always shown
    Vector3 lastLabelPosition =
        new Vector3(graphRect.x + leftSpace + (maxValue - minValue) * stepX, graphRect.yMax + 10);
    Handles.Label(lastLabelPosition, maxValue.ToString());

    // Draw y-axis labels (reduction values)
    int numYLabels = 5;
    for (int i = 0; i <= numYLabels; i++) {
        float yValue = maxY * i / numYLabels;
        Vector3 labelPosition = new Vector3(graphRect.x, graphRect.yMax - yValue * stepY);
        Handles.Label(labelPosition, yValue.ToString("N2"));
    }

    // Show values on hover and draw square
    Vector2 mousePosition = Event.current.mousePosition;
    if (graphRect.Contains(mousePosition)) {
        float mouseX = mousePosition.x - graphRect.x - leftSpace;
        float mouseY = graphRect.yMax - mousePosition.y;
        long index = Mathf.RoundToInt(mouseX / stepX) + minValue;
        if (index >= minValue && index <= maxValue) {
            float valueY = (float)dmgType.DmgReductionFn.ReducedDmg(damageAmount, index);
            Vector3 squarePosition = new Vector3(graphRect.x + leftSpace + (index - minValue) * stepX,
                graphRect.yMax - valueY * stepY);
            Handles.color = Color.red;
            Handles.DrawSolidDisc(squarePosition, Vector3.forward, 3);

            // Draw label next to the square
            string labelText =
                $"Stat: {index},\nTaken dmg: {valueY:N2},\nReduction: {damageAmount - valueY:N2}";
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
            else if (labelPosition.y + labelSize.y > graphRect.yMax) {
                labelPosition.y = graphRect.yMax - labelSize.y;
            }

            // Create a custom GUI style with a semi-transparent background
            GUIStyle backgroundStyle = new GUIStyle(GUI.skin.box);
            backgroundStyle.normal.background = Texture2D.whiteTexture;
            Color backgroundColor = new Color(0, 0, 0, 0.5f); // Set alpha to 50%
            GUI.backgroundColor = backgroundColor;

            // Draw background box
            Rect labelRect = new Rect(labelPosition.x, labelPosition.y - (labelSize.y / 2), labelSize.x,
                labelSize.y);
            GUI.Box(labelRect, GUIContent.none, backgroundStyle);

            // Draw the label
            Handles.Label(labelPosition, labelText);
        }

        Repaint();
    }

    GUILayout.Space(20);
    EditorGUILayout.HelpBox(
        "Hold the mouse for a moment on top of the graph to see the value at a certain stat level",
        MessageType.Info);
    GUILayout.Space(20);
}
    }
}
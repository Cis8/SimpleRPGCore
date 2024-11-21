using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore {
    [CreateAssetMenu(fileName = "New Growth Formula", menuName = "Simple RPG/Growth Formula")]
    public class GrowthFormula : ScriptableObject
    {
        [SerializeField] private bool useConstantAtLvl1 = true;
        [SerializeField] private long constantAtLvl1;
        [SerializeField] private List<LevelGrowthFormulaPair> levelToGrowthFormulas;
        [SerializeField] private IntVar maxLevel;
        [SerializeField] private long[] growthFoValues;
        public long[] GrowthFoValues => growthFoValues;

        public long GetGrowthValue(int level) {
            Assert.IsTrue(level <= maxLevel.Value, "Level is greater than max level");
            return growthFoValues[level - 1];
        }
        
        private void OnValidate() {
            // sort growthFormulas by FromLevel
            /*levelToGrowthFormulas.Sort((a, b) =>
                a.FromLevel.CompareTo(b.FromLevel));*/
            // or use:
            //levelToGrowthFormulas = levelToGrowthFormulas.OrderBy(lvToGrowthFo => lvToGrowthFo.FromLevel).ToList();
            
            if (levelToGrowthFormulas.Count >= 1) {
                // if growthFormulas count >= 1 and the first formula contains "PRV" keyword, then either the
                // useConstantAtLvl1 should be true or the formula should start at level 2
                var firstLvlToGrowthFo = levelToGrowthFormulas[0];
                if (firstLvlToGrowthFo.GrowthFormula.Contains(previousGrowthValueKeyword)) {
                    Assert.IsTrue(useConstantAtLvl1 || levelToGrowthFormulas[0].FromLevel == 2,
                        "First growth formula should start at level 2 if it contains PRV keyword");
                }
                
                // check that the first growth formula is starting from lvl 2 if useConstantAtLvl1 is true
                Assert.IsTrue(
                    (useConstantAtLvl1 && firstLvlToGrowthFo.FromLevel == 2) || !useConstantAtLvl1,
                    "First growth formula should start at level 2 since useConstantAtLvl1 is true. " +
                    "Otherwise, gaps will be left between level 1 and the first formula's StartFrom level");
                
                // check that the first growth formula is starting from lvl 1 if useConstantAtLvl1 is false
                Assert.IsTrue(
                    (!useConstantAtLvl1 && firstLvlToGrowthFo.FromLevel == 1) || useConstantAtLvl1,
                    "First growth formula should start at level 1 since Use Constant AtLvl 1 is false");
            }
            
            // check that growthFormulas are not overlapping
            for (var i = 1; i < levelToGrowthFormulas.Count; i++) { 
                Assert.IsTrue(levelToGrowthFormulas[i-1].FromLevel < levelToGrowthFormulas[i].FromLevel,
                    "Growth formulas are overlapping");
            }
            
            growthFoValues = new long[maxLevel];
            
            if (useConstantAtLvl1) {
                growthFoValues[0] = constantAtLvl1;
            }
            
            // evaluate growth formulas
            if (levelToGrowthFormulas.Count <= 0) return;
            var lvl = useConstantAtLvl1 ? 1 : 0;
            for (; lvl < maxLevel; lvl++) {
                var level = lvl + 1;
                var lvlRelatedGrowthFo = ReplaceKeywordsInExpression(level);
                ExpressionEvaluator.Evaluate(lvlRelatedGrowthFo, out double computedValue);
                growthFoValues[lvl] = (long) computedValue;
            }
        }

        private string ReplaceKeywordsInExpression(int level) {
            // as precondition, we have that growthFormulas are sorted by FromLevel
            var levelToFormula = levelToGrowthFormulas.FindLast(levFormPair =>
                levFormPair.FromLevel <= level);
            var growthFormula = levelToFormula.GrowthFormula;
            growthFormula = growthFormula
                .Replace(levelKeyword, level.ToString())
                .Replace(sumKeyword, growthFoValues.Take(level - 1).Sum().ToString());
            switch (level) {
                case 1:
                    return growthFormula;
                case 2:
                    growthFormula = growthFormula.Replace(previousGrowthValueKeyword, growthFoValues[level - 2].ToString());
                    return growthFormula;
            }

            growthFormula = growthFormula.Replace(secondPreviousGrowthValueKeyword, growthFoValues[level - 3].ToString());
            growthFormula = growthFormula.Replace(previousGrowthValueKeyword, growthFoValues[level - 2].ToString());
            return growthFormula;
        }
        
        [Serializable]
        private class LevelGrowthFormulaPair
        {
            [SerializeField] private int fromLevel = 2; // inclusive level since which this formula is to be used
            [SerializeField] private string growthFormula = "";
            
            public int FromLevel { get => fromLevel; set => fromLevel = value; }
            public string GrowthFormula { get => growthFormula; set => growthFormula = value; }
            
        }

        private const string levelKeyword = "LVL"; // keyword to be replaced with the level
        private const string previousGrowthValueKeyword = "PRV"; // keyword to be replaced with the previous growth value
        private const string secondPreviousGrowthValueKeyword = "2PRV"; // keyword to be replaced with the second previous growth value
        private const string sumKeyword = "SUM"; // keyword to be replaced with the sum of the previous growth values
        
    #if UNITY_EDITOR
        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable() {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged() {
            if (Selection.activeObject == this) {
                OnValidate();
            }
        }
    #endif
    }
}

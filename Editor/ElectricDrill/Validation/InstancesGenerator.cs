using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Scaling;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    public class InstancesGenerator
    {
        private const string RootFolder = "Assets/AssetsValidation/SimpleRPGCore/Validation";
        private static readonly string[] Classes = {
            "Warrior",
            "Mage",
            "Rogue"
        };
        private static readonly string[] Stats = {
            "Physical Attack",
            "Critical Chance",
            "Ability Power"
        };
        private static readonly string[] Attributes = {
            "Strength",
            "Dexterity",
            "Intelligence"
        };

        [MenuItem("Assets/Tools/Generate ScriptableObjects")]
        public static void GenerateScriptableObjects() {
            // Create folders
            CreateFolders();

            // Create Stats
            var statInstances = new Dictionary<string, Stat>();
            foreach (string statName in Stats) {
                Stat stat = ScriptableObject.CreateInstance<Stat>();
                stat.name = statName;
                AssetDatabase.CreateAsset(stat, $"{RootFolder}/Stats/{statName.Replace(" ", "")}.asset");
                statInstances[statName] = stat;
            }

            // Create Attributes
            var attributeInstances = new Dictionary<string, Attribute>();
            foreach (string attributeName in Attributes) {
                Attribute attribute = ScriptableObject.CreateInstance<Attribute>();
                attribute.name = attributeName;
                AssetDatabase.CreateAsset(attribute, $"{RootFolder}/Attributes/{attributeName.Replace(" ", "")}.asset");
                attributeInstances[attributeName] = attribute;
            }

            // Create Stat Set
            var statHashSet = new SerializableHashSet<Stat>();
            foreach (var stat in statInstances.Values) {
                statHashSet.Add(stat);
            }
            StatSet statSet = ScriptableObject.CreateInstance<StatSet>();
            statSet.name = "DefaultStatSet";
            statSet.SetStats(statHashSet);
            AssetDatabase.CreateAsset(statSet, $"{RootFolder}/StatSets/DefaultStatSet.asset");

            // Create Attribute Set
            var attributeHashSet = new SerializableHashSet<Attribute>();
            foreach (var attribute in attributeInstances.Values) {
                attributeHashSet.Add(attribute);
            }
            AttributeSet attributeSet = ScriptableObject.CreateInstance<AttributeSet>();
            attributeSet.name = "DefaultAttributeSet";
            attributeSet.SetAttributes(attributeHashSet);
            AssetDatabase.CreateAsset(attributeSet, $"{RootFolder}/AttributeSets/DefaultAttributeSet.asset");

            foreach (string className in Classes) {
                // Create Class
                Class characterClass = ScriptableObject.CreateInstance<Class>();
                characterClass.name = className;
                characterClass.StatSet = statSet;
                characterClass.AttributeSet = attributeSet;
                string classFolder = $"{RootFolder}/Classes/{characterClass.name}";
                AssetDatabase.CreateAsset(characterClass, $"{classFolder}/{characterClass.name}.asset");

                // Create Stat Growth Formulas
                foreach (var stat in statInstances) {
                    GrowthFormula growthFormula = ScriptableObject.CreateInstance<GrowthFormula>();
                    growthFormula.name = $"{stat.Key} Growth";
                    growthFormula.useConstantAtLvl1 = true;
                    growthFormula.constantAtLvl1 = 10;
                    growthFormula.levelToGrowthFormulas = new List<GrowthFormula.LevelGrowthFormulaPair> {
                        new GrowthFormula.LevelGrowthFormulaPair { FromLevel = 2, GrowthFormula = "PRV + 10" }
                    };
                    growthFormula.OnValidate();
                    AssetDatabase.CreateAsset(growthFormula, $"{classFolder}/StatGrowthFormulas/{growthFormula.name}.asset");
                    characterClass._statGrowthFormulas[stat.Value] = growthFormula;
                }

                // Create Attribute Growth Formulas
                foreach (var attributeToGf in attributeInstances) {
                    GrowthFormula growthFormula = ScriptableObject.CreateInstance<GrowthFormula>();
                    growthFormula.name = $"{attributeToGf.Key} Growth";
                    growthFormula.useConstantAtLvl1 = true;
                    growthFormula.constantAtLvl1 = 10;
                    growthFormula.levelToGrowthFormulas = new List<GrowthFormula.LevelGrowthFormulaPair> {
                        new GrowthFormula.LevelGrowthFormulaPair { FromLevel = 2, GrowthFormula = "PRV + 10" }
                    };
                    growthFormula.OnValidate();
                    AssetDatabase.CreateAsset(growthFormula, $"{classFolder}/AttributeGrowthFormulas/{growthFormula.name}.asset");
                    characterClass.attributeGrowthFormulas[attributeToGf.Value] = growthFormula;
                }
            }

            // Create Attribute Scaling Components
            CreateAttributeScalingComponents(attributeSet, statInstances, attributeInstances);

            // Save all assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("ScriptableObjects generated successfully!");
        }

        private static void CreateFolders() {
            string[] rootFolders = RootFolder.Split('/');
            string folderBuiltSoFar = rootFolders[0];
            foreach (string folder in rootFolders[1..]) {
                if (!AssetDatabase.IsValidFolder(folderBuiltSoFar + "/" + folder)) {
                    AssetDatabase.CreateFolder(folderBuiltSoFar, folder);
                }
                folderBuiltSoFar += "/" + folder;
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/Stats")) {
                AssetDatabase.CreateFolder($"{RootFolder}", "Stats");
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/Stats/AttrToStatScalings")) {
                AssetDatabase.CreateFolder($"{RootFolder}/Stats", "AttrToStatScalings");
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/Attributes")) {
                AssetDatabase.CreateFolder($"{RootFolder}", "Attributes");
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/StatSets")) {
                AssetDatabase.CreateFolder($"{RootFolder}", "StatSets");
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/AttributeSets")) {
                AssetDatabase.CreateFolder($"{RootFolder}", "AttributeSets");
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/Classes")) {
                AssetDatabase.CreateFolder($"{RootFolder}", "Classes");
            }

            foreach (string className in Classes) {
                string classFolder = $"{RootFolder}/Classes/{className}";
                if (!AssetDatabase.IsValidFolder(classFolder)) {
                    AssetDatabase.CreateFolder($"{RootFolder}/Classes", className);
                }

                if (!AssetDatabase.IsValidFolder($"{classFolder}/StatGrowthFormulas")) {
                    AssetDatabase.CreateFolder(classFolder, "StatGrowthFormulas");
                }

                if (!AssetDatabase.IsValidFolder($"{classFolder}/AttributeGrowthFormulas")) {
                    AssetDatabase.CreateFolder(classFolder, "AttributeGrowthFormulas");
                }
            }
        }

        private static void CreateAttributeScalingComponents(AttributeSet attributeSet, Dictionary<string, Stat> statInstances, Dictionary<string, Attribute> attributeInstances) {
            var scalingMappings = new Dictionary<string, string> {
                { "Ability Power", "Intelligence" },
                { "Physical Attack", "Strength" },
                { "Critical Chance", "Dexterity" }
            };

            foreach (var mapping in scalingMappings) {
                AttributesScalingComponent scalingComponent = ScriptableObject.CreateInstance<AttributesScalingComponent>();
                scalingComponent.name = $"{mapping.Key.Replace(" ", "")}AttrScaling";
                scalingComponent.SetSet(attributeSet);
                scalingComponent._scalingAttributeValues[attributeInstances[mapping.Value]] = 1.0;
                AssetDatabase.CreateAsset(scalingComponent, $"{RootFolder}/Stats/AttrToStatScalings/{scalingComponent.name}.asset");

                // Assign the scaling component to the corresponding stat
                statInstances[mapping.Key].AttributesScaling = scalingComponent;
            }
        }
    }
}
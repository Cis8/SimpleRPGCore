using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Characteristics;
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
        private static readonly string[] Characteristics = {
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

            // Create Characteristics
            var characteristicInstances = new Dictionary<string, Characteristic>();
            foreach (string characteristicName in Characteristics) {
                Characteristic characteristic = ScriptableObject.CreateInstance<Characteristic>();
                characteristic.name = characteristicName;
                AssetDatabase.CreateAsset(characteristic, $"{RootFolder}/Characteristics/{characteristicName.Replace(" ", "")}.asset");
                characteristicInstances[characteristicName] = characteristic;
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

            // Create Characteristic Set
            var characteristicHashSet = new SerializableHashSet<Characteristic>();
            foreach (var characteristic in characteristicInstances.Values) {
                characteristicHashSet.Add(characteristic);
            }
            CharacteristicSet characteristicSet = ScriptableObject.CreateInstance<CharacteristicSet>();
            characteristicSet.name = "DefaultCharacteristicSet";
            characteristicSet.SetCharacteristics(characteristicHashSet);
            AssetDatabase.CreateAsset(characteristicSet, $"{RootFolder}/CharacteristicSets/DefaultCharacteristicSet.asset");

            foreach (string className in Classes) {
                // Create Class
                Class characterClass = ScriptableObject.CreateInstance<Class>();
                characterClass.name = className;
                characterClass.StatSet = statSet;
                characterClass.CharacteristicSet = characteristicSet;
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

                // Create Characteristic Growth Formulas
                foreach (var characteristic in characteristicInstances) {
                    GrowthFormula growthFormula = ScriptableObject.CreateInstance<GrowthFormula>();
                    growthFormula.name = $"{characteristic.Key} Growth";
                    growthFormula.useConstantAtLvl1 = true;
                    growthFormula.constantAtLvl1 = 10;
                    growthFormula.levelToGrowthFormulas = new List<GrowthFormula.LevelGrowthFormulaPair> {
                        new GrowthFormula.LevelGrowthFormulaPair { FromLevel = 2, GrowthFormula = "PRV + 10" }
                    };
                    growthFormula.OnValidate();
                    AssetDatabase.CreateAsset(growthFormula, $"{classFolder}/CharacteristicGrowthFormulas/{growthFormula.name}.asset");
                    characterClass.characteristicGrowthFormulas[characteristic.Value] = growthFormula;
                }
            }

            // Create Characteristic Scaling Components
            CreateCharacteristicScalingComponents(characteristicSet, statInstances, characteristicInstances);

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

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/Stats/CharToStatScalings")) {
                AssetDatabase.CreateFolder($"{RootFolder}/Stats", "CharToStatScalings");
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/Characteristics")) {
                AssetDatabase.CreateFolder($"{RootFolder}", "Characteristics");
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/StatSets")) {
                AssetDatabase.CreateFolder($"{RootFolder}", "StatSets");
            }

            if (!AssetDatabase.IsValidFolder($"{RootFolder}/CharacteristicSets")) {
                AssetDatabase.CreateFolder($"{RootFolder}", "CharacteristicSets");
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

                if (!AssetDatabase.IsValidFolder($"{classFolder}/CharacteristicGrowthFormulas")) {
                    AssetDatabase.CreateFolder(classFolder, "CharacteristicGrowthFormulas");
                }
            }
        }

        private static void CreateCharacteristicScalingComponents(CharacteristicSet characteristicSet, Dictionary<string, Stat> statInstances, Dictionary<string, Characteristic> characteristicInstances) {
            var scalingMappings = new Dictionary<string, string> {
                { "Ability Power", "Intelligence" },
                { "Physical Attack", "Strength" },
                { "Critical Chance", "Dexterity" }
            };

            foreach (var mapping in scalingMappings) {
                CharacteristicsScalingComponent scalingComponent = ScriptableObject.CreateInstance<CharacteristicsScalingComponent>();
                scalingComponent.name = $"{mapping.Key.Replace(" ", "")}CharScaling";
                scalingComponent.SetSet(characteristicSet);
                scalingComponent._scalingAttributeValues[characteristicInstances[mapping.Value]] = 1.0;
                AssetDatabase.CreateAsset(scalingComponent, $"{RootFolder}/Stats/CharToStatScalings/{scalingComponent.name}.asset");

                // Assign the scaling component to the corresponding stat
                statInstances[mapping.Key].CharacteristicsScaling = scalingComponent;
            }
        }
    }
}
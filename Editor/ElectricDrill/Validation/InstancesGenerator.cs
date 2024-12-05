using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Characteristics;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    public class InstancesGenerator
    {
        [MenuItem("Tools/Generate ScriptableObjects")]
        public static void GenerateScriptableObjects() {
            // Create Stats
            Stat physicalAttack = ScriptableObject.CreateInstance<Stat>();
            physicalAttack.name = "Physical Attack";
            AssetDatabase.CreateAsset(physicalAttack, "Assets/Stats/PhysicalAttack.asset");

            Stat dodgeChance = ScriptableObject.CreateInstance<Stat>();
            dodgeChance.name = "Dodge Chance";
            AssetDatabase.CreateAsset(dodgeChance, "Assets/Stats/DodgeChance.asset");

            // Create Characteristics
            Characteristic strength = ScriptableObject.CreateInstance<Characteristic>();
            strength.name = "Strength";
            AssetDatabase.CreateAsset(strength, "Assets/Characteristics/Strength.asset");

            Characteristic agility = ScriptableObject.CreateInstance<Characteristic>();
            agility.name = "Agility";
            AssetDatabase.CreateAsset(agility, "Assets/Characteristics/Agility.asset");

            // Create Stat Set
            StatSet statSet = ScriptableObject.CreateInstance<StatSet>();
            statSet.name = "DefaultStatSet";
            statSet.SetStats(new SerializableHashSet<Stat> { physicalAttack, dodgeChance });
            AssetDatabase.CreateAsset(statSet, "Assets/StatSets/DefaultStatSet.asset");

            // Create Characteristic Set
            CharacteristicSet characteristicSet = ScriptableObject.CreateInstance<CharacteristicSet>();
            characteristicSet.name = "DefaultCharacteristicSet";
            characteristicSet.SetCharacteristics(new SerializableHashSet<Characteristic> { strength, agility });
            AssetDatabase.CreateAsset(characteristicSet, "Assets/CharacteristicSets/DefaultCharacteristicSet.asset");

            // Create Class
            Class characterClass = ScriptableObject.CreateInstance<Class>();
            characterClass.name = "Warrior";
            characterClass.StatSet = statSet;
            characterClass.CharacteristicSet = characteristicSet;
            AssetDatabase.CreateAsset(characterClass, "Assets/Classes/Warrior.asset");

            // Create Growth Formulas
            GrowthFormula physicalAttackGrowth = ScriptableObject.CreateInstance<GrowthFormula>();
            physicalAttackGrowth.name = "PhysicalAttackGrowth";
            physicalAttackGrowth.useConstantAtLvl1 = true;
            physicalAttackGrowth.constantAtLvl1 = 10;
            physicalAttackGrowth.levelToGrowthFormulas = new List<GrowthFormula.LevelGrowthFormulaPair> {
                new GrowthFormula.LevelGrowthFormulaPair { FromLevel = 2, GrowthFormula = "PRV + 10" }
            };
            physicalAttackGrowth.OnValidate();
            AssetDatabase.CreateAsset(physicalAttackGrowth, "Assets/GrowthFormulas/PhysicalAttackGrowth.asset");

            GrowthFormula dodgeChanceGrowth = ScriptableObject.CreateInstance<GrowthFormula>();
            dodgeChanceGrowth.name = "DodgeChanceGrowth";
            dodgeChanceGrowth.useConstantAtLvl1 = true;
            dodgeChanceGrowth.constantAtLvl1 = 5;
            dodgeChanceGrowth.levelToGrowthFormulas = new List<GrowthFormula.LevelGrowthFormulaPair> {
                new GrowthFormula.LevelGrowthFormulaPair { FromLevel = 2, GrowthFormula = "PRV + 5" }
            };
            dodgeChanceGrowth.OnValidate();
            AssetDatabase.CreateAsset(dodgeChanceGrowth, "Assets/GrowthFormulas/DodgeChanceGrowth.asset");

            // Assign Growth Formulas to Stats and Characteristics in the Class instance
            characterClass._statGrowthFormulas[physicalAttack] = physicalAttackGrowth;
            characterClass._statGrowthFormulas[dodgeChance] = dodgeChanceGrowth;

            // Save all assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("ScriptableObjects generated successfully!");
        }
    }
}
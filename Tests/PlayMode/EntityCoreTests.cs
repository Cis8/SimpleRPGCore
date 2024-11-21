using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ElectricDrill.SimpleRpgCore.Tests
{
    public class EntityCoreTests
    {
        // Using a single stat for testing the correct cloning of the stats in the two EntityCore instances
        /*static StatSetMock stats = ScriptableObject.CreateInstance<StatSetMock>().Init();
        const int startingArmorValue = 10;

        private class ArmorStatMock : Stat
        {
            public ArmorStatMock Init() {
                this.name = "Armor";
                Value = startingArmorValue;
                return this;
            }
        }

        private class StatSetMock : StatSet
        {
            public StatSetMock Init() {
                Stats = new SerializableHashSet<Stat>() { CreateInstance<ArmorStatMock>().Init() };
                return this;
            }
        }

        private class ClassMock : Class
        {
            public ClassMock Init() {
                _statSet = stats;
                _statValuePairs = new List<StatGrowthFnPair>() {
                    new () {
                        Stat = stats.GetStat("Armor"),
                        StatGrowthFn = startingArmorValue
                    }
                };
                return this;
            }
        }

        private class EntityCoreWithPreloadedStats : EntityCore
        {
            protected override void Awake() {
                Init();
                base.Awake(); // Here stats will be cloned
            }

            private void Init() {
                Health = gameObject.AddComponent<Health>();
                Class = ScriptableObject.CreateInstance<ClassMock>().Init();
                Stats = stats;
            }
        }

        [Test]
        public void ModifyingAStatToOneEntityDoesNotModifyTheSameStatOfTheOtherEntity() {
            // Arrange
            EntityCore ec1 = new GameObject().AddComponent<EntityCoreWithPreloadedStats>();
            EntityCore ec2 = new GameObject().AddComponent<EntityCoreWithPreloadedStats>();
            ec1.Stats.Get("Armor").Value = 20;

            // Assert
            Assert.AreNotEqual(ec1.Stats.GetStat("Armor").Value, ec2.Stats.GetStat("Armor").Value);
        }*/
    }
}
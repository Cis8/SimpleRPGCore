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
    public class FlatDmgReductionFnTests
    {
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

        private class EntityCoreMock : EntityCore
        {
            protected override void Awake() {
                Init();
                base.Awake();
            }

            private EntityCoreMock Init() {
                Health = gameObject.AddComponent<Health>();
                Stats = stats;
                Class = ScriptableObject.CreateInstance<ClassMock>().Init();
                return this;
            }
        }

        private class PhysicalDmgType : DmgType
        {
            public PhysicalDmgType Init() {
                name = "Physical";
                ReducedBy = CreateInstance<ArmorStatMock>().Init();
                DmgReduction = CreateInstance<FlatDmgReductionFn>();
                return this;
            }
        }

        [Test]
        public void _20PhysicalDmgThatIsFlatlyReducedBy10OfArmorByConstant1Subtracts10HealthOnly() {
            // Arrange
            EntityCore ec = new GameObject().AddComponent<EntityCoreMock>();
            var physicalDmgType = ScriptableObject.CreateInstance<PhysicalDmgType>().Init();
            var healthBeforeDamage = ec.Health.HP;
            int amountOfDamage = 20;

            // Act
            // todo re-enable
            //ec.TakeDamage(amountOfDamage, physicalDmgType);

            // Assert
            Assert.AreEqual(ec.Health.HP + 10, healthBeforeDamage);
        }*/
    }
}
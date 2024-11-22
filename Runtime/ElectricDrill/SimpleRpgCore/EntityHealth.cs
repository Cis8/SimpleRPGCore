using System;
using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Damage;
using ElectricDrill.SimpleRpgCore.Events;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore
{
    [RequireComponent(typeof(EntityCore))]
    public class EntityHealth : MonoBehaviour, IDamageable, IHealable
    {
        [SerializeField] private bool healthCanBeNegative = false;
        // if true, the max hp will be taken from the class and assigned to _maxHp, overriding any value set in the inspector
        // if false, the value set in the inspector will be used
        [SerializeField] private bool useClassMaxHp = false;
        [SerializeField] private LongRef maxHp;
        [SerializeField] private LongRef hp;
        [SerializeField] private LongRef barrier;
        [SerializeField] private Stat healAmountModifier;
        [SerializeField] private OnDeathStrategy onDeathStrategy;

        private EntityCore _core;
        private EntityStats _stats;
        private EntityClass _entityClass;
        
        // Events
        [SerializeField] private PreDmgGameEvent preDmgInfoEvent;
        [SerializeField] private TakenDmgGameEvent takenDmgInfoEvent;
        [SerializeField] private EntityHealedGameEvent healedEvent;
        [SerializeField] private EntityGainedHealthGameEvent gainedHealthEvent;
        [SerializeField] private EntityLostHealthGameEvent lostHealthEvent;
        [SerializeField] private EntityDiedGameEvent entityDiedEvent;

        public long MAX_HP => maxHp;
        public long HP => hp;
        public long Barrier => barrier;
        public bool HealthCanBeNegative { get => healthCanBeNegative; set => healthCanBeNegative = value; }

        private void Awake() {
            Assert.IsNotNull(preDmgInfoEvent, $"PreDmgGameEvent is missing for {gameObject.name}");
            Assert.IsNotNull(takenDmgInfoEvent, $"TakenDmgGameEvent is missing for {gameObject.name}");
            Assert.IsNotNull(healedEvent, $"HealedGameEvent is missing for {gameObject.name}");
            Assert.IsFalse(maxHp <= 0, $"Max HP of an Entity must be greater than 0. {name}'s Max HP was {MAX_HP}");
            Assert.IsFalse(hp < 0, $"HP of an Entity must be greater than or equal to 0. {name}'s HP was {HP}");
            Assert.IsNotNull(onDeathStrategy, $"OnDeathStrategy is missing for {gameObject.name}");
            Assert.IsNotNull(entityDiedEvent, $"DiedGameEvent is missing for {gameObject.name}");
        }

        private void Start() {
            _stats = GetComponent<EntityStats>();
            Assert.IsTrue(healAmountModifier == null || _stats.StatSet.Contains(healAmountModifier), $"StatSet of {gameObject.name} doesn't contain the stat {healAmountModifier}");
            _entityClass = GetComponent<EntityClass>();
            if (useClassMaxHp) {
                Assert.IsNotNull(_entityClass, $"Class of {gameObject.name} is missing");
                maxHp.Value = _entityClass.Class.GetMaxHpAt(_core.Level);
            }
            hp.Value = maxHp;
        }

        private void Update() {
        }

        public virtual void TakeDamage(PreDmgInfo preDmg) {
            Assert.IsTrue(preDmg.Amount >= 0, "Damage amount must be greater than or equal to 0");

            preDmgInfoEvent?.Raise(preDmg, _core);

            // get stats from the entity
            var defensiveStat = preDmg.Type.ReducedBy != null ? _stats.Get(preDmg.Type.ReducedBy) : 0;
            var piercingStat = preDmg.Type.DefensiveStatPiercedBy != null ? preDmg.Dealer.Stats.Get(preDmg.Type.DefensiveStatPiercedBy) : 0;

            // calculate the reduced defensive stat and the reduced amount of damage
            var reducedDefensiveStat = preDmg.Type.DefReductionFn != null ? preDmg.Type.DefReductionFn.ReducedDef(piercingStat, defensiveStat) : defensiveStat;
            var dmgToBeTaken = preDmg.Type.DmgReductionFn != null ? preDmg.Type.DmgReductionFn.ReducedDmg(preDmg.Amount, reducedDefensiveStat) : preDmg.Amount;

            var barrierReducedDmgToBeTaken = dmgToBeTaken;
            if (!preDmg.Type.IgnoresBarrier)
            {
                // subtract the dmg to be taken to the entity's barrier (if it has any)
                if (barrier > 0) {
                    barrierReducedDmgToBeTaken = Math.Max(0, dmgToBeTaken - barrier);
                    RemoveBarrier(dmgToBeTaken);
                }
            }

            // subtract the barrier-reduced dmg to be taken to the entity's health
            RemoveHealth(barrierReducedDmgToBeTaken);

            // the amount of damage taken is the amount of damage reduced by the defensive stat, but not by the barrier
            var takenDmgInfo = new TakenDmgInfo(dmgToBeTaken, preDmg, _core);
            takenDmgInfoEvent?.Raise(takenDmgInfo);
            if (IsDead()) {
                entityDiedEvent.Raise(this, takenDmgInfo);
                onDeathStrategy.Die(this);
            }
        }

        public void AddBarrier(long amount) {
            Assert.IsTrue(amount >= 0, $"Barrier amount to be added must be greater than or equal to 0, was {amount}");
            barrier.Value += amount;
        }
        
        private void RemoveBarrier(long amount) {
            Assert.IsTrue(amount >= 0, $"Barrier amount to be removed must be greater than or equal to 0, was {amount}");
            barrier.Value = Math.Max(0, barrier - amount);
        }
        
        /// <summary>
        /// Adds <paramref name="amount"/> health to the current health. If <paramref name="amount"/> would cause the
        /// current health to trespass the max health, current health is set to max health instead.
        /// </summary>
        /// <param name="amount">Health amount to be added</param>
        private long AddHealth(long amount) {
            Assert.IsTrue(amount >= 0, $"Health amount to be added must be greater than or equal to 0, was {amount}");
            var previousHp = hp;
            hp.Value = Math.Min(hp + amount, maxHp);
            long gainedHealth = hp - previousHp;
            gainedHealthEvent?.Raise(this, gainedHealth);
            return gainedHealth;
        }

        /// <summary>
        /// Subtracts <paramref name="amount"/> health to the current health. If <paramref name="amount"/> would cause
        /// the current health to go below zero and HealthCanBeNegative is false, it is set to 0 instead.
        /// </summary>
        /// <param name="amount">Health amount to be removed</param>
        private void RemoveHealth(long amount) {
            Assert.IsTrue(amount >= 0, $"Health amount to be removed must be greater than or equal to 0, was {amount}");
            var previousHp = hp;
            var newHp = hp - amount;
            if (newHp < 0 && !healthCanBeNegative) {
                newHp = 0;
            }
            hp.Value = newHp;
            lostHealthEvent?.Raise(this, previousHp - hp);
        }
        
        public void Heal(long amount)
        {
            Assert.IsTrue(amount >= 0, $"Heal amount must be greater than or equal to 0, was {amount}");
            if (healAmountModifier != null) {
                Percentage healModifier = _stats.Get(healAmountModifier);
                amount = (long) (amount * healModifier);
            }
            var gainedHealth = AddHealth(amount);
            healedEvent.Raise(this, gainedHealth);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>true if current health is <= 0, false otherwise</returns>
        public bool IsDead() {
            return hp <= 0;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>true if current health is > 0, false otherwise</returns>
        public bool IsAlive() {
            return !IsDead();
        }
        
        private void OnLevelUp(int level) {
            if (useClassMaxHp)
                maxHp.Value = _entityClass.Class.GetMaxHpAt(level);
            // todo add flag to decide if health should be restored on level-up
        }
        
        private void OnEnable() {
            _core = GetComponent<EntityCore>();
            _core.Level.OnLevelUp += OnLevelUp;
        }


        private void OnDisable() {
            _core.Level.OnLevelUp -= OnLevelUp;
        }
    }
}
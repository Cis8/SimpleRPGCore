using System;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Events;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore
{
    [Serializable]
    public class EntityLevel
    {
        // LEVEL FIELDS
        // even if the level could be retrieved from the experience, it is more efficient to have a dedicated field
        [SerializeField, HideInInspector] private IntRef _level = 1; // Add message that tells the user to not manually change the value of this IntVar
        [SerializeField] private IntRef _maxLevel = 100;
        
        // EXPERIENCE FIELDS
        [SerializeField] private GrowthFormula _experienceGrowthFormula;
        [SerializeField, HideInInspector] internal LongRef currentTotalExperience;
        [SerializeField] private Stat experienceGainedModifierStat;
        private Func<Percentage> _experienceGainedModifier;
        
        internal Stat ExperienceGainedModifierStat => experienceGainedModifierStat;
        
        // Editor-wired Events
        [SerializeField] private IntGameEvent _onLevelUpEditor;
        
        // Code-wired Events
        private Action<int> _onLevelUpCode;
        public Action<int> OnLevelUp { get => _onLevelUpCode; set => _onLevelUpCode = value; }
        public int Level {
            get
            {
                Assert.IsNotNull(_level);
                Assert.IsTrue(_level >= 1, "Level must be greater than or equal to 1");
                return _level;
            }
            set
            {
                Assert.IsTrue(value >= 1, $"Level must be greater than or equal to 1, was {value}");
                Assert.IsTrue(value <= _maxLevel, $"Level must be less than or equal to the max level, was {value}");
                _level.Value = value;
            }
        }

        private void LevelUp() {
            if (_level.Value >= _maxLevel.Value) {
                Debug.LogWarning("Entity is already at max level");
                return;
            }
            _level.Value++;
            // todo evaluate if these shall be raised in the property setter (and changed to onLevelChanged)
            _onLevelUpEditor?.Raise(_level);
            _onLevelUpCode?.Invoke(_level);
        }
        
        // generate docs
        /// <summary>
        /// Adds, to the current total experience, the amount of experience needed to reach the next level.
        /// </summary>
        /// <returns>The amount of experience added</returns>
        private long AddExpForNextLevel() {
            var amountToAdd = NextLevelTotalExperience() - currentTotalExperience;
            currentTotalExperience.Value += amountToAdd;
            LevelUp();
            return amountToAdd;
        }
        
        public void AddExp(long amount) {
            long modifiedAmount = (long)(amount * GetExpGainedMultiplier());
            AddExpWithoutModifier(modifiedAmount);
        }
        
        private void AddExpWithoutModifier(long amount) {
            if (currentTotalExperience.Value + amount >= NextLevelTotalExperience()) {
                var remaining = amount - AddExpForNextLevel();
                AddExpWithoutModifier(remaining);
            }
            else {
                currentTotalExperience.Value += amount;
            }
        }

        private double GetExpGainedMultiplier() {
            var modifier = _experienceGainedModifier?.Invoke();
            return 1.0d + (modifier?? 0.0d);
        }
        
        internal void SetExperienceGainedModifier(Func<Percentage> experienceGainedModifier) {
            _experienceGainedModifier = experienceGainedModifier;
        }

        public void SetTotalCurrentExp(long totalCurrentExperience) {
            Assert.IsNotNull(_maxLevel, "Max Level is missing");
            currentTotalExperience.Value = totalCurrentExperience;
            var growthFoValues = _experienceGrowthFormula.GrowthFoValues;
            var i = 0;
            for (; i < growthFoValues.Length; i++) {
                if (growthFoValues[i] > currentTotalExperience) {
                    _level.Value = i + 1;
                }
            }
            // if the total experience is greater or equal to the last level's experience, then the entity is at max level
            if (i >= growthFoValues.Length)
                _level.Value = _maxLevel.Value;
        }

        public long CurrentTotalExperience => currentTotalExperience;
        public long CurrentLevelTotalExperience() => _level == 1 ? 
            0
            : _experienceGrowthFormula.GetGrowthValue(_level - 1);
        
        public long NextLevelTotalExperience() {
            Assert.IsNotNull(_experienceGrowthFormula, $"Experience Growth Formula is missing for {nameof(EntityLevel)}");
            return _level == _maxLevel
                ? _experienceGrowthFormula.GetGrowthValue(_level - 1)
                : _experienceGrowthFormula.GetGrowthValue(_level);
        }

        // implicit conversion from EntityLevel to int
        public static implicit operator int(EntityLevel entityLevel) => entityLevel.Level;

        // used to check that the experience corresponds to the level. If not, the experience is set to the correct value
        public void ValidateExperience() {
            if (_experienceGrowthFormula == null) return;
            
            if (currentTotalExperience.Value < CurrentLevelTotalExperience() || currentTotalExperience.Value >= NextLevelTotalExperience()) {
                Debug.LogWarning("Current total exp does not correspond to the current level." +
                                 $" Resetting it to {CurrentLevelTotalExperience()}, the base value for level {_level}");
                currentTotalExperience.Value = CurrentLevelTotalExperience();
            }
        }
    }
}
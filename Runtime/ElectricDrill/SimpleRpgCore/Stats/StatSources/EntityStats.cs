using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Characteristics;
using ElectricDrill.SimpleRpgCore.Damage;
using ElectricDrill.SimpleRpgCore.Events;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    [RequireComponent(typeof(EntityCore))]
    public class EntityStats : MonoBehaviour
    {
        private EntityCore _entityCore;

        [FormerlySerializedAs("_useFixedBaseStats")] [SerializeField] private bool _useClassBaseStats = true;

        // todo hide fixed or dynamic stats based on the value of _useFixedStats
        
        // Dynamic stats
        protected EntityClass _entityClass;

        protected StatSetInstance _baseClassStats;

        protected StatSetInstance _flatModifiersStats;

        // the key is the target stat, the value contains all the percentages of the source stats to be summed up
        private Dictionary<Stat, StatSetInstance> _statToStatModifiers;

        protected StatSetInstance _percentageModifiers;
        
        // Fixed stats
        [SerializeField] private StatSet fixedBaseStatsStatSet;
        [SerializeField] private List<SerKeyValPair<Stat, long>> _inspectorReservedFixedBaseStats;
        private StatSetInstance _fixedBaseStats;
        
        // todo evaluate if finalStats cache shall be added
        
        // Events
        [SerializeField] private StatChangedGameEvent _onStatChanged;
        
        public StatChangedGameEvent OnStatChanged => _onStatChanged;

        /// <summary>
        /// Returns the StatSet of the entity. If _useFixedBaseStats is true, it returns the fixedBaseStatsStatSet, otherwise it returns the StatSet of the entity's class.
        /// </summary>
        public StatSet StatSet {
            get {
                if (_useClassBaseStats) {
                    if (TryGetComponent(out _entityClass))
                        return _entityClass.Class.StatSet;
                    throw new Exception($"EntityClass component must be attached to the {gameObject.name} GameObject if _useFixedBaseStats is set false");
                }
                return fixedBaseStatsStatSet;
            }
        }

        private void Awake() {
            Assert.IsNotNull(_onStatChanged, $"{name}'s OnStatChanged must be set in the inspector");
            _entityCore = GetComponent<EntityCore>();
            CheckInitializeClassBaseStats();
            _statToStatModifiers ??= new Dictionary<Stat, StatSetInstance>();
            InitializeFlatModifierStatsIfNull();
            InitializePercentageModifierStatsIfNull();
        }
        
        void Start() {
            
        }

        void Update() {
        }

        // READ STATS
        public long GetBase(Stat stat) {
            Assert.IsTrue(StatSet.Contains(stat), $"Stat {stat.name} is not in the {name}'s StatSet ({StatSet.name})");
            if (_useClassBaseStats) {
                var baseValue = _baseClassStats.Get(stat);
                baseValue += stat.CharacteristicsScaling?.CalculateValue(_entityCore) ?? 0;
                return stat.Clamp(baseValue);
            }
            
            return stat.Clamp(_fixedBaseStats[stat]);
        }
        
        public long Get(Stat stat) {
            Assert.IsTrue(StatSet.Contains(stat), $"Stat {stat.name} is not in the {name}'s StatSet ({StatSet.name})");
            return stat.Clamp(CalculateFinalStat(stat));
        }
        
        private long CalculateFinalStat(Stat stat) {
            var statValue = CalculateFlatStat(stat);
            
            if (_statToStatModifiers.ContainsKey(stat)) {
                // apply stat to stat modifiers
                var statToStatModification = _statToStatModifiers[stat].Select(kv => {
                    Percentage sourceStatPerc = kv.Value;
                    var flatModifierForStat = (long)Math.Round(CalculateFlatStat(kv.Key) * sourceStatPerc);
                    return flatModifierForStat;
                }).Sum();
                statValue += statToStatModification;
            }

            if (_percentageModifiers.Contains(stat)) {
                // apply percentage modifiers
                var percentageModifier = (long)Math.Round(statValue * _percentageModifiers.GetAsPercentage(stat));
                statValue += percentageModifier;
            }
            
            return statValue;

            long CalculateFlatStat(Stat flatStat) {
                return GetBase(flatStat) + _flatModifiersStats[flatStat];
            }
        }

        // WRITE STATS
        public void AddFlatModifier(Stat stat, long value) {
            long oldValue = Get(stat);
            var dependentStats = GetDependentStats(stat);
            var oldDependentValues = dependentStats.ToDictionary(dependentStat => dependentStat, Get);

            _flatModifiersStats.AddValue(stat, value);
            long newValue = Get(stat);
            CheckRaiseStatChanged(stat, oldValue, newValue);

            foreach (var dependentStat in dependentStats) {
                long oldDependentValue = oldDependentValues[dependentStat];
                long newDependentValue = Get(dependentStat);
                CheckRaiseStatChanged(dependentStat, oldDependentValue, newDependentValue);
            }
        }
        
        public void AddStatToStatModifer(Stat target, Stat source, Percentage percentage) {
            if (!_statToStatModifiers.ContainsKey(target)) {
                _statToStatModifiers.Add(target, new StatSetInstance(StatSet));
            }
            long oldValue = Get(target);
            _statToStatModifiers[target].AddValue(source, (long)percentage);
            long newValue = Get(target);
            CheckRaiseStatChanged(target, oldValue, newValue);
        }
        
        public void AddPercentageModifier(Stat stat, Percentage value) {
            long oldValue = Get(stat);
            _percentageModifiers.AddValue(stat, (long)value);
            long newValue = Get(stat);
            CheckRaiseStatChanged(stat, oldValue, newValue);   
        }
        
        // EVENTS and EDITOR
        private void OnLevelUp(int level) {
            var oldStatValues = GetCurrentFinalStatValues();
            _baseClassStats = _entityClass?.Class.CreateStatSetInstanceAt(level);

            foreach (var stat in StatSet.Stats) {
                long oldValue = oldStatValues.Get(stat);
                long newValue = Get(stat);
                CheckRaiseStatChanged(stat, oldValue, newValue);
            }
        }

        private void CheckRaiseStatChanged(Stat stat, long oldValue, long newValue) {
            if (oldValue != newValue)
                _onStatChanged.Raise(new StatChangeInfo(this, stat, oldValue, newValue));
        }
        
        private List<Stat> GetDependentStats(Stat stat) {
            var dependentStats = new List<Stat>();

            if (_statToStatModifiers.TryGetValue(stat, out var statSetInstanceForStat)) {
                foreach (var kvp in statSetInstanceForStat) {
                    if (kvp.Value != 0) {
                        dependentStats.Add(kvp.Key);
                    }
                }
            }

            return dependentStats;
        }
        
        private StatSetInstance GetCurrentFinalStatValues() {
            var finalStatValues = new StatSetInstance(StatSet);
            foreach (var stat in StatSet.Stats) {
                finalStatValues.AddValue(stat, Get(stat));
            }
            return finalStatValues;
        }
        
        private void OnEnable() {
            _entityCore = GetComponent<EntityCore>();
            CheckInitializeClassBaseStats();
            _entityCore.Level.OnLevelUp += OnLevelUp;
            if (!_useClassBaseStats)
                InitializeFixedBaseStats();
#if UNITY_EDITOR
            OnValidate();
#endif
        }


        private void OnDisable() {
            _entityCore.Level.OnLevelUp -= OnLevelUp;
#if UNITY_EDITOR
            OnValidate();
#endif
        }

        private void OnValidate() {
            if (!_useClassBaseStats) {
                InitializationUtils.RefreshInspectorReservedValues(ref _inspectorReservedFixedBaseStats, fixedBaseStatsStatSet?.Stats);
                InitializeFixedBaseStats();
            }
        }

        // UTILS
        private void CheckInitializeClassBaseStats() {
            if (_useClassBaseStats) {
                if (!TryGetComponent(out _entityClass))
                    Debug.LogError($"EntityClass component must be attached to the {gameObject.name} GameObject if _useFixedBaseStats is set false");
                _baseClassStats = _entityClass?.Class.CreateStatSetInstanceAt(_entityCore.Level);
            }
        }
        
        private void InitializeFixedBaseStats() {
            if (!_useClassBaseStats) {
                _fixedBaseStats = new StatSetInstance(fixedBaseStatsStatSet);
                foreach (var statValuePair in _inspectorReservedFixedBaseStats) {
                    _fixedBaseStats.AddValue(statValuePair.Key, statValuePair.Value);
                }
            }
        }
        
        private void InitializeFlatModifierStatsIfNull() {
            _flatModifiersStats ??= new StatSetInstance(StatSet);
        }
        
        private void InitializePercentageModifierStatsIfNull() {
            _percentageModifiers ??= new StatSetInstance(StatSet);
        }
    }
}
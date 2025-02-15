using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Events;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    [RequireComponent(typeof(EntityCore))]
    public class EntityStats : MonoBehaviour, IStatSet
    {
        private EntityCore _entityCore;

        [SerializeField, HideInInspector] private bool useBaseStatsFromClass = true;
        public bool UseClassBaseStats {
            get => useBaseStatsFromClass;
            internal set => useBaseStatsFromClass = value;
        }

        // Dynamic stats
        protected IClassSource _entityClass;

        protected StatSetInstance _flatModifiersStats;

        // the key is the target stat, the value contains all the percentages of the source stats to be summed up
        internal Dictionary<Stat, StatSetInstance> _statToStatModifiers;

        protected StatSetInstance _percentageModifiers;
        
        // Fixed stats
        [SerializeField, HideInInspector] internal StatSet fixedBaseStatsStatSet;
        [SerializeField, HideInInspector] internal SerializableDictionary<Stat, long> _fixedBaseStats = new();
        
        // Events
        [SerializeField, HideInInspector] private StatChangedGameEvent _onStatChanged;

        public EntityCore EntityCore {
            get => _entityCore;
            internal set => _entityCore = value;
        }

        public IClassSource EntityClass {
            get => _entityClass;
            internal set => _entityClass = value;
        }

        public StatChangedGameEvent OnStatChanged {
            get => _onStatChanged;
            internal set => _onStatChanged = value;
        }
        
        internal SerializableDictionary<Stat, long> FixedBaseStats {
            get => _fixedBaseStats;
            set => _fixedBaseStats = value;
        }
        
        public Dictionary<Stat, long>.KeyCollection FixedBaseStatsKeys => _fixedBaseStats.Keys;

        public void Set(Stat s, long v) {
            _fixedBaseStats[s] = v;
        }
        
        /// <summary>
        /// Returns the StatSet of the entity. If _useFixedBaseStats is true, it returns the fixedBaseStatsStatSet, otherwise it returns the StatSet of the entity's class.
        /// </summary>
        public virtual StatSet StatSet {
            get {
                if (useBaseStatsFromClass) {
                    if (_entityClass != null || TryGetComponent(out _entityClass))
                        return _entityClass.Class.StatSet;
                    throw new Exception($"EntityClass component must be attached to the {gameObject.name} GameObject if useBaseStatsFromClass is set true");
                }
                return fixedBaseStatsStatSet;
            }
        }

        private void Awake() {
            _entityCore = GetComponent<EntityCore>();
            _statToStatModifiers ??= new Dictionary<Stat, StatSetInstance>();
            InitializeFlatModifierStatsIfNull();
            InitializePercentageModifierStatsIfNull();
        }
        
        void Start() {
            Assert.IsNotNull(_onStatChanged, $"{name}'s OnStatChanged must be set in the inspector");
        }

        void Update() {
        }

        // READ STATS
        public long GetBase(Stat stat) {
            return GetBaseAt(stat, _entityCore.Level);
        }

        private long GetBaseAt(Stat stat, int level) {
            Assert.IsTrue(StatSet.Contains(stat), $"Stat {stat.name} is not in the {name}'s StatSet ({StatSet.name})");
            long baseValue;
            
            baseValue = GetRawBaseAt(stat, level);
            baseValue += GetAttributesStatBonus(stat);
            
            return stat.Clamp(baseValue);
        }
        
        private long GetRawBaseAt(Stat stat, int level) {
            Assert.IsTrue(StatSet.Contains(stat), $"Stat {stat.name} is not in the {name}'s StatSet ({StatSet.name})");
            
            return useBaseStatsFromClass ? _entityClass.Class.GetStatAt(stat, level) : _fixedBaseStats[stat];
        }
        
        private long GetAttributesStatBonus(Stat stat) {
            if (_entityCore.Attributes && _entityCore.Attributes.enabled)
                return stat.AttributesScaling?.CalculateValue(_entityCore) ?? 0;
            return 0;
        }
        
        public virtual long Get(Stat stat) {
            Assert.IsTrue(StatSet.Contains(stat), $"Stat {stat.name} is not in the {name}'s StatSet ({StatSet.name})");
            return stat.Clamp(CalculateFinalStat(stat));
        }
        
        private long CalculateFinalStat(Stat stat) {
            var statValue = CalculateFlatStat(stat);
            
            if (_statToStatModifiers.ContainsKey(stat)) {
                // apply stat to stat modifiers
                var statToStatModification = _statToStatModifiers[stat].Select(kv => {
                    Percentage sourceStatPerc = kv.Value;
                    return sourceStatPerc == 0 ? 0 : (long)Math.Round(CalculateFlatStat(kv.Key) * sourceStatPerc);
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
        protected virtual void OnLevelUp(int level) {
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
            EntityCore.Level.OnLevelUp += OnLevelUp;
#if UNITY_EDITOR
            OnValidate();
#endif
        }


        private void OnDisable() {
            EntityCore.Level.OnLevelUp -= OnLevelUp;
#if UNITY_EDITOR
            OnValidate();
#endif
        }

        private void OnValidate() {
            if (!useBaseStatsFromClass) {
                InitializeFixedBaseStats();
            }
        }

        internal void InitializeFixedBaseStats() {
            InitializationUtils.RefreshInspectorReservedValues(ref _fixedBaseStats.inspectorReservedPairs, fixedBaseStatsStatSet?.Stats);
            _fixedBaseStats.OnAfterDeserialize();
        }
        

        // UTILS
#if UNITY_EDITOR
        
        static EntityStats()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }
        
        private static void OnSelectionChanged() {
            if (Selection.activeObject is GameObject selectedObject && selectedObject.TryGetComponent<EntityStats>(out var entityStats)) {
                entityStats.OnValidate();
            }
        }
#endif
        
        internal void InitializeFlatModifierStatsIfNull() {
            _flatModifiersStats ??= new StatSetInstance(StatSet);
        }
        
        internal void InitializePercentageModifierStatsIfNull() {
            _percentageModifiers ??= new StatSetInstance(StatSet);
        }
    }
}
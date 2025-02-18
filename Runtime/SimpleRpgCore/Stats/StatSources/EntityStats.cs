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
    /// <summary>
    /// Component that manages the statistics of an entity in the game.
    /// It handles base stats, flat stat modifiers, stat to stat modifiers, and percentage stat modifiers.<br/>
    /// Base stats can either be fixed or come from the entity's class (if one is available on the Game Object).
    /// When stats change because of a modifier of any kind, the assigned <see cref="StatChangedGameEvent"/> is raised.
    /// </summary>
    [RequireComponent(typeof(EntityCore))]
    public class EntityStats : MonoBehaviour, IStatSet
    {
        private EntityCore _entityCore;

        [SerializeField, HideInInspector] private bool useBaseStatsFromClass = true;
        /// <summary>
        /// Indicates whether to use base stats from the entity's class or the fixed base stats.
        /// </summary>
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

        /// <summary>
        /// The conveniently cached <see cref="EntityCore"/> component of the entity.
        /// </summary>
        public EntityCore EntityCore {
            get => _entityCore;
            internal set => _entityCore = value;
        }

        /// <summary>
        /// The class source of the entity. In most cases, this is the <see cref="EntityClass"/> component attached to the entity.
        /// </summary>
        public IClassSource EntityClass {
            get => _entityClass;
            internal set => _entityClass = value;
        }

        /// <summary>
        /// Event raised when a stat changes due to a modifier.
        /// </summary>
        public StatChangedGameEvent OnStatChanged {
            get => _onStatChanged;
            internal set => _onStatChanged = value;
        }
        
        internal SerializableDictionary<Stat, long> FixedBaseStats {
            get => _fixedBaseStats;
            set => _fixedBaseStats = value;
        }
        
        internal Dictionary<Stat, long>.KeyCollection FixedBaseStatsKeys => _fixedBaseStats.Keys;

        /// <summary>
        /// Sets the value of a fixed base stat.
        /// </summary>
        /// <param name="s">The stat to set.</param>
        /// <param name="v">The value to set.</param>
        public void SetFixed(Stat s, long v) {
            _fixedBaseStats[s] = v;
        }
        
        /// <summary>
        /// The stat set used to calculate the entity's stats.
        /// </summary>
        /// <returns>
        /// If useBaseStatsFromClass is true, it returns the stat set of the entity's class.
        /// Otherwise, it returns the fixed base stats stat set.
        /// </returns>
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
        
        private void Start() {
            Assert.IsNotNull(_onStatChanged, $"{name}'s OnStatChanged must be set in the inspector");
        }

        private void Update() {
        }

        /// <summary>
        /// The base value is the value of the stat without any modifiers.
        /// If UseClassBaseStats is true, it returns the value from the entity's class.
        /// Otherwise, it returns the value from the fixed base stats.
        /// </summary>
        /// <param name="stat">The stat to get the base value of.</param>
        /// <returns>The base value of the stat. The value is clamped to the stat's min and max values.</returns>
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
        
        /// <summary>
        /// The final value of a stat, considering all the modifiers.
        /// Calculation is done in the following order:<br/>
        /// 1) Base value<br/>
        /// 2) Flat modifiers<br/>
        /// 3) Stat to stat modifiers<br/>
        /// 4) Percentage modifiers<br/>
        /// </summary>
        /// <param name="stat">The stat to get the final value of.</param>
        /// <returns>The final value of the stat. The value is clamped to the stat's min and max values.</returns>
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

        /// <summary>
        /// Adds a flat modifier to a stat.
        /// </summary>
        /// <param name="stat">The stat to add the flat modifier to.</param>
        /// <param name="value">The value of the flat modifier.</param>
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
        
        /// <summary>
        /// Adds a stat-to-stat modifier. Such modifiers add a percentage of the source stat to the target stat.
        /// Such modifiers consider the base value and the flat modifiers of the source stat.
        /// </summary>
        /// <param name="target">The target stat.</param>
        /// <param name="source">The source stat.</param>
        /// <param name="percentage">The <see cref="Percentage"/> of the source stat to add to the target stat.</param>
        public void AddStatToStatModifer(Stat target, Stat source, Percentage percentage) {
            if (!_statToStatModifiers.ContainsKey(target)) {
                _statToStatModifiers.Add(target, new StatSetInstance(StatSet));
            }
            long oldValue = Get(target);
            _statToStatModifiers[target].AddValue(source, (long)percentage);
            long newValue = Get(target);
            CheckRaiseStatChanged(target, oldValue, newValue);
        }
        
        /// <summary>
        /// Adds a <see cref="Percentage"/> modifier to a stat. Such modifiers consider the base value of the stat,
        /// the flat modifiers, and the stat-to-stat modifiers.
        /// </summary>
        /// <param name="stat">The stat to add the percentage modifier to.</param>
        /// <param name="value">The value of the percentage modifier.</param>
        public void AddPercentageModifier(Stat stat, Percentage value) {
            long oldValue = Get(stat);
            _percentageModifiers.AddValue(stat, (long)value);
            long newValue = Get(stat);
            CheckRaiseStatChanged(stat, oldValue, newValue);   
        }
        
        // EVENTS and EDITOR
        
        /// <summary>
        /// Callback method called when the entity levels up.
        /// </summary>
        /// <param name="level">The new level of the entity.</param>
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
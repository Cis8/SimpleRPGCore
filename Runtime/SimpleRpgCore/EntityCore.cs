using System;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Events;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine.Assertions;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore
{
    public class EntityCore : MonoBehaviour, ILevel, IAttributes
    {
        [SerializeField] private EntityLevel _level;
        
        // mainly here for enhancing the performance while reading stats with scaling formulas
        private EntityStats _stats;
        private EntityAttributes _attributes;
        
        // EVENTS
        [SerializeField] private EntityCoreGameEvent spawnedEntityEvent;

        public virtual EntityLevel Level { get => _level; internal set => _level = value; }
        public virtual EntityStats Stats => _stats ? _stats : GetComponent<EntityStats>();
        public virtual EntityAttributes Attributes => _attributes ? _attributes : GetComponent<EntityAttributes>();

        public string Name => name;

        internal EntityCoreGameEvent SpawnedEntityEvent {
            get => spawnedEntityEvent;
            set => spawnedEntityEvent = value;
        }

        protected virtual void Awake() {
        }

        protected virtual void Start() {
            if (!_stats)
                _stats = GetComponent<EntityStats>();
            
            // this can be null since it is not required
            _attributes = GetComponent<EntityAttributes>();
            if (_level.ExperienceGainedModifierStat)
                _level.SetExperienceGainedModifier(() => _stats.Get(_level.ExperienceGainedModifierStat));
            
            _level.ValidateExperience();
            
            Assert.IsNotNull(spawnedEntityEvent, $"Spawned entity event is null on {name}");
            spawnedEntityEvent.Raise(this);
        }

        protected virtual void Update() {
        }

        private void OnValidate() {
            // this is required so that if the level is updated, also the spendable attribute points are updated
            if (TryGetComponent(out EntityAttributes attributes)) {
                attributes.OnValidate();
            }
        }
    }
}
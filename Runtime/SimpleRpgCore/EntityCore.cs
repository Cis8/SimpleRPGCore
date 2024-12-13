using ElectricDrill.SimpleRpgCore.Attributes;
using UnityEngine;
using ElectricDrill.SimpleRpgCore.Events;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine.Assertions;


namespace ElectricDrill.SimpleRpgCore
{
    public class EntityCore : MonoBehaviour
    {
        [SerializeField] private EntityLevel _level;
        
        // mainly here for enhancing the performance while reading stats with scaling formulas
        private EntityStats _stats;
        private EntityAttributes _attributes;
        
        // EVENTS
        [SerializeField] private EntityCoreGameEvent spawnedEntityEvent;

        public virtual EntityLevel Level => _level;
        public virtual EntityStats Stats => _stats ? _stats : GetComponent<EntityStats>();
        public EntityAttributes Attributes => _attributes ? _attributes : GetComponent<EntityAttributes>();

        protected virtual void Awake() {
        }

        protected virtual void Start() {
            if (!_stats)
                _stats = GetComponent<EntityStats>();
            
            // this can be null since it is not required
            _attributes = GetComponent<EntityAttributes>();
            if (_level.ExperienceGainedModifierStat)
                _level.SetExperienceGainedModifier(() => _stats.Get(_level.ExperienceGainedModifierStat));
            
            Assert.IsNotNull(spawnedEntityEvent, $"Spawned entity event is null on {name}");
            spawnedEntityEvent.Raise(this);
        }

        protected virtual void Update() {
        }
    }
}
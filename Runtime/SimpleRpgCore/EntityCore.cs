using System;
using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Attributes;
using UnityEngine;
using ElectricDrill.SimpleRpgCore.Events;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine.Assertions;
using UnityEngine.Serialization;


namespace ElectricDrill.SimpleRpgCore
{
    public class EntityCore : MonoBehaviour
    {
        [SerializeField] private EntityLevel _level;
        
        // mainly here for enhancing the performance while reading stats with scaling formulas
        private EntityStats _stats;
        private EntityAttributes _attributes;

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
        }

        protected virtual void Update() {
        }
        
        private void CheckMissingReferences() {
            Assert.IsNotNull(_level, "Entity Level is missing");
        }
    }
}
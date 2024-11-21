using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElectricDrill.SimpleRpgCore.Damage;
using ElectricDrill.SimpleRpgCore.Events;
using ElectricDrill.SimpleRpgCore.Stats;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine.Assertions;
using UnityEngine.Serialization;


namespace ElectricDrill.SimpleRpgCore
{
    // requires EntityLevel and EntityStats
    [RequireComponent(typeof(EntityStats))]
    public class EntityCore : MonoBehaviour
    {
        [SerializeField] private EntityLevel _level;
        
        // mainly here for enhancing the performance while reading stats with scaling formulas
        private EntityStats _stats;

        public EntityLevel Level => _level;
        public EntityStats Stats => _stats;

        protected virtual void Awake() {
        }

        protected virtual void Start() {
            _stats = GetComponent<EntityStats>();
        }

        protected virtual void Update() {
        }
        
        private void CheckMissingReferences() {
            Assert.IsNotNull(_level, "Entity Level is missing");
        }
    }
}
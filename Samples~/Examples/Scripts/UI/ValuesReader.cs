using System;
using System.Collections;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Stats;
using UnityEngine;

namespace ElectricDrill.SimpleRPGCoreSamples {
    public enum ValueReaderType { Stat, Attribute }
    
    public class ValuesReader : MonoBehaviour
    {
        public EntityCore entityCore;
        public ValueReaderType readerType; 
        private EntityAttributes entityAttributes;
        private EntityStats entityStats;

        private void Awake() {
            switch (readerType) {
                case ValueReaderType.Stat:
                    entityStats = entityCore.GetComponent<EntityStats>();
                    if (entityStats == null) {
                        Debug.LogError("EntityStats component not found on the EntityCore.");
                    }
                    break;
                case ValueReaderType.Attribute:
                    entityAttributes = entityCore.GetComponent<EntityAttributes>();
                    if (entityAttributes == null) {
                        Debug.LogError("EntityAttributes component not found on the EntityCore.");
                    }
                    break;
            } 
        }

        void Start() {

        }

        void Update() {
            
        }

        private void OnEnable() {
            switch (readerType) {
                case ValueReaderType.Stat:
                    if (entityStats != null) {
                        StartCoroutine(ReadStats());
                    }
                    break;
                case ValueReaderType.Attribute:
                    if (entityAttributes != null) {
                        StartCoroutine(ReadAttributes());
                    }
                    break;
            }
        }
        
        private void OnDisable() {
            switch (readerType) {
                case ValueReaderType.Stat:
                    if (entityStats != null) {
                        StopCoroutine(ReadStats());
                    }
                    break;
                case ValueReaderType.Attribute:
                    if (entityAttributes != null) {
                        StopCoroutine(ReadAttributes());
                    }
                    break;
            }
        }

        public IEnumerator ReadStats() {
            while (true) {
                foreach (var stat in entityStats.StatSet.Stats) {
                    Debug.Log($"Stat: {stat.name}, Value: {entityStats.Get(stat)}");
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        public IEnumerator ReadAttributes() {
            while (true) {
                foreach (var attribute in entityAttributes.AttributeSet.Attributes) {
                    Debug.Log($"Attribute: {attribute.name}, Value: {entityAttributes.Get(attribute)}");
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    public abstract class SoSetScalingComponentBase<SetType, KeyType> : ScalingComponent where SetType : ScriptableObject
    {
        [SerializeField] protected SetType _set;
        
        [SerializeField] protected SerializableDictionary<KeyType, double> _scalingAttributeValues = new();

        public override long CalculateValue(EntityCore entity) {
            Assert.IsNotNull(_set, $"StatSet of {name} is missing");
            Assert.AreEqual(_set, GetEntitySet(entity), $"{typeof(SetType)} of {name} of {GetType()} ({_set.name}) does not match {entity.name}'s Set ({GetEntitySet(entity).name})");
            return _scalingAttributeValues.Sum(attributeMapping => {
                var value = GetEntityValue(entity, attributeMapping.Key);
                return (long) Math.Round(value * attributeMapping.Value);
            });
        }

        protected abstract SetType GetEntitySet(EntityCore entity);
        protected abstract long GetEntityValue(EntityCore entity, KeyType key);

        protected virtual void OnValidate() {
            if (_set != null) {
                _scalingAttributeValues = 
                    GetSetItems().Select(item => {
                        if (_scalingAttributeValues == null) {
                            return new KeyValuePair<KeyType, double>(item, 0d);
                        } else {
                            var existingItem = _scalingAttributeValues.FirstOrDefault(s => s.Key.Equals(item));
                            if (existingItem.Key == null) {
                                return new KeyValuePair<KeyType, double>(item, 0d);
                            }
                            return existingItem;
                        }
                    }).ToDictionary(pair => pair.Key, pair => pair.Value);
            }
            else {
                _scalingAttributeValues = new();
            }
        }
        
        protected abstract IEnumerable<KeyType> GetSetItems();

#if UNITY_EDITOR
        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable() {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged() {
            if (Selection.activeObject == this) {
                OnValidate();
            }
        }
#endif
    }
}
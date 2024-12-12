using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Attributes;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Attributes {
    public class AttributeSetInstance : IEnumerable<KeyValuePair<Attribute, long>>, IAttributeContainer
    {
        private Dictionary<Attribute, long> _attributes = new();

        private AttributeSetInstance() {}
        
        public AttributeSetInstance(AttributeSet attrSet) {
            foreach (var attribute in attrSet.Attributes) {
                _attributes.Add(attribute, 0);
            }
        }

        public Dictionary<Attribute, long> Attributes => _attributes;
        
        public void AddValue(Attribute attribute, long value) {
            if (!_attributes.TryAdd(attribute, value))
                _attributes[attribute] += value;
        }
        
        public long Get(Attribute attribute) {
            return _attributes[attribute];
        }
        
        // overload of the [] operator
        public long this[Attribute attribute] {
            get => Get(attribute);
            set => AddValue(attribute, value);
        }
        
        public AttributeSetInstance Clone() {
            var clone = new AttributeSetInstance();
            foreach (var attribute in _attributes) {
                clone.AddValue(attribute.Key, attribute.Value);
            }
            return clone;
        }
        
        public bool Contains(Attribute stat) {
            return _attributes.ContainsKey(stat);
        }
        
        public Percentage GetAsPercentage(Attribute stat) {
            return new Percentage(Get(stat));
        }

        public IEnumerator<KeyValuePair<Attribute, long>> GetEnumerator() {
            return _attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        public static AttributeSetInstance operator +(AttributeSetInstance a, AttributeSetInstance b) {
            AttributeSetInstance result = new();
            
            foreach (var attribute in a) {
                result.AddValue(attribute.Key, attribute.Value + b[attribute.Key]);
            }
            return result;
        }
        
        // explicit conversion from SerializableDictionary<Attribute, long> to AttributeSetInstance
        public static explicit operator AttributeSetInstance(SerializableDictionary<Attribute, long> dictionary) {
            return dictionary.ToAttributeSetInstance(null);
        }
    }
    
    // extension method for SerializableDictionary<Attribute, long> to create a AttributeSetInstance
    public static class AttributeSetInstanceExtensions {
        public static AttributeSetInstance ToAttributeSetInstance(this SerializableDictionary<Attribute, long> dictionary, AttributeSet attributeSet) {
            // Assert that the dictionary contains all the attributes in the attributeSet
            foreach (var attribute in attributeSet.Attributes) {
                if (!dictionary.ContainsKey(attribute)) {
                    Debug.LogError($"Dictionary does not contain the attribute {attribute} from the AttributeSet {attributeSet}");
                }
            }
            var attrSetInstance = new AttributeSetInstance(attributeSet);
            foreach (var pair in dictionary) {
                attrSetInstance.AddValue(pair.Key, pair.Value);
            }
            return attrSetInstance;
        }
    }
}

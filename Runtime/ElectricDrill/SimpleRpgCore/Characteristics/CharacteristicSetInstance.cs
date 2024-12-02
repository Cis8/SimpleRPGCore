using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Characteristics;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Characteristics {
    public class CharacteristicSetInstance : IEnumerable<KeyValuePair<Characteristic, long>>, ICharacteristicContainer
    {
        private Dictionary<Characteristic, long> _characteristics = new();

        private CharacteristicSetInstance() {}
        
        public CharacteristicSetInstance(CharacteristicSet charSet) {
            foreach (var characteristic in charSet.Characteristics) {
                _characteristics.Add(characteristic, 0);
            }
        }

        public Dictionary<Characteristic, long> Characteristics => _characteristics;
        
        public void AddValue(Characteristic characteristic, long value) {
            if (!_characteristics.TryAdd(characteristic, value))
                _characteristics[characteristic] += value;
        }
        
        public long Get(Characteristic characteristic) {
            return _characteristics[characteristic];
        }
        
        // overload of the [] operator
        public long this[Characteristic characteristic] {
            get => Get(characteristic);
            set => AddValue(characteristic, value);
        }
        
        public CharacteristicSetInstance Clone() {
            var clone = new CharacteristicSetInstance();
            foreach (var characteristic in _characteristics) {
                clone.AddValue(characteristic.Key, characteristic.Value);
            }
            return clone;
        }
        
        public bool Contains(Characteristic stat) {
            return _characteristics.ContainsKey(stat);
        }
        
        public Percentage GetAsPercentage(Characteristic stat) {
            return new Percentage(Get(stat));
        }

        public IEnumerator<KeyValuePair<Characteristic, long>> GetEnumerator() {
            return _characteristics.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        public static CharacteristicSetInstance operator +(CharacteristicSetInstance a, CharacteristicSetInstance b) {
            CharacteristicSetInstance result = new();
            
            foreach (var characteristic in a) {
                result.AddValue(characteristic.Key, characteristic.Value + b[characteristic.Key]);
            }
            return result;
        }
        
        // explicit conversion from SerializableDictionary<Characteristic, long> to CharacteristicSetInstance
        public static explicit operator CharacteristicSetInstance(SerializableDictionary<Characteristic, long> dictionary) {
            return dictionary.ToCharacteristicSetInstance(null);
        }
    }
    
    // extension method for SerializableDictionary<Characteristic, long> to create a CharacteristicSetInstance
    public static class CharacteristicSetInstanceExtensions {
        public static CharacteristicSetInstance ToCharacteristicSetInstance(this SerializableDictionary<Characteristic, long> dictionary, CharacteristicSet characteristicSet) {
            // Assert that the dictionary contains all the characteristics in the characteristicSet
            foreach (var characteristic in characteristicSet.Characteristics) {
                if (!dictionary.ContainsKey(characteristic)) {
                    Debug.LogError($"Dictionary does not contain the characteristic {characteristic} from the CharacteristicSet {characteristicSet}");
                }
            }
            var charSetInstance = new CharacteristicSetInstance(characteristicSet);
            foreach (var pair in dictionary) {
                charSetInstance.AddValue(pair.Key, pair.Value);
            }
            return charSetInstance;
        }
    }
}

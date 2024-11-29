using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        [SerializeField] internal List<SerKeyValPair<TKey, TValue>> inspectorReservedPairs = new();

        [SerializeField, HideInInspector] private bool missingKeyPair;

        [NonSerialized] private Dictionary<TKey, TValue> _dictionary = new();

        public void OnBeforeSerialize() {
            lock (this) {
                if (inspectorReservedPairs.Exists(p => p.Key == null))
                    missingKeyPair = true;

                inspectorReservedPairs.Clear();
                foreach (var kvp in _dictionary) {
                    inspectorReservedPairs.Add(new SerKeyValPair<TKey, TValue>(kvp.Key, kvp.Value));
                }

                if (missingKeyPair) {
                    inspectorReservedPairs.Add(new SerKeyValPair<TKey, TValue>());
                }   
            }
        }

        public void OnAfterDeserialize() {
            lock (this) {
                _dictionary = new Dictionary<TKey, TValue>();

                if (inspectorReservedPairs == null)
                    inspectorReservedPairs = new List<SerKeyValPair<TKey, TValue>>();

                if (inspectorReservedPairs.FindAll(p => p.Key == null).Count > 1) {
                    missingKeyPair = true;
                    inspectorReservedPairs.RemoveAll(p => p.Key == null);
                    Debug.LogWarning("Assign a valid key to the pair with null key before adding another pair");
                }
                else {
                    var nullKvpIndex = inspectorReservedPairs.FindIndex(p => p.Key == null);
                    if (nullKvpIndex != -1) {
                        missingKeyPair = true;
                        inspectorReservedPairs.RemoveAt(nullKvpIndex);
                    }
                    else {
                        missingKeyPair = false;
                    }
                }

                foreach (var pair in inspectorReservedPairs) {
                    if (pair.Key != null) {
                        _dictionary[pair.Key] = pair.Value;
                    }
                }   
            }
        }

        public void Add(TKey key, TValue value) {
            _dictionary.Add(key, value);
#if UNITY_EDITOR
            OnBeforeSerialize();
#endif
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key] {
            get => _dictionary[key];
            set {
                _dictionary[key] = value;
#if UNITY_EDITOR
                OnBeforeSerialize();
#endif
            }
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => _dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => _dictionary.Values;
        
        // implicit conversion from SerializableDictionary to Dictionary
        public static implicit operator Dictionary<TKey, TValue>(SerializableDictionary<TKey, TValue> serializableDictionary) {
            return serializableDictionary._dictionary;
        }
        
        // implicit conversion from Dictionary to SerializableDictionary
        public static implicit operator SerializableDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary) {
            return new SerializableDictionary<TKey, TValue> {_dictionary = dictionary};
        }
        
        public void Clear() {
            missingKeyPair = false;
            inspectorReservedPairs.Clear();
            _dictionary.Clear();
        }
        
        public bool ContainsKey(TKey key) {
            return _dictionary.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
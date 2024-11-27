using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] internal List<SerKeyValPair<TKey, TValue>> inspectorReservedPairs = new();

        [SerializeField, HideInInspector] private bool missingKeyPair;

        [NonSerialized] private Dictionary<TKey, TValue> _dictionary = new();

        public void OnBeforeSerialize()
        {
            lock (this)
            {
                if (inspectorReservedPairs.Exists(p => p.Key == null))
                    missingKeyPair = true;

                inspectorReservedPairs.Clear();
                foreach (var kvp in _dictionary)
                {
                    inspectorReservedPairs.Add(new SerKeyValPair<TKey, TValue>(kvp.Key, kvp.Value));
                }

                if (missingKeyPair)
                {
                    inspectorReservedPairs.Add(new SerKeyValPair<TKey, TValue>());
                }   
            }
        }

        public void OnAfterDeserialize()
        {
            lock (this)
            {
                _dictionary = new Dictionary<TKey, TValue>();

                if (inspectorReservedPairs == null)
                    inspectorReservedPairs = new List<SerKeyValPair<TKey, TValue>>();

                if (inspectorReservedPairs.FindAll(p => p.Key == null).Count > 1)
                {
                    missingKeyPair = true;
                    inspectorReservedPairs.RemoveAll(p => p.Key == null);
                    Debug.LogWarning("Assign a valid key to the pair with null key before adding another pair");
                }
                else
                {
                    var nullKvpIndex = inspectorReservedPairs.FindIndex(p => p.Key == null);
                    if (nullKvpIndex != -1)
                    {
                        missingKeyPair = true;
                        inspectorReservedPairs.RemoveAt(nullKvpIndex);
                    }
                    else
                    {
                        missingKeyPair = false;
                    }
                }

                foreach (var pair in inspectorReservedPairs)
                {
                    if (pair.Key != null)
                    {
                        _dictionary[pair.Key] = pair.Value;
                    }
                }   
            }
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => _dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => _dictionary.Values;
    }
}
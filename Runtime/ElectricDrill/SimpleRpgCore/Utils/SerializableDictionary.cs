using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Utils
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        [SerializeField] private List<SerKeyValPair<TKey, TValue>> inspectorReservedPairs = new();
        
        // holds the key-value pairs with no key to be shown in the inspector. Once the key is added, the pair is moved to the dictionary
        [SerializeField, HideInInspector] bool missingKeyPair;

        private Dictionary<TKey, TValue> dictionary = new();

        public void OnBeforeSerialize()
        {
            /*Debug.Log("Before OnBeforeSerialize: " +
                      $"inspectorReservedPairs: {inspectorReservedPairs.Count}\n" +
                      $"missingKeyPairs: {missingKeyPairs.Count}\n" +
                      $"dictionary: {dictionary.Count}");*/
            if (inspectorReservedPairs.Count == 2) Debug.Log("2");
            if (inspectorReservedPairs.Exists(p => p.Key == null))
                missingKeyPair = true;
                
            inspectorReservedPairs.Clear();
            foreach (var kvp in dictionary)
            {
                inspectorReservedPairs.Add(new SerKeyValPair<TKey, TValue>(kvp.Key, kvp.Value));
            }
            
            if (missingKeyPair)
            {
                inspectorReservedPairs.Add(new SerKeyValPair<TKey, TValue>());
            }
            /*Debug.Log("After OnBeforeSerialize: " +
                      $"inspectorReservedPairs: {inspectorReservedPairs.Count}\n" +
                      $"missingKeyPairs: {missingKeyPairs.Count}\n" +
                      $"dictionary: {dictionary.Count}");*/
        }

        public void OnAfterDeserialize()
        {
            /*Debug.Log("Before OnAfterDeserialize: " +
                      $"inspectorReservedPairs: {inspectorReservedPairs.Count}\n" +
                      $"missingKeyPairs: {missingKeyPairs.Count}\n" +
                      $"dictionary: {dictionary.Count}");*/
            
            dictionary = new Dictionary<TKey, TValue>();
            foreach (var pair in inspectorReservedPairs) {
                if (pair.Key != null) {
                    dictionary.Add(pair.Key, pair.Value);
                }
                else {
                    missingKeyPair = true;
                }
                // else {
                //     missingKeyPairs = new List<SerKeyValPair<TKey, TValue>>();
                //     missingKeyPairs.Add(pair);
                // }
            }
            
            if (!inspectorReservedPairs.Exists(p => p.Key == null)) {
                missingKeyPair = false;
            }
            
            /*Debug.Log("After OnAfterDeserialize: " +
                      $"inspectorReservedPairs: {inspectorReservedPairs.Count}\n" +
                      $"missingKeyPairs: {missingKeyPairs.Count}\n" +
                      $"dictionary: {dictionary.Count}");*/
        }

        public void Add(TKey key, TValue value)
        {
            dictionary.Add(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;
    }    
}

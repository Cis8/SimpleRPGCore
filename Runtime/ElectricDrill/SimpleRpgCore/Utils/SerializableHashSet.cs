using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Utils
{
    [Serializable]
    public class SerializableHashSet<T> : ISerializationCallbackReceiver,
        ICollection<T>,
        ISerializable
    {
        HashSet<T> hashSet = new();
        [SerializeField] private List<T> values = new();

        // save the hashset to list
        public void OnBeforeSerialize() {
            values.Clear();
            
            foreach (var value in hashSet) {
                values.Add(value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize() {
            hashSet.Clear();
            foreach (T value in values) {
                hashSet.Add(value);
            }
        }
        
        public IEnumerator<T> GetEnumerator() {
            return hashSet.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        
        public int Count => hashSet.Count;

        public void Add(T item) {
            hashSet.Add(item);
        }

        public void Clear() {
            hashSet.Clear();
        }

        public bool Contains(T item) {
            return hashSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            hashSet.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            return hashSet.Remove(item);
        }

        int ICollection<T>.Count => hashSet.Count;
        public bool IsReadOnly => false;

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            ((ISerializable)hashSet).GetObjectData(info, context);
        }
    }
}
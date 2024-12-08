using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Utils
{
    [Serializable]
    public class SerializableHashSet<T> : ISerializationCallbackReceiver,
        ICollection<T>,
        ISerializable
    {
        [NonSerialized] HashSet<T> hashSet = new();
        [SerializeField] private List<T> inspectorReservedValues = new();
        [SerializeField, HideInInspector] private bool missingValue = false;

        // save the hashset to list
        public void OnBeforeSerialize() {
            lock (this) {
                if (inspectorReservedValues.Exists(v => v == null)) {
                    missingValue = true;
                }
                inspectorReservedValues.Clear();
            
                foreach (var value in hashSet) {
                    if (value == null) {
                        continue;
                    }
                    inspectorReservedValues.Add(value);
                }
            
                if (missingValue) {
                    inspectorReservedValues.Add(default);
                }   
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize() {
            lock (this) {
                hashSet = new HashSet<T>();
                
                inspectorReservedValues ??= new List<T>();
            
                if (inspectorReservedValues.FindAll(v => v == null).Count > 1) {
                    inspectorReservedValues.RemoveAll(v => v == null);
                    missingValue = true;
                    Debug.LogWarning("Assign a valid value to the null item before adding another one");
                }
                else {
                    var nullItemIdx = inspectorReservedValues.FindIndex(v => v == null);
                    if (nullItemIdx != -1) {
                        missingValue = true;
                        inspectorReservedValues.RemoveAt(nullItemIdx);
                    }
                    else {
                        missingValue = false;
                    }
                }
            
                foreach (var value in inspectorReservedValues) {
                    if (value != null) {
                        hashSet.Add(value);
                    }
                }   
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
        
        public int RemoveWhere(Predicate<T> match) {
            return hashSet.RemoveWhere(match);
        }

        int ICollection<T>.Count => hashSet.Count;
        public bool IsReadOnly => false;

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            ((ISerializable)hashSet).GetObjectData(info, context);
        }
    }
}
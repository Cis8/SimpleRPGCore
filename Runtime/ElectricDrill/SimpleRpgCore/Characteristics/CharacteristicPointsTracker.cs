using System;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [Serializable]
    public class CharacteristicPointsTracker
    {
        [SerializeField, HideInInspector] int total; 
        [SerializeField, HideInInspector] int available;

        public int Available => available;

        [SerializeField, HideInInspector] private SerializableDictionary<Characteristic, int> spentCharacteristicPoints = new();
        
        internal SerializableDictionary<Characteristic, int> SpentCharacteristicPoints => spentCharacteristicPoints;
        public Dictionary<Characteristic, int>.KeyCollection SpentCharacteristics => spentCharacteristicPoints.Keys;

        internal void Init(int totalPoints) {
            total = totalPoints;
            available = total - spentCharacteristicPoints.Values.Sum();
        }
        
        public void AddPoints(int amount) {
            available += amount;
        }
        
        public long GetSpentOn(Characteristic characteristic) {
            return spentCharacteristicPoints[characteristic];
        }
        
        public void SpendOn(Characteristic characteristic, int amount) {
            if (available < amount) {
                Debug.LogError($"Not enough characteristic points to spend. You have {available} points, " +
                               $"but you want to spend {amount} points on {characteristic}");
                return;
            }
            available -= amount;
            spentCharacteristicPoints[characteristic] += amount;
        }
        
        public void Refund(Characteristic characteristic, int amount) {
            if (!spentCharacteristicPoints.ContainsKey(characteristic)) {
                Debug.LogError($"The characteristic {characteristic} is not spent yet");
                return;
            }
            var refundedAmount = Mathf.Min(spentCharacteristicPoints[characteristic], amount);
            available += refundedAmount;
            spentCharacteristicPoints[characteristic] -= refundedAmount;
        }
        
        public void Refund(Characteristic characteristic) {
            if (!spentCharacteristicPoints.ContainsKey(characteristic)) {
                Debug.LogError($"The characteristic {characteristic} is not spent yet");
                return;
            }
            available += spentCharacteristicPoints[characteristic];
            spentCharacteristicPoints[characteristic] = 0;
        }
        
        public void RefundAll() {
            foreach (var characteristic in spentCharacteristicPoints.Keys.ToList()) {
                Refund(characteristic);
            }
            Assert.IsTrue(total == available && spentCharacteristicPoints.Values.Sum() == 0, $"RefundAll failed. Spent points: {spentCharacteristicPoints.Values.Sum()}, available: {available}, total: {total}");
        }
        
        private int GetSpentPoints() {
            return spentCharacteristicPoints.Values.Sum();
        }
        
#if UNITY_EDITOR
        internal void Validate() {
            var spentPoints = GetSpentPoints();
            
            if (spentPoints < 0) {
                Debug.LogError($"Spent characteristic points is negative: {spentPoints}");
            }
            
            if (spentPoints + available <= total) {
                return;
            }
            
            RefundAll();
            
            spentCharacteristicPoints.OnBeforeSerialize();
            
            if (available < 0) {
                Debug.LogError($"Available characteristic points is negative: {available}");
            }
        }  
#endif
    }
}
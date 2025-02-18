using System;
using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace ElectricDrill.SimpleRpgCore.Attributes
{
    [Serializable]
    public class AttributePointsTracker
    {
        [SerializeField, HideInInspector] int total; 
        [SerializeField, HideInInspector] int available;

        public int Available => available;

        [SerializeField, HideInInspector] private SerializableDictionary<Attribute, int> spentAttributePoints = new();
        
        internal SerializableDictionary<Attribute, int> SpentAttributePoints => spentAttributePoints;
        public Dictionary<Attribute, int>.KeyCollection SpentAttributesKeys => spentAttributePoints.Keys;

        internal void Init(int totalPoints) {
            total = totalPoints;
            available = total - spentAttributePoints.Values.Sum();
            if (available < 0) {
                RefundAll();
                Debug.LogWarning($"Available attribute points are negative: {available}. Refunding all points");
            }
        }
        
        public void AddPoints(int amount) {
            available += amount;
        }
        
        public long GetSpentOn(Attribute attribute) {
            return spentAttributePoints[attribute];
        }
        
        public void SpendOn(Attribute attribute, int amount) {
            if (available < amount) {
                Debug.LogError($"Not enough attribute points to spend. You have {available} points, " +
                               $"but you want to spend {amount} points on {attribute}");
                return;
            }
            available -= amount;
            spentAttributePoints[attribute] += amount;
        }
        
        public void Refund(Attribute attribute, int amount) {
            if (!spentAttributePoints.ContainsKey(attribute)) {
                Debug.LogError($"The attribute {attribute} is not spent yet");
                return;
            }
            var refundedAmount = Mathf.Min(spentAttributePoints[attribute], amount);
            available += refundedAmount;
            spentAttributePoints[attribute] -= refundedAmount;
        }
        
        public void Refund(Attribute attribute) {
            if (!spentAttributePoints.ContainsKey(attribute)) {
                Debug.LogError($"The attribute {attribute} is not spent yet");
                return;
            }
            available += spentAttributePoints[attribute];
            spentAttributePoints[attribute] = 0;
        }
        
        public void RefundAll() {
            foreach (var attribute in spentAttributePoints.Keys.ToList()) {
                Refund(attribute);
            }
            Assert.IsTrue(total == available && spentAttributePoints.Values.Sum() == 0, $"RefundAll failed. Spent points: {spentAttributePoints.Values.Sum()}, available: {available}, total: {total}");
        }
        
        private int GetSpentPoints() {
            return spentAttributePoints.Values.Sum();
        }
        
#if UNITY_EDITOR
        internal void Validate() {
            var spentPoints = GetSpentPoints();
            
            if (spentPoints < 0) {
                Debug.LogError($"Spent attribute points is negative: {spentPoints}");
            }
            
            if (spentPoints + available <= total) {
                return;
            }
            
            RefundAll();
            
            spentAttributePoints.OnBeforeSerialize();
            
            if (available < 0) {
                Debug.LogError($"Available attribute points is negative: {available}");
            }
        }  
#endif
    }
}
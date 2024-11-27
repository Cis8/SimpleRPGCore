using System;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [Serializable]
    public class CharacteristicPointsTracker
    {
        [SerializeField, HideInInspector] int available;

        public int Available => available;

        [SerializeField, HideInInspector] private SerializableDictionary<Characteristic, long> spentCharacteristicPoints = new();
        
        internal SerializableDictionary<Characteristic, long> SpentCharacteristicPoints => spentCharacteristicPoints;
        public Dictionary<Characteristic, long>.KeyCollection SpentCharacteristics => spentCharacteristicPoints.Keys;

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
    }
}
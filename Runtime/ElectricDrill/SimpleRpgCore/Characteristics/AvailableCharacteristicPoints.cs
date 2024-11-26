using System;
using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [Serializable]
    public class AvailableCharacteristicPoints
    {
        /*[SerializeField] int available;
        // this is internal so that the object that holds this class can initialize it in OnValidate or other methods
        [SerializeField] internal List<SerKeyValPair<Characteristic, int>> inspectorReservedSpentCharacteristicPoints;
        private CharacteristicSetInstance _spentCharacteristicPoints;
        
        public int Available => available;
        public CharacteristicSetInstance SpentCharacteristicPoints => _spentCharacteristicPoints;

        public void Add(int amount) {
            available += amount;
        }
        
        public void InitializeSpentCharacteristicPoints(CharacteristicSet characteristicSet) {
            _spentCharacteristicPoints = new CharacteristicSetInstance(characteristicSet);
            foreach (var statValuePair in inspectorReservedSpentCharacteristicPoints) {
                _spentCharacteristicPoints.AddValue(statValuePair.Key, statValuePair.Value);
            }
        }*/
        
        [SerializeField] int available;
        [SerializeField] SerializableDictionary<Characteristic, int> spentCharacteristicPoints = new();
        
        public SerializableDictionary<Characteristic, int> SpentCharacteristicPoints => spentCharacteristicPoints;

        public void Add(int amount) {
            available += amount;
        }
    }
}
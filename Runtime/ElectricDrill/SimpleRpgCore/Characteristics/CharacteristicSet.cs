using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Utils;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    [CreateAssetMenu(fileName = "New Characteristic Set", menuName = "Simple RPG/Characteristic Set")]
    public class CharacteristicSet : ScriptableObject, ICharacteristicContainer
    {
        [SerializeField] private SerializableHashSet<Characteristic> _characteristics = new();

        public IReadOnlyList<Characteristic> Characteristics => _characteristics.ToList();

        public Characteristic Get(Characteristic characteristic) {
            return _characteristics.First(c => c == characteristic);
        }

        public bool Contains(Characteristic characteristic) {
            return _characteristics.Contains(characteristic);
        }
        
#if UNITY_EDITOR
        private void OnEnable() {
            Characteristic.OnCharacteristicDeleted += HandleCharacteristicDeleted;
        }

        private void OnDisable() {
            Characteristic.OnCharacteristicDeleted -= HandleCharacteristicDeleted;
        }

        private void HandleCharacteristicDeleted(Characteristic deletedCharacteristic) {
            if (_characteristics.Contains(deletedCharacteristic)) {
                _characteristics.Remove(deletedCharacteristic);
            }
        }
#endif
    }
}
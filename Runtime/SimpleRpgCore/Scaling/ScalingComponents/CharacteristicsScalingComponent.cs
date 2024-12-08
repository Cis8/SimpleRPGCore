using System.Collections.Generic;
using System.Linq;
using ElectricDrill.SimpleRpgCore.Characteristics;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    [CreateAssetMenu(fileName = "New Characteristics Scaling Component", menuName = "Simple RPG/Scaling/Characteristics Component")]
    public class CharacteristicsScalingComponent : SoSetScalingComponentBase<CharacteristicSet, Characteristic>
    {
        protected override CharacteristicSet GetEntitySet(EntityCore entity) => entity.Characteristics.CharacteristicSet;

        protected override long GetEntityValue(EntityCore entity, Characteristic key) => entity.Characteristics.Get(key);
        
        protected override IEnumerable<Characteristic> GetSetItems() => _set.Characteristics;
        
#if UNITY_EDITOR
        protected override void OnEnable() {
            Characteristic.OnCharacteristicDeleted += HandleCharacteristicDeleted;
        }

        protected override void OnDisable() {
            Characteristic.OnCharacteristicDeleted -= HandleCharacteristicDeleted;
        }

        private void HandleCharacteristicDeleted(Characteristic deletedCharacteristic) {
            if (_scalingAttributeValues.Keys.Contains(deletedCharacteristic)) {
                _scalingAttributeValues.Remove(deletedCharacteristic);
            }
        }
#endif
    }
}
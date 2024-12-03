using System.Collections.Generic;
using ElectricDrill.SimpleRpgCore.Characteristics;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Scaling
{
    [CreateAssetMenu(fileName = "New Characteristics Scaling Component", menuName = "Simple RPG/Scaling/Characteristics Component")]
    public class CharacteristicsScalingComponent : SoSetScalingComponentBase<CharacteristicSet, Characteristic>
    {
        protected override CharacteristicSet GetEntitySet(EntityCore entity) => entity.Characteristics.CharacteristicSet;

        protected override long GetEntityValue(EntityCore entity, Characteristic key) => entity.Characteristics.Get(key);
        
        public override bool IsKeyNotNull(Characteristic key) => key;

        protected override IEnumerable<Characteristic> GetSetItems() => _set.Characteristics;
    }
}
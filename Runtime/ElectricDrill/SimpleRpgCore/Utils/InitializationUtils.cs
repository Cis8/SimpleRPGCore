using System.Collections.Generic;
using System.Linq;

namespace ElectricDrill.SimpleRpgCore.Utils
{
    public static class InitializationUtils
    {
        public static void RefreshInspectorReservedValues<KeyType, ValueT>(ref List<SerKeyValPair<KeyType, ValueT>> inspectorReservedValues, IEnumerable<KeyType> keys) {
            if (keys != null) {
                var tempValues = inspectorReservedValues ?? new List<SerKeyValPair<KeyType, ValueT>>();
                inspectorReservedValues = keys.Select(key => {
                    var existingValue = tempValues.FirstOrDefault(assignedValueKey => assignedValueKey.Key.Equals(key));
                    if (existingValue.Key == null)
                    {
                        return new SerKeyValPair<KeyType, ValueT>(key, default);
                    }

                    return existingValue;
                }).ToList();
            }
            else {
                inspectorReservedValues = null;
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace ElectricDrill.SimpleRpgCore.Utils
{
    public static class InitializationUtils
    {
        public static void RefreshInspectorReservedValues<TKey, TValue>(ref List<SerKeyValPair<TKey, TValue>> inspectorReservedValues, IEnumerable<TKey> keys) {
            if (keys != null) {
                var tempValues = inspectorReservedValues ?? new List<SerKeyValPair<TKey, TValue>>();
                inspectorReservedValues = keys.Select(key => {
                    var existingValue = tempValues.FirstOrDefault(assignedValueKey => assignedValueKey.Key.Equals(key));
                    if (existingValue.Key == null) {
                        return new SerKeyValPair<TKey, TValue>(key, default);
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
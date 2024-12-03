using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Characteristics
{
    public class CharacteristicAssetModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            Characteristic characteristic = AssetDatabase.LoadAssetAtPath<Characteristic>(assetPath);
            if (characteristic != null) {
                Characteristic.OnWillDeleteCharacteristic(characteristic);
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
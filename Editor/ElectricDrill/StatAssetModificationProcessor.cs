using ElectricDrill.SimpleRpgCore.Stats;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    public class StatAssetModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            Stat stat = AssetDatabase.LoadAssetAtPath<Stat>(assetPath);
            if (stat != null) {
                Stat.OnWillDeleteStat(stat);
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.Stats
{
    public class StatAssetModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            Stat stat = AssetDatabase.LoadAssetAtPath<Stat>(assetPath);
            if (stat != null)
            {
                Debug.Log($"Stat {stat.name} is about to be deleted");
                Stat.OnWillDeleteStat(stat);
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
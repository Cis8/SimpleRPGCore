using ElectricDrill.SimpleRpgCore.Attributes;
using UnityEditor;
using UnityEngine;

namespace ElectricDrill.SimpleRpgCore.CstmEditor
{
    public class AttributeAssetModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            Attribute attribute = AssetDatabase.LoadAssetAtPath<Attribute>(assetPath);
            if (attribute != null) {
                Attribute.OnWillDeleteAttribute(attribute);
            }
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
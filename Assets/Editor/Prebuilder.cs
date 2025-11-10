using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine;

[InitializeOnLoad]
public class PreBuildAddressables
{
    static PreBuildAddressables()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(BuildWithAddressables);
    }

    static void BuildWithAddressables(BuildPlayerOptions options)
    {
        try
        {
            AddressableAssetSettings.BuildPlayerContent();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Addressables build failed: " + e.Message);
        }

        BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(options);
    }
}

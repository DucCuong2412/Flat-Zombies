#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class GMSoftCustomMenu
{
    [MenuItem("GmSoft/Export/Custom Export")]
    static void Export()
    {
        Debug.Log($"export package...");
        AssetDatabase.ExportPackage(AssetDatabase.GetAllAssetPaths(), PlayerSettings.productName + ".unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
    }
}

#endif

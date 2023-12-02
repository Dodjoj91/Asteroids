using UnityEngine.AddressableAssets;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class AddressablesManager : Manager
{
    #region Variables

    [SerializeField] AssetReference test;
    const string enemyPackAssetOneName = "enemyPackOne";

    #endregion

    #region Load Functions

    void LoadPrefab(AssetReference group)
    {
        var handle = Addressables.LoadAssetAsync<IList<Object>>(group);
        handle.Completed += OnAssetLoadComplete;
    }

    public AsyncOperationHandle<IList<Object>> LoadAsyncGroup(ESpawnPreset eSpawnPreset)
    {
        AsyncOperationHandle<IList<Object>> op = new AsyncOperationHandle<IList<Object>>();

        switch (eSpawnPreset)
        {
            case ESpawnPreset.AssetPackOne:
                op = Addressables.LoadAssetsAsync<Object>(enemyPackAssetOneName, null);
                op.Completed += OnAssetLoadComplete;
                break;
            case ESpawnPreset.AssetPackTwo:
                break;
        }

        return op;
    }

    void OnAssetLoadComplete(AsyncOperationHandle<IList<Object>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var obj in handle.Result)
            {
                Object prefabInstance = obj;
            }
        }
        else
        {
            Debug.LogError("Failed to load prefab: + " + handle.DebugName);
        }
    }

    public static string GetAddressFromAssetReference(AssetReference reference)
    {
        var loadResourceLocations = Addressables.LoadResourceLocationsAsync(reference);
        var result = loadResourceLocations.WaitForCompletion();
        if (result.Count > 0)
        {
            string key = result[0].PrimaryKey;
            Addressables.Release(loadResourceLocations);
            return key;
        }

        Addressables.Release(loadResourceLocations);
        return string.Empty;
    }

    #endregion
}
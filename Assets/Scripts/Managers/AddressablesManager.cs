using UnityEngine.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressablesManager : Manager
{
    [SerializeField] private AddressableAssetGroup defaultGroup;
    [SerializeField] private AddressableAssetGroup enemyGroupOne;
    [SerializeField] private AddressableAssetGroup enemyGroupTwo;


    void LoadPrefab(AddressableAssetGroup group)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(defaultGroup);
        handle.Completed += OnPrefabLoadComplete;
    }

    public AsyncOperationHandle LoadAsyncGroup(ESpawnPreset eSpawnPreset)
    {
        AsyncOperationHandle op = new AsyncOperationHandle();
        
        switch (eSpawnPreset)
        {
            case ESpawnPreset.AssetPackOne:
                op = Addressables.LoadAssetAsync<GameObject>(enemyGroupOne); 
                break;
            case ESpawnPreset.AssetPackTwo:
                op = Addressables.LoadAssetAsync<GameObject>(enemyGroupTwo);
                break;
        }

        return op;
    }

    void OnPrefabLoadComplete(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefabInstance = handle.Result;
            // Instantiate and use the prefab as needed.
        }
        else
        {
            Debug.LogError($"Failed to load prefab: {handle.DebugName}");
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
}

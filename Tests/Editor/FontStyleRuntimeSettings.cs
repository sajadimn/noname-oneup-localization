using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FontStyleRuntimeSettings : LocalizationSettings
{
    private AsyncOperationHandle<LocalizationSettings> initialization;

    public override AsyncOperationHandle<LocalizationSettings> GetInitializationOperation()
    {
        if (!initialization.IsValid())
            initialization = Addressables.ResourceManager.CreateCompletedOperation<LocalizationSettings>(this, null);
        return initialization;
    }

    private void OnDestroy()
    {
        if (initialization.IsValid())
            Addressables.Release(initialization);
    }
}

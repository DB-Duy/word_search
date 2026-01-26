namespace Shared.SharedDevToDev.Internal
{
    public interface IDevToDevInitAsyncOperation
    {
        void StartInit();
        void OnSdkInitialized();
        void OnRemoteConfigInitialized();
    }
}
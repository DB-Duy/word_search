using Shared.Core.Async;

namespace Shared.Service.AudioAds
{
    public interface IAudioAdsService
    {
        IAsyncOperation TryPlay();
        void Stop();

        void Pause();
        void Resume();
    }
}
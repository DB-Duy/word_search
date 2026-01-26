using Shared.Core.Async;
using Shared.Service.AudioAds.Internal;
using Shared.View.AudioAds;

namespace Shared.Service.AudioAds
{
    public interface ISharedAudioAd
    {
        AudioAdSource AudioAdSource { get; }
        bool IsReadyToPlay { get; }
        IAsyncOperation Play(AudioAdPlacement placement);
        void MoveTo(AudioAdPlacement placement);
        void Stop();
        void Pause();
        void Resume();
    }
}
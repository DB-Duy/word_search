using Shared.Core.Async;
using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.Gadsme
{
    public interface IGadsmeService : IInitializable
    {
        bool IsAudioAdAvailable { get; }
        IAsyncOperation PlayAudioAd(string placement);
        void Stop();
        // void Pause();
        // void Resume();
    }
}
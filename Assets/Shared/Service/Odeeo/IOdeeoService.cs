#if ODEEO_AUDIO
using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.Odeeo
{
    public interface IOdeeoService : IInitializable
    {
        public string AppKey { get; }

        void Pause();
        void Resume();
    }
}
#endif
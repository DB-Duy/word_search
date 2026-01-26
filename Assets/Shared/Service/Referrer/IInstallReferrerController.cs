using Shared.Core.Handler.Corou.Initializable;
using Shared.Core.Repository.JsonType;

namespace Shared.Referrer
{
    public interface IInstallReferrerController : IInitializable
    {
#if GOOGLE_PLAY
        IInstallReferrerController Setup(IJsonRepository<ReferrerDetails> repository);
        void TrackReferrer();
#endif
    }
}

using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.S2S;

namespace Shared.Repository.S2S
{
    [Repository]
    public class S2SConfigRepository : FirebaseRemoteConfigRepository<S2SConfig>
    {
    }
}
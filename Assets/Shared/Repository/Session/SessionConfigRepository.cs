#if GOOGLE_PLAY || APPSTORE || APPSTORE_CHINA
using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Repository.RemoteConfig.Models;

namespace Shared.Repository.Session
{
    [Repository]
    public class SessionConfigRepository : StoreRemoteConfigRepository<SessionTimeoutConfig>
    {
    }
}
#endif
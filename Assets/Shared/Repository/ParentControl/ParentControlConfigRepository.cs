using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.ParentControl;

namespace Shared.Repository.ParentControl
{
    [Repository]
    public class ParentControlConfigRepository : FirebaseRemoteConfigRepository<ParentControlConfig>
    {
    }
}
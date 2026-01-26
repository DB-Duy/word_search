using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.InAppUpdate;

namespace Shared.Repository.InAppUpdate
{
    [Repository]
    public class InAppUpdateConfigRepository : StoreRemoteConfigRepository<InAppUpdateConfig>
    {
    }
}
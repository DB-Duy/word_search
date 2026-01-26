using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.Ump;

namespace Shared.Repository.Ump
{
    [Repository]
    public class UmpConfigRepository : StoreRemoteConfigRepository<UmpConfig>
    {
    }
}
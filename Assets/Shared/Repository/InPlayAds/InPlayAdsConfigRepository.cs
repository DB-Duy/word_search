using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.InPlayAds;

namespace Shared.Repository.InPlayAds
{
    [Repository]
    public class InPlayAdsConfigRepository : StoreRemoteConfigRepository<InplayAdsConfig>
    {
    }
}
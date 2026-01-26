using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.AudioAds;

namespace Shared.Repository.AudioAds
{
    [Repository]
    public class AudioAdsConfigRepository : StoreRemoteConfigRepository<AudioAdsConfig>
    {
    }
}
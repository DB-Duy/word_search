using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.Ads;

namespace Shared.Repository.Ads
{
    [Repository]
    public class MultipleInterstitialAdsConfigRepository : FirebaseRemoteConfigRepository<MultipleInterstitialAdsConfig>
    {
    }
}
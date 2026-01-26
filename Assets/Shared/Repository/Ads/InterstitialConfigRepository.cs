using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.Ads;

namespace Shared.Repository.Ads
{
    // new FirebaseRemoteConfigRepository<InterstitialConfig>(Constants.RemoteConfig.InterstitialConfig, defaultRemoteConfigController);
    [Repository]
    public class InterstitialConfigRepository : FirebaseRemoteConfigRepository<InterstitialConfig>
    {
    }
}
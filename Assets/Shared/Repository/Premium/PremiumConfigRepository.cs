using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.Premium;

namespace Shared.Repository.Premium
{
    //new FirebaseRemoteConfigRepository<SubscriptionConfig>(Constants.RemoteConfig.Subscription, defaultRemoteConfigController);
    [Repository]
    public class PremiumConfigRepository : FirebaseRemoteConfigRepository<PremiumConfig>
    {
    }
}
using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.SharedBright;

namespace Shared.Repository.SharedBright
{
    [Repository]
    public class BrightSdkConfigRepository : FirebaseRemoteConfigRepository<BrightSDKConfig>
    {
        
    }
}
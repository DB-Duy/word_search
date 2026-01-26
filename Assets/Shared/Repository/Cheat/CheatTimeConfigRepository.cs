#if CHEAT_TIME_DETECTOR
using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.Cheat;

namespace Shared.Repository.Cheat
{
    [Repository]
    public class CheatTimeConfigRepository : FirebaseRemoteConfigRepository<CheatTimeConfig>
    {
        
    }
}
#endif
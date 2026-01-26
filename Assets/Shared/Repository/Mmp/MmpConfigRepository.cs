using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.Mmp;
using Shared.Utils;

namespace Shared.Repository.Mmp
{
    [Repository]
    public class MmpConfigRepository : FirebaseRemoteConfigRepository<List<MmpConfig.EventConfig>>
    {
    }
}
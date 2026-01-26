using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.FirebaseRemoteConfig
{
    public interface IFirebaseRemoteConfigService : IInitializable
    {
        IHandler<Dictionary<string, string>> RemoteConfigChangeHandler { get; }
        IAsyncOperation Fetch();
        void ProcessAbTestingDetector();
    }
}
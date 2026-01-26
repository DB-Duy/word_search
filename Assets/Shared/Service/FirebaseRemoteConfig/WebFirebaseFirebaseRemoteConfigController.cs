#if FIREBASE_WEBGL

using Shared.Common;

namespace Shared.SharedFirebase.Web
{
    public class WebFirebaseFirebaseRemoteConfigController : IFirebaseRemoteConfigService
    {
        public bool IsInitialized { get; private set; }
        
        public IAsyncOperation Initialize()
        {
            return new SharedAsyncOperation().Success();
        }
        
        public IAsyncOperation Fetch()
        {
            return new SharedAsyncOperation().Success();
        }

        
    }
}
#endif
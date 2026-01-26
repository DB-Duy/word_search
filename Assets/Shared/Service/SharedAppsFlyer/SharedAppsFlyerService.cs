#if APPS_FLYER
using Shared.Core.Async;
using Shared.Core.IoC;
using UnityEngine;

namespace Shared.Service.SharedAppsFlyer
{
    [Service]
    public class SharedAppsFlyerService : IAppsFlyerService
    {
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;

            var go = new GameObject("AppsFlyerObject");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<AppsFlyerObjectScript>();
            _initOperation = new SharedAsyncOperation();
            IsInitialized = true;
            return _initOperation;
        }

    }
}
#endif
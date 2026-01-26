#if FIREBASE_WEBGL
using Shared.Common;
using Shared.Utils;
using UnityEngine;

namespace Shared.SharedFirebase.Web
{
    public class WebFirebaseController : IFirebaseService
    {
        private const string TAG = "WebFirebaseController";
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            SharedLogger.Log($"{TAG}->Initialize");
            Application.ExternalCall("InitializeFirebaseInstant");
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }
    }
}
#endif
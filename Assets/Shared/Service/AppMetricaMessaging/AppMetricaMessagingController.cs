#if APPMETRICA_PUSH

using Shared.Common;
using Shared.Utils;

namespace Shared.SharedAppMetrica
{
      public class AppMetricaMessagingController : IAppMetricaMessagingController
    {
        private const string TAG = "AppMetricaMessagingController";

        private IYandexMetricaPush _platformInstance;
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;

        public IAsyncOperation Initialize()
        {
            SharedLogger.Log($"{TAG}->Initialize");
            if (_initOperation != null) return _initOperation;
            _platformInstance = _CreatePlatformInstance();
            _platformInstance.Initialize();
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }

        private IYandexMetricaPush _CreatePlatformInstance()
        {
#if UNITY_EDITOR
            return new YandexAppMetricaPushDummy();
#elif UNITY_ANDROID
            return new YandexMetricaPushAndroid();
#elif UNITY_IOS
            return new YandexMetricaPushIOS();
#endif
        }
    }
}
#endif
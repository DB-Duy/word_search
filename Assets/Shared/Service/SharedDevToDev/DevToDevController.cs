#if DEV_TO_DEV
using DevToDev.Analytics;
using DevToDev.Analytics.ABTest;
using Shared.Common;
using Shared.SharedDevToDev.Internal;
using Shared.Utils;
using UnityEngine;

namespace Shared.SharedDevToDev
{
    [DisallowMultipleComponent]
    public class DevToDevController : MonoBehaviour, IDTDRemoteConfigListener, IDevToDevController
    {
        private const string Tag = "DevToDevController";

        private readonly string[] _logFilters = { "DevToDev" };

        private IDevToDevConfig _config;
        private DevToDevInitAsyncOperation _initAsyncOperation;
        
        public bool IsInitialized { get; private set; }

        public IDevToDevController Setup(IDevToDevConfig config)
        {
            SharedLogger.Log($"{Tag}->Setup: {config}");
            _config = config;
            return this;
        }

        public IAsyncOperation Initialize()
        {
            SharedLogger.Log($"{Tag}->Initialize", _logFilters);
            if (_initAsyncOperation != null) return _initAsyncOperation;
            _initAsyncOperation = new DevToDevInitAsyncOperation(15f);
            _initAsyncOperation.StartInit();

            DTDRemoteConfig.RemoteConfigWaiting = 10.0f;
            DTDRemoteConfig.GroupDefinitionWaiting = 10.0f;
            DTDAnalytics.SetInitializationCompleteCallback(() =>
            {
                SharedLogger.Log($"{Tag}->DTDAnalytics.SetInitializationCompleteCallback", _logFilters);
                _initAsyncOperation.OnSdkInitialized();
                IsInitialized = true;
            });
            var config = new DTDAnalyticsConfiguration
            {
                ApplicationVersion = Application.version,
                LogLevel = SharedSymbols.IsDevelopment ? DTDLogLevel.Debug : DTDLogLevel.No,
                TrackingAvailability = DTDTrackingStatus.Enable,
                CurrentLevel = _config.Level,
                UserId = SystemInfo.deviceUniqueIdentifier
            };
            SharedLogger.Log($"{Tag}->Initialize: {config}", _logFilters);
            DTDRemoteConfig.Defaults = _config.DefaultRemoteConfigDictionary;
            DTDAnalytics.InitializeWithAbTests(_config.AppId, config, this);
            return _initAsyncOperation;
        }

        public void OnReceived(DTDRemoteConfigReceiveResult result)
        {
            SharedLogger.Log($"{Tag}->OnReceived: {result.ToString()}", _logFilters);
            if (result is DTDRemoteConfigReceiveResult.Failure or DTDRemoteConfigReceiveResult.Empty) _initAsyncOperation.OnRemoteConfigInitialized();
        }

        public void OnPrepareToChange()
        {
            SharedLogger.Log($"{Tag}->OnPrepareToChange", _logFilters);
        }

        public void OnChanged(DTDRemoteConfigChangeResult result, string exceptionText = null)
        {
            SharedLogger.Log($"{Tag}->OnChanged: {result.ToString()} {exceptionText}", _logFilters);
            switch (result)
            {
                case DTDRemoteConfigChangeResult.Failure:
                    break;
                case DTDRemoteConfigChangeResult.Success:
                    DTDRemoteConfig.ApplyConfig();
                    SharedLogger.Log($"{Tag}->OnChanged: DTDRemoteConfig.ApplyConfig();", _logFilters);
                    break;
            }
            _initAsyncOperation.OnRemoteConfigInitialized();
        }
    }
}
#endif
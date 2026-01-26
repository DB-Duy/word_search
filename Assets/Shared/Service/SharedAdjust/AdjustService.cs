#if ADJUST
using AdjustSdk;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Entity.SharedAdjust;
using Shared.Repository.Adjust;
using Shared.Service.SharedAdjust.Handler;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.SharedAdjust
{
    /// <summary>
    /// https://github.com/adjust/unity_sdk#qs-get-sdk
    /// </summary>
    [Service]
    public class AdjustService : IAdjustService, ISharedUtility, ISharedLogTag
    {
        public string LogTag => SharedLogTag.Adjust;
        
        [Inject] private IConfig _config;
        [Inject] private AdjustRepository _adjustRepository;
        
        private IHandler _referrerHandler;
        private IHandler ReferrerHandler => _referrerHandler ??= SequenceHandlerChain.CreateChainFromType<IAdjustReferrerHandler>();

        private IHandler<AdjustAttribution> _attributionHandler;
        private IHandler<AdjustAttribution> AttributionHandler => _attributionHandler ??= SequenceHandlerChain<AdjustAttribution>.CreateChainFromType<IAdjustAttributionHandler>();
        
        public bool IsInitialized { get; private set; }
        private AdjustEntity _adjustEntity = new();

        public IAsyncOperation Initialize()
        {
            if (IsInitialized) return new SharedAsyncOperation().Success();
            IsInitialized = true;
            this.LogInfo();
            _adjustEntity = _adjustRepository.Get() ?? new AdjustEntity();
#if UNITY_ANDROID
            _adjustEntity.AndroidId = this.GetAndroidId();
#endif

            var config = _NewAdjustConfig();
            config.AttributionChangedDelegate = attribution =>
            {
                AttributionHandler?.Handle(attribution);
                _adjustEntity.AdjustAttribution = attribution;
                _adjustRepository.Save(_adjustEntity);
            };
            this.LogInfo(nameof(config), config.ToDict());
            
            ReferrerHandler?.Handle();
            
#if ADJUST_OAID
            this.LogInfo("call", "com.adjust.sdk.oaid.AdjustOaidAndroid.ReadOaid();");
            com.adjust.sdk.oaid.AdjustOaidAndroid.ReadOaid();
            // Error during msa sdk initialization dlopen failed: library "libmsaoaidsec.so" not found
#endif
            
            Adjust.InitSdk(config);
            
            Adjust.GetAttribution(attribution =>
            {
                this.LogInfo(SharedLogTag.Adjust, "callback", "Adjust.GetAttribution", nameof(attribution), attribution);
                AttributionHandler?.Handle(attribution);
                _adjustEntity.AdjustAttribution = attribution;
                _adjustRepository.Save(_adjustEntity);
            });
#if UNITY_ANDROID
            Adjust.GetGoogleAdId(googleAdId =>
            {
                this.LogInfo(SharedLogTag.Adjust, "callback", "Adjust.GetGoogleAdId", nameof(googleAdId), googleAdId);
                _adjustEntity.GoogleAdId = googleAdId;
                _adjustRepository.Save(_adjustEntity);
            });
#endif
            Adjust.GetAdid(adid =>
            {
                this.LogInfo(SharedLogTag.Adjust, "callback", "Adjust.GetAdid", nameof(adid), adid);
                _adjustEntity.AdjustId = adid;
                _adjustRepository.Save(_adjustEntity);
            });
            Adjust.GetSdkVersion(sdkVersion =>
            {
                this.LogInfo(SharedLogTag.Adjust, "callback", "Adjust.GetSdkVersion", nameof(sdkVersion), sdkVersion);
                _adjustEntity.SdkVersion = sdkVersion;
                _adjustRepository.Save(_adjustEntity);
            });
            return new SharedAsyncOperation().Success();
        }

        public AdjustEntity Get() => _adjustRepository.Get();

        private AdjustConfig _NewAdjustConfig()
        {
            var config = new AdjustConfig(_config.AdjustAppToken, SharedSymbols.IsDevelopment ? AdjustEnvironment.Sandbox : AdjustEnvironment.Production)
            {
                LogLevel = SharedSymbols.IsDevelopment ? AdjustLogLevel.Verbose : AdjustLogLevel.Suppress,
                FbAppId = _config.FacebookId,
                IsAdServicesEnabled = true,
                IsIdfaReadingEnabled = true
            };

#if UNITY_ANDROID
            config.ExternalDeviceId = this.GetAndroidId();
            this.LogInfo(nameof(config.ExternalDeviceId), config.ExternalDeviceId);
#endif
            return config;
        }
    }
}
#endif
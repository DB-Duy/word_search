#if LEVEL_PLAY && AD_QUALITY
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.AdQuality
{
    [Service]
    public class AdQualityService : IAdQualityService, ISAdQualityInitCallback, ISharedUtility, ISharedLogTag
    {
        [Inject] private IConfig _config;
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            this.LogInfo(nameof(_config.IronSourceAppKey), _config.IronSourceAppKey);
            var adQualityConfig = new ISAdQualityConfig
            {
                AdQualityInitCallback = this,
                UserId = SystemInfo.deviceUniqueIdentifier,
                TestMode = SharedSymbols.IsDevelopment,
                LogLevel = SharedSymbols.IsDevelopment ? ISAdQualityLogLevel.VERBOSE : ISAdQualityLogLevel.DEBUG
            };
            IronSourceAdQuality.Initialize(_config.IronSourceAppKey, adQualityConfig);
            return _initOperation;
        }

        // Callback
        public void adQualitySdkInitFailed(ISAdQualityInitError adQualityInitError, string errorMessage)
        {
            this.LogError(nameof(adQualityInitError), adQualityInitError, nameof(errorMessage), errorMessage);
            _initOperation.Fail($"{adQualityInitError.ToString()} - {errorMessage}");
            IsInitialized = false;
        }

        // Callback
        public void adQualitySdkInitSuccess()
        {
            this.LogInfo();
            _initOperation.Success();
            IsInitialized = true;
        }

        public string LogTag => SharedLogTag.AdQualityNLevelPlay;
    }
}
#endif
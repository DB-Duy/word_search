#if LEVEL_PLAY && !UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.Handler.Corou;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Entity.Config;
using Shared.Entity.ParentControl;
using Shared.Entity.Ump;
using Shared.Repository.Ump;
using Shared.Service.Ads.SharedLevelPlay.PostHandler;
using Shared.Service.Ads.SharedLevelPlay.PreHandler;
using Shared.Service.ParentControl;
using Shared.Service.SharedCoroutine;
using Shared.Service.Tracking;
using Shared.Utils;
using Unity.Services.LevelPlay;
using UnityEngine;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay
{
    /// <summary>
    /// https://developers.is.com/ironsource-mobile/unity/aps-integration-guide/#step-5
    /// https://ogury-ltd.gitbook.io/ironsource-unity/
    /// https://developers.is.com/ironsource-mobile/unity/unity-plugin/#step-1
    /// </summary>
    [Service]
    public class LevelPlayAdService : IAdService, ISharedUtility, IUnityOnApplicationPause
    {
        [Inject] private IConfig _config;
        [Inject] private UsPrivacyStringRepository _usPrivacyStringRepository;
        [Inject] private ITrackingService _trackingService;
        [Inject] private IRewardAd _rewardAd;
        [Inject] private IInterstitialAd _interstitialAd;
        [Inject] private IBannerAd _bannerAd;
        [Inject(Optional = true)] private IParentControlService _parentControlService;
        
        public IBannerAd BannerAd => _bannerAd;
        public IInterstitialAd InterstitialAd => _interstitialAd;
        public IRewardAd RewardAd => _rewardAd;

        public bool IsInitialized { get; private set; } = false;

        private ICoroutineHandler _preInitHandler;
        private ICoroutineHandler PreInitHandler => _preInitHandler ??= ParallelCoroutineHandlerChain.CreateChainFromType<ILevelPlayPreInitHandler>();

        private ICoroutineHandler _postInitHandler;
        private ICoroutineHandler PostInitHandler => _postInitHandler ??= ParallelCoroutineHandlerChain.CreateChainFromType<ILevelPlayPostInitHandler>();
        
        private static readonly Dictionary<Gender, string> GenderMap = new()
        {
            { Gender.Male , "male" },
            { Gender.Female , "female"}
        };

        private IAsyncOperation _initOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();

            IronSourceEvents.onImpressionDataReadyEvent += _OnImpressionDataReadyEvent;
            
            var doNotSell = _usPrivacyStringRepository.Get() == UsPrivacyValue.Const1YynOn ? "true" : "false";

            IronSource.Agent.setUserId(SystemInfo.deviceUniqueIdentifier);
            IronSource.Agent.setConsent(true);
            IronSource.Agent.setMetaData("do_not_sell", doNotSell);
            IronSource.Agent.setMetaData("is_child_directed", "false");
#if LOG_INFO
            IronSource.Agent.validateIntegration();
            IronSource.Agent.setMetaData("is_test_suite", "enable");
#endif
            IronSource.Agent.setAdaptersDebug(SharedSymbols.IsDevelopment);
#if UNITY_ANDROID
            IronSource.Agent.setMetaData("Facebook_IS_CacheFlag", "ALL");
#endif
            // https://developers.is.com/ironsource-mobile/unity/bigo-change-log/
            IronSource.Agent.shouldTrackNetworkState(true);
            IronSource.Agent.setManualLoadRewardedVideo(true);

            // Set segment with RAM info
            var segment = new IronSourceSegment
            {
                customs = new Dictionary<string, string> { { "deviceRAM", SystemInfo.systemMemorySize.ToString() }}
            };
            if (_parentControlService != null)
            {
                var step = _parentControlService.GetStep();
                if (step == Step.Granted)
                {
                    var info = _parentControlService.Get();
                    segment.age = info.Age;
                    if (GenderMap.TryGetValue(info.Gender, out var value)) segment.gender = value;    
                }    
            }
            
            IronSource.Agent.setSegment(segment);
            // End Segment config

            this.StartSharedCoroutine(PreInitHandler.Handle());
            LevelPlay.OnInitSuccess += _OnInitSuccess;
            LevelPlay.OnInitFailed += _OnInitFailed;
            LevelPlay.Init(_config.IronSourceAppKey, SystemInfo.deviceUniqueIdentifier);
            this.LogInfo(SharedLogTag.AdNLevelPlay_, "log", "End");
            return _initOperation;
        }

        private void _OnInitSuccess(LevelPlayConfiguration configuration)
        {
            this.LogInfo(SharedLogTag.AdNLevelPlay_, nameof(configuration), configuration);
            AdFlag.IsInitialized = true;
            IsInitialized = true;
            _initOperation.Success();
            if (PostInitHandler != null) this.StartSharedCoroutine(PostInitHandler.Handle());
        }

        private void _OnInitFailed(LevelPlayInitError error)
        {
            this.LogInfo(SharedLogTag.AdNLevelPlay_, "error", error);
            IsInitialized = false;
            _initOperation.Fail("_OnInitFailed");
            this.StartSharedCoroutine(_TryReInit());
        }

        private IEnumerator _TryReInit()
        {
            while (Application.internetReachability == NetworkReachability.NotReachable) yield return null;
            this.LogInfo("f", nameof(_TryReInit));
            LevelPlay.Init(_config.IronSourceAppKey, SystemInfo.deviceUniqueIdentifier);
        }

        private void _OnImpressionDataReadyEvent(IronSourceImpressionData impressionData)
        {
            this.LogInfo(SharedLogTag.AdNLevelPlay_, nameof(impressionData), impressionData);
            _trackingService.Track(impressionData.ToAdRevenueEvent());
        }

        public void OnApplicationPause(bool isPaused)
        {
            this.LogInfo(SharedLogTag.AdNLevelPlay_, nameof(isPaused), isPaused);
            IronSource.Agent.onApplicationPause(isPaused);
        }

        public void LaunchTestSuite()
        {
            this.LogInfo(SharedLogTag.AdNLevelPlay_);
            IronSource.Agent.launchTestSuite();
        }
    }
}
#endif
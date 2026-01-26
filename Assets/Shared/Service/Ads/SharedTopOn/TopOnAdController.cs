#if TOPON
using System.Collections.Generic;
using System.Diagnostics;
using AnyThinkAds.Api;
using Newtonsoft.Json;
using Shared.Ads.SharedTopOn.Banner;
using Shared.Ads.SharedTopOn.Interstitial;
using Shared.Ads.SharedTopOn.PreHandler;
using Shared.Ads.SharedTopOn.Reward;
using Shared.Common;
using Shared.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Shared.Ads.SharedTopOn
{
    /// <summary>
    /// https://bitbucket.org/indiezvn/ng-mobile/src/b192_HW/Assets/Plugins/Android/UnityPlayerActivityWithANRWatchDog.java
    /// </summary>
    [DisallowMultipleComponent]
    public class TopOnAdController : MonoBehaviour, ITopOnAdController, ATSDKInitListener
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "TopOnAdController";
        
        private readonly HashSet<ITopOnImpressionDataReadyEventHandler> _handlers = new();
        public IAdConfig AdConfig { get; private set; }
        private ITopOnAdConfig _topOnAdConfig;
        
        public IBannerAd BannerAd { get; private set; }
        public IInterstitialAd InterstitialAd { get; private set;}
        public IRewardAd RewardAd { get; private set;}

        public bool IsInitialized { get; private set; }
        public bool IsNotInitialized => !IsInitialized;

        private IAsyncOperation _initOperation;

        private readonly HashSet<IPreInitHandler> _preInitHandlers = new();

        public ITopOnAdController AddPreInitHandlers(params IPreInitHandler[] handlers)
        {
            _preInitHandlers.AddRange(handlers);
            return this;
        }

        public ITopOnAdController AddImpressionHandlers(params ITopOnImpressionDataReadyEventHandler[] handlers)
        {
            _handlers.AddRange(handlers);
            return this;
        }
        
        public IAdController SetUp(IAdConfig adConfig)
        {
            SharedLogger.Log($"{TAG}->SetUp: {adConfig}");
            AdConfig = adConfig;
            _topOnAdConfig = (ITopOnAdConfig)adConfig;
            BannerAd = gameObject.AddComponent<TopOnBannerAd>();
            InterstitialAd = gameObject.AddComponent<TopOnInterstitialAd>();
            RewardAd = gameObject.AddComponent<TopOnRewardedAd>();
            return this;
        }

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            
            ATBannerAd.Instance.client.onAdImpressEvent += (a, erg) => _OnImpressionSuccessEvent("ATBannerAd.Instance.client.onAdImpressEvent", erg);
            ATBannerAd.Instance.client.onAdAutoRefreshEvent += (a, erg) => _OnImpressionSuccessEvent("ATBannerAd.Instance.client.onAdAutoRefreshEvent", erg);
            ATInterstitialAd.Instance.client.onAdShowEvent += (a, erg) => _OnImpressionSuccessEvent("ATInterstitialAd.Instance.client.onAdShowEvent", erg);
            ATRewardedVideo.Instance.client.onAdVideoStartEvent += (a, erg) => _OnImpressionSuccessEvent("ATRewardedVideo.Instance.client.onAdVideoStartEvent", erg);
            
            BannerAd.Setup(this);
            InterstitialAd.Setup(this);
            RewardAd.Setup(this);
            
            _SetAdStatus();
            ATSDKAPI.setLogDebug(SharedSymbols.IsDevelopment);
            var gdprApplies = SharedCore.Instance.UmpController.IABTCF_gdprApplies == 1;
            ATSDKAPI.setGDPRLevel(gdprApplies ? ATSDKAPI.PERSONALIZED : ATSDKAPI.NONPERSONALIZED);
            ATSDKAPI.initCustomMap(new Dictionary<string, string> { { "user_id", SystemInfo.deviceUniqueIdentifier } });
            ATSDKAPI.initSDK(_topOnAdConfig.TopOnAppId, _topOnAdConfig.TopOnAppKey, this);

            _TestModeDeviceInfo();
            _Debug();

            foreach (var handler in _preInitHandlers) StartCoroutine(handler.Handle());
            return _initOperation;
        }
        
        public void initSuccess()
        {
            IsInitialized = true;
            _initOperation.Success();
        }

        public void initFail(string message)
        {
            IsInitialized = false;
            _initOperation.Fail(message);
        }

        private void _OnImpressionSuccessEvent(string function, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{TAG}->{function} ->_OnImpressionSuccessEvent", erg);
            foreach (var handler in _handlers) handler.Handle(erg);
        }

        public void LaunchTestSuite()
        {
            SharedLogger.Log($"{TAG}->LaunchTestSuite");
        }

        [Conditional("LOG_INFO")]
        private void _Debug()
        {
            var logDict = new Dictionary<string, object>()
            {
                {"gdprLevel", ATSDKAPI.getGDPRLevel()}
            };
            Debug.Log($"{TAG}->_Debug: {JsonConvert.SerializeObject(logDict)}");
        }

        [Conditional("LOG_INFO")]
        private void _TestModeDeviceInfo()
        {
            SharedLogger.Log($"{TAG}->TestModeDeviceInfo");
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            var binder = new AndroidJavaClass("com.unity3d.player.SharedTopOn");
            binder.CallStatic("testModeDeviceInfo", currentActivity);
        }
        
        public void _SetAdStatus()
        {
            SharedLogger.Log($"{TAG}->SetAdStatus");
            var binder = new AndroidJavaClass("com.unity3d.player.SharedTopOn");
            binder.CallStatic("SetAdStatus");
        }
    }
}
#endif
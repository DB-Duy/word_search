using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.Handler.Corou.Initializable;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Service.Loading.Handlers;
using Shared.Service.SharedCoroutine;
using Shared.Service.Tracking;
using Shared.Service.Tracking.Common;
using Shared.Service.Tracking.TrackingEvent.Game;
using Shared.Utils;
using Shared.View.Ad;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

namespace Shared.Service.Loading
{
    [Service]
    public class LoadingService : ILoadingService, IIoC, ISharedUtility
    {
        public enum TaskType
        {
            Free,
            TimeOut,
            Hard
        }
        [Inject] private IConfig _config;
        
        private IHandler _preInitHandler;
        public IHandler PreInitHandler => _preInitHandler ??= SequenceHandlerChain.CreateChainFromType<IPreInitHandler>();

        private IHandler _postInitHandler;
        private IHandler PostInitHandler => _postInitHandler ??= SequenceHandlerChain.CreateChainFromType<IPostInitHandler>();

        private IAsyncOperation<LoadOperation> _loadOperation;

        public IAsyncOperation<LoadOperation> Load()
        {
            if (_loadOperation != null) return _loadOperation;
            _loadOperation = new SharedAsyncOperation<LoadOperation>(new LoadOperation());

            this.ShowFps();
            this.StartSharedCoroutine(_LoadInternal());
            return _loadOperation;
        }

        public Action OnLoadComplete { get; set; }

        private IEnumerator _LoadInternal()
        {
            PreInitHandler?.Handle();
            var initializables = InitializableRegistry.GetOrderedInitializables(DefaultServiceOrders);
            List<InitializableHandler> handlers = new();
            foreach (var initializable in initializables)
            {
                if (initializable == null)
                {
                    this.LogWarning("LoadingService", "_LoadInternal", "Skipping null initializable");
                    continue;
                }

                var className = initializable.GetType().FullName;

                var taskType = TaskTypes.Get(className, TaskType.TimeOut);
                switch (taskType)
                {
                    case TaskType.Free:
                    {
                        handlers.Add(new InitializableHandler(initializable, isFreeTask: true));
                        break;
                    }
                    case TaskType.TimeOut:
                    {
                        handlers.Add(new InitializableHandler(initializable, timeout: _config.LoadingTaskTimeout));
                        break;
                    }
                    case TaskType.Hard:
                    {
                        handlers.Add(new InitializableHandler(initializable));
                        break;
                    }
                }
            }

            var handler = new InitializableHandlerChain( 
                completeAction: (totalTime, timeDict) => this.Track(new LoadingToHomeParams(totalTime, timeDict)),
                stepCompleteAction: (step) => _UpdateLoadingProcess(step: step, total: handlers.Count),
                handlers.ToArray());
            yield return handler.Handle();
            PostInitHandler?.Handle();
            _loadOperation.Data.LoadState = LoadState.Finished;
            _loadOperation.Success();
            OnLoadComplete?.Invoke();
            this.LogInfo("exit", "Done, Changing Scene");
#if LOG_INFO
            QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
            Application.targetFrameRate = _config.TargetFrameRate;
            this.LogInfo("f", "Force set FPS", "QualitySettings.vSyncCount", QualitySettings.vSyncCount, "Application.targetFrameRate", Application.targetFrameRate, "_config.TargetFrameRate", _config.TargetFrameRate);
#endif
        }

        private void _UpdateLoadingProcess(int step, int total)
        {
            var floatStep = (float)step;
            var percent = (floatStep / total) * 100f;
            _loadOperation.Data.LoadingPercent = (int)percent;
        }

        /// <summary>
        /// Below function will be used for loading propress.
        /// </summary>
        public bool IsInitialized { get; private set; }
        public IAsyncOperation Initialize()
        {
            if (IsInitialized) return new SharedAsyncOperation().Success();
            IsInitialized = true;
            this.Track(TrackingScreen.Loading);
#if LOG_INFO
            Debug.LogFormat($"{nameof(Screen.safeArea)}: {Screen.safeArea.ToString()}");
            _LogIosDeviceInfo();
#endif
            return new SharedAsyncOperation().Success();
        }

        [Conditional("LOG_INFO")]
        private void _LogIosDeviceInfo()
        {
            var logDict = new Dictionary<string, string>()
            {
                { "scaleFactor", $"{IosScaleFactorPlugin.GetScaleFactor()}" },
                { "nativeScaleFactor", $"{IosScaleFactorPlugin.GetNativeScaleFactor()}" },
                { "ScreenWidthInPixels", $"{IosScaleFactorPlugin.GetScreenWidthInPixels()}" },
                { "ScreenHeightInPixels", $"{IosScaleFactorPlugin.GetScreenHeightInPixels()}" },
                { "ScreenWidthInPoints", $"{IosScaleFactorPlugin.GetScreenWidthInPoints()}" },
                { "ScreenHeightInPoints", $"{IosScaleFactorPlugin.GetScreenHeightInPoints()}" },
                { "Screen.width", $"{Screen.width}"},
                { "Screen.height", $"{Screen.height}"},
                { "Screen.safeArea", Screen.safeArea.ToString() }
            };
            Debug.Log($"iOS Device: {JsonConvert.SerializeObject(logDict)}");
        }

        private string[] DefaultServiceOrders { get; } = 
        {
            "Shared.Service.Audio.AudioService",
            "Shared.Service.SharedAppsFlyer.SharedAppsFlyerService",
            "Shared.Service.SharedAdjust.AdjustService",
            "Shared.Service.UnityGamingService.UnityGamingService",
            
            "Shared.Service.Firebase.MobileFirebaseService",
            "Shared.Service.FirebaseRemoteConfig.MobileFirebaseRemoteConfigService",
            
            "Shared.Service.AppMetrica.AppMetricaService",
            "Shared.Service.Mmp.MmpService",
            
            "Shared.Service.GameInterrupt.GameInterruptService",
            "Shared.Service.Session.Android.AndroidSessionService",
            "Shared.Service.Session.DefaultSessionService",
            "Shared.Service.Messaging.FirebaseMessagingService",// Do iOS sẽ show popup yêu cầu user xác thực, nên sẽ nhảy vào OnApplicationPause.
            "Shared.Service.Loading.LoadingService",
            
            "Shared.Service.ParentControl.ParentControlService",
            "Shared.Service.Ump.SharedUmpService",
            "Shared.Service.TrackingAuthorization.IosTrackingAuthorizationService",
            "Shared.Service.Facebook.FacebookService",
            
            "Shared.Service.Ads.SharedLevelPlay.LevelPlayAdService",
            "Shared.Service.Ads.SharedLevelPlay.Mrec.LevelPlayMrecAdService",
            "Shared.Service.AdQuality.AdQualityService",
            "Shared.Service.Ads.Fake.FakeAdService",
            "Shared.Service.Ads.Fake.FakeBannerAd",
            "Shared.Service.Ads.Fake.FakeInterstitialAd",
            "Shared.Service.Ads.Fake.FakeRewardAd",
            // IAP
            "Shared.Service.Iap.FakeIapService",
            // In Play Ads.
            "Shared.Service.Adverty.Adverty5Service",
            "Shared.Service.Adverty.Adverty4Service",
            "Shared.Service.Gadsme.GadsmeService",
            // Audio Ads
            "Shared.Service.AudioMob.AudioMobService",
            "Shared.Service.Odeeo.OdeeoService",
            "Shared.Service.AudioAds.AudioAdsService",
            
            "Shared.Service.ErrorTracking.ErrorTrackingService",
            "Shared.Service.Audio.AudioMediator",
        };
        
        private Dictionary<string, TaskType> TaskTypes { get; } = new () 
        {
            { "Shared.Service.SharedAppsFlyer.SharedAppsFlyerService", TaskType.Free },
            { "Shared.Service.SharedAdjust.AdjustService", TaskType.Free },
            { "Shared.Service.UnityGamingService.UnityGamingService", TaskType.Free },
            { "Shared.Service.TrackingAuthorization.IosTrackingAuthorizationService", TaskType.Hard },
            
            { "Shared.Service.Firebase.MobileFirebaseService", TaskType.TimeOut },
            { "Shared.Service.Messaging.FirebaseMessagingService", TaskType.TimeOut },
            { "Shared.Service.FirebaseRemoteConfig.MobileFirebaseRemoteConfigService", TaskType.TimeOut },
            
            { "Shared.Service.AppMetrica.AppMetricaService", TaskType.TimeOut },
            { "Shared.Service.GameInterrupt.GameInterruptService", TaskType.TimeOut },
            { "Shared.Service.Session.Android.AndroidSessionService", TaskType.TimeOut },
            { "Shared.Service.Session.DefaultSessionService", TaskType.Hard },
            { "Shared.Service.Loading.LoadingService", TaskType.TimeOut },
            
            {"Shared.Service.ParentControl.ParentControlService", TaskType.Hard},
            {"Shared.Service.Ump.SharedUmpService", TaskType.Hard},
            {"Shared.Service.Facebook.FacebookService", TaskType.Free},
            
            {"Shared.Service.Ads.SharedLevelPlay.LevelPlayAdService", TaskType.Free},
            {"Shared.Service.Ads.SharedLevelPlay.Mrec.LevelPlayMrecAdService", TaskType.Free},
            {"Shared.Service.AdQuality.AdQualityService", TaskType.Free},
            {"Shared.Service.Ads.Fake.FakeAdService", TaskType.Free},
            {"Shared.Service.Ads.Fake.FakeBannerAd", TaskType.Free},
            {"Shared.Service.Ads.Fake.FakeInterstitialAd", TaskType.Free},
            {"Shared.Service.Ads.Fake.FakeRewardAd", TaskType.Free},
            // IAP
            {"Shared.Service.Iap.FakeIapService", TaskType.Free},
            // In Play Ads.
            {"Shared.Service.Adverty.Adverty5Service", TaskType.Free},
            {"Shared.Service.Adverty.Adverty4Service", TaskType.Free},
            {"Shared.Service.Gadsme.GadsmeService", TaskType.Free},
            // Audio Ads
            {"Shared.Service.AudioMob.AudioMobService", TaskType.Free},
            {"Shared.Service.Odeeo.OdeeoService", TaskType.Free},
            {"Shared.Service.AudioAds.AudioAdsService", TaskType.TimeOut},
            
            {"Shared.Service.ErrorTracking.ErrorTrackingService", TaskType.TimeOut}
        };
    }
}
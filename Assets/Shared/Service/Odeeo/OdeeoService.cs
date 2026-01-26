#if ODEEO_AUDIO
using System.Collections.Generic;
using Odeeo;
using Odeeo.Data;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Entity.Config;
using Shared.Service.Ads;
using Shared.Service.Audio;
using Shared.Service.Audio.Internal;
using Shared.Service.Odeeo.Internal;
using Shared.Service.Tracking;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Odeeo
{
    /// <summary>
    // 2024/03/12 13:48:01.377 22448 22709 Info Unity AuAd OdeeoAdsManager-> OnInitializationFinished
    // 2024/03/12 13:48:01.576 22448 22709 Info Unity AuAd OdeeoAdsManager-> SubscribePlacement:397120091
    // 2024/03/12 13:48:18.864 22448 22709 Info Unity AuAd OdeeoAdsManager-> AdOnAvailabilityChanged:True --- entity: Odeeo.Entity.OdeeoAdData
    // 2024/03/12 13:48:43.761 22448 22709 Info Unity AuAd OdeeoAdsManager->Show
    // 2024/03/12 13:48:46.525 22448 22709 Info Unity AuAd OdeeoAdsManager -> AdOnImpression callback ->
    // 2024/03/12 13:48:46.557 22448 22709 Info Unity AuAd OdeeoAdsManager-> AdOnShow
    // 2024/03/12 13:48:46.638 22448 22709 Info Unity AuAd OdeeoAdsManager-> AdOnAvailabilityChanged:False --- entity: Odeeo.Entity.OdeeoAdData
    // 2024/03/12 13:49:16.628 22448 22709 Info Unity AuAd OdeeoAdsManager-> AdOnClose callback with reason:AdCompleted
    // 2024/03/12 13:49:33.028 22448 22709 Info Unity AuAd OdeeoAdsManager-> AdOnAvailabilityChanged:True --- entity: Odeeo.Entity.OdeeoAdData
    /// </summary>
    [Service]
    public class OdeeoService : IUnityOnApplicationPause, IOdeeoService, ISharedUtility, ISharedLogTag
    {
        [Inject] private IConfig _config;
        [Inject] private IAudioService _audioService;
        public string AppKey => _config.OdeeoAppKey;

        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;

        private readonly MuteRequest _muteRequest = new("OdeeoService");

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();

            OdeeoSdk.OnInitializationSuccess += _OnInitializationFinished;
            OdeeoSdk.OnInitializationFailed += _OnInitializationFailed;

            OdeeoEvents.OnPlacementRewardEvent += _OnPlacementReward;
            OdeeoEvents.OnPlacementAvailabilityChangedEvent += _OnPlacementAvailabilityChanged;
            OdeeoEvents.OnPlacementClickEvent += _OnPlacementClick;
            OdeeoEvents.OnPlacementCloseEvent += _OnPlacementClose;
            OdeeoEvents.OnPlacementShowEvent += _OnPlacementShow;
            OdeeoEvents.OnPlacementPauseEvent += _OnPlacementPause;
            OdeeoEvents.OnPlacementResumeEvent += _OnPlacementResume;
            OdeeoEvents.OnPlacementMuteEvent += _OnPlacementMute;
            OdeeoEvents.OnPlacementImpressionEvent += _OnPlacementImpression;
            OdeeoEvents.OnPlacementShowFailedEvent += _OnPlacementShowFailed;

            AdEvents.OnInterstitialShowStartedEvent += Pause;
            AdEvents.OnInterstitialShowCompletedEvent += Resume;

            AdEvents.OnRewardedAdShowStartedEvent += Pause;
            AdEvents.OnRewardedAdShowCompletedEvent += Resume;

            var logLevel = SharedSymbols.IsDevelopment ? OdeeoSdk.LogLevel.Debug : OdeeoSdk.LogLevel.None;
            OdeeoSdk.SetLogLevel(logLevel);
            OdeeoSdk.Initialize(AppKey);

            this.LogInfo(nameof(AppKey), AppKey);

#if UNITY_IOS && UNITY_EDITOR
            //Wrapped native IOS requestTrackingAuthorization function, to be able fetch advertiser id
            OdeeoSdk.RequestTrackingAuthorization();
#endif
            return _initOperation;
        }

        public void Pause()
        {
            this.LogInfo();
            OdeeoSdk.onApplicationPause(true);
        }

        public void Resume()
        {
            this.LogInfo();
            OdeeoSdk.onApplicationPause(false);
        }

        // ------------------------------------------------------------------------------------------------------------
        // Unity GameObject Lifecycle callbacks
        // ------------------------------------------------------------------------------------------------------------
        public void OnApplicationPause(bool pauseStatus)
        {
            this.LogInfo(nameof(pauseStatus), pauseStatus);
            OdeeoSdk.onApplicationPause(pauseStatus);
        }

        // ------------------------------------------------------------------------------------------------------------
        // SDK callbacks
        // ------------------------------------------------------------------------------------------------------------
        private void _OnInitializationFinished()
        {
            this.LogInfo("OdeeoSdk.IsInitialized()", OdeeoSdk.IsInitialized());
            _initOperation.Success();
            IsInitialized = true;
            OdeeoEvents.OnInitializationFinishedEvent.Invoke();
        }

        private void _OnInitializationFailed(int errorParam, string error)
        {
            this.LogInfo(nameof(errorParam), errorParam, nameof(error), error);
            _initOperation.Fail($"{errorParam} - {error}");
            IsInitialized = false;
            OdeeoEvents.OnInitializationFailedEvent.Invoke(errorParam, error);
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Placement callbacks
        /// ------------------------------------------------------------------------------------------------------------
        ///
        private void _OnPlacementAvailabilityChanged(string placement, bool flag)
        {
            this.LogInfo(nameof(placement), placement, nameof(flag), flag);
        }

        private void _OnPlacementShow(string placement)
        {
            this.LogInfo(nameof(placement), placement);
            this.Track(AdShowSucceededParams.Audio(placement));
            _audioService.RequestMute(_muteRequest);
        }

        private void _OnPlacementPause(string placement)
        {
            this.LogInfo(nameof(placement), placement);
        }

        private void _OnPlacementResume(string placement)
        {
            this.LogInfo(nameof(placement), placement);
        }

        private void _OnPlacementMute(string placement)
        {
            this.LogInfo(nameof(placement), placement);
        }

        private void _OnPlacementShowFailed(string placement, string errorCode, string des)
        {
            this.LogInfo(nameof(placement), placement, nameof(errorCode), errorCode, nameof(des), des);
            this.Track(AdShowFailedParams.Audio(placement, errorCode, des));
        }

        private void _OnPlacementReward(string placement, float value)
        {
            this.LogInfo(nameof(placement), placement, nameof(value), value);
        }

        private void _OnPlacementClick(string placement)
        {
            this.LogInfo(nameof(placement), placement);
            this.Track(AdClickedParams.Audio(placement));
        }

        private void _OnPlacementClose(string placement)
        {
            this.LogInfo(nameof(placement), placement);
            this.Track(AdClosedParams.Audio(placement));
            _audioService.RemoveMuteRequest(_muteRequest);
        }

        private void _OnPlacementImpression(string placement, OdeeoImpressionData data)
        {
            var adImpressionParams = new AdImpressionEvent(
                adPlatform: "odeeo",
                adSource: "odeeo",
                adUnitName: data.GetPlacementType().ToString(),
                adFormat: "audio",
                currency: "USD",
                value: data.GetPayableAmount());
            this.LogInfo(nameof(placement), placement, nameof(data), data, nameof(adImpressionParams),
                adImpressionParams);
            this.Track(adImpressionParams);
        }

        // -------------------------------------------------------------------------------------------------------------
        // Utilities
        // -------------------------------------------------------------------------------------------------------------
        private static readonly Dictionary<IconPosition, OdeeoSdk.IconPosition> PositionMap = new()
        {
            { IconPosition.TopLeft, OdeeoSdk.IconPosition.TopLeft },
            { IconPosition.TopCenter, OdeeoSdk.IconPosition.TopCenter },
            { IconPosition.TopRight, OdeeoSdk.IconPosition.TopRight },
            { IconPosition.CenterLeft, OdeeoSdk.IconPosition.CenterLeft },
            { IconPosition.Centered, OdeeoSdk.IconPosition.Centered },
            { IconPosition.CenterRight, OdeeoSdk.IconPosition.CenterRight },
            { IconPosition.BottomLeft, OdeeoSdk.IconPosition.BottomLeft },
            { IconPosition.BottomCenter, OdeeoSdk.IconPosition.BottomCenter },
            { IconPosition.BottomRight, OdeeoSdk.IconPosition.BottomRight }
        };

        public string LogTag => SharedLogTag.AudioAdsNOdeoo;
    }
}
#endif
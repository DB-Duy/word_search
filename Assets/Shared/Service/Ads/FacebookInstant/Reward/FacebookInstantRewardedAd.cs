#if FACEBOOK_INSTANT
using System;
using System.Runtime.InteropServices;
using Shared.Ads.Common;
using Shared.Ads.Common.RV;
using Shared.Ads.FacebookInstant.Callbacks;
using Shared.Ads.FacebookInstant.Config;
using Shared.Tracking.Models;
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Models.Templates;
using Shared.Utils;
using UnityEngine;

namespace Shared.Ads.FacebookInstant.Reward
{
    public class FacebookInstantRewardedAd : MonoBehaviour, IRewardAd
    {
        [DllImport("__Internal")]
        private static extern void LoadFacebookInstantRewardedAd(string rewardedID);

        [DllImport("__Internal")]
        private static extern string IsFacebookInstantRewardedAdReady();

        [DllImport("__Internal")]
        private static extern void ShowFacebookInstantRewardedAd();

        // ReSharper disable once InconsistentNaming
        private const string TAG = "FacebookInstantRewardedAd";

        private bool _isSetupDone = false;
        private bool _autoLoadEnable = false;

        public bool IsRewardedVideoAvailable => Convert.ToBoolean(IsFacebookInstantRewardedAdReady());
        private bool IsInternetReachability => SharedCore.Instance.InternetReachabilityController.IsAvailable;
        
        public IRewardedAdShowFailMessageResolver FailMessageResolver { get; set; }
        private IRewardedAdAsyncOperation ShowOperation { get; set; }

        private IAdController _adController;
        private IAdPlacement _placement;

        private readonly FlagLock _loadAdLock = new(name: $"{TAG}._loadAdLock", isLock: false);
        private readonly FlagLock _showAdLock = new(name: $"{TAG}._showAdLock", isLock: false);

        /// ------------------------------------------------------------------------------------------------------------
        /// Setup Functions
        /// ------------------------------------------------------------------------------------------------------------
        public void Setup(IAdController adController)
        {
            if (!_isSetupDone)
            {
                _isSetupDone = true;
                _adController = adController;
                _AddListeners();
            }
            else Debug.LogErrorFormat("{0}->Setup: _isSetupDone=true", TAG);
        }

        private void _AddListeners()
        {
            // Load
            FacebookInstantEvents.Reward.onAdReadyEvent += _OnAdReadyEvent;
            FacebookInstantEvents.Reward.onAdLoadFailedEvent += _OnAdLoadFailedEvent;
            // Show
            FacebookInstantEvents.Reward.onAdOpenedEvent += _OnAdOpenedEvent;
            FacebookInstantEvents.Reward.onAdShowFailedEvent += _OnAdShowFailedEvent;
            // Close
            FacebookInstantEvents.Reward.onAdClosedEvent += _OnAdClosedEvent;
            // Reward
            FacebookInstantEvents.Reward.onAdRewardedEvent += _OnAdRewardedEvent;
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Load Functions
        /// ------------------------------------------------------------------------------------------------------------
        public void StartLoadLoop()
        {
            _autoLoadEnable = true;
            _LoadInternal();
        }

        void Update()
        {
            if (_autoLoadEnable && _loadAdLock.IsNotLock && Time.frameCount % 90 == 0) _LoadInternal();
        }

        private void _LoadInternal()
        {
            var error = string.Empty;
            if (Application.isEditor) error = "Application.isEditor";
            else if (_adController == null) error = "_adManager == null";
            else if (_adController.IsNotInitialized) error = "_adManager.IsNOTInitialized";
            else if (_loadAdLock.IsLock) error = "_loadAdLock.IsLock";
            else if (_showAdLock.IsLock) error = "_showAdLock.IsLock";
            else if(!IsInternetReachability) error = "No Network connection";
            else if (IsRewardedVideoAvailable) error = "IsRewardedVideoAvailable";
            if (!string.IsNullOrEmpty(error))
            {
                SharedLogger.Log($"{TAG}->_LoadInternal: IGNORE: {error}");
                return;
            }

            _loadAdLock.Lock();
            var adConfig = (IFacebookInstantAdConfig)_adController.AdConfig;
            LoadFacebookInstantRewardedAd(adConfig.FacebookRewardedId);
            SharedCore.Instance.TrackingService.Track(AdLoadParams.RewardedAdLoadParams());
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Show Functions
        /// ------------------------------------------------------------------------------------------------------------
        public IRewardedAdAsyncOperation Show(IAdPlacement placement)
        {
            SharedLogger.Log($"{TAG}->Show: {placement.Name}");
            var errorMessage = string.Empty;
            if (Application.isEditor) errorMessage = "Application.isEditor";
            else if (_showAdLock.IsLock) errorMessage = "_showAdLock.IsLock";
            else if (_adController == null) errorMessage = "_adController == null";
            else if (_adController.IsNotInitialized) errorMessage = "_adController.IsNotInitialized";
            else if(!IsInternetReachability) errorMessage = "No Network connection";
            else if (!IsRewardedVideoAvailable) errorMessage = "!IsRewardedVideoAvailable";
            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.LogError($"{TAG}->Show: Error: {placement.Name} {errorMessage}");
                var userMessage = FailMessageResolver
                    .Resolve(IsInternetReachability ? RewardedAdShowFailReason.NoInternetConnection : RewardedAdShowFailReason.NotAvailable);

                return new RewardedAdAsyncOperation().Fail(errorMessage, userMessage);
            }

            _placement = placement;
            ShowOperation = new RewardedAdAsyncOperation();
            _showAdLock.Lock();
            _TrackAvailable(placement);
            ShowFacebookInstantRewardedAd();
            return ShowOperation;
        }

        public IRewardAd SetShowFailMessageResolver(IRewardedAdShowFailMessageResolver resolver)
        {
            FailMessageResolver = resolver;
            return this;
        }

        private void _TrackAvailable(IAdPlacement placement)
        {
            ITrackingEvent e = IsRewardedVideoAvailable
                ? AdReadyParams.Rewarded(placement.Name)
                : AdNotReadyParams.Rewarded(placement.Name);
            SharedCore.Instance.TrackingService.Track(e);
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Callbacks
        /// ------------------------------------------------------------------------------------------------------------
        private void _OnAdReadyEvent()
        {
            SharedLogger.Log($"{TAG}->_OnAdReadyEvent");
            _loadAdLock.Unlock();
            SharedCore.Instance.TrackingService.Track(AdLoadSucceededParams.Rewarded());
        }

        private void _OnAdLoadFailedEvent(string message)
        {
            SharedLogger.Log($"{TAG}->_OnAdLoadFailedEvent {message}");
            _loadAdLock.Unlock();
            SharedCore.Instance.TrackingService.Track(AdLoadFailedParams.Rewarded(string.Empty, message));
        }

        private void _OnAdOpenedEvent()
        {
            SharedLogger.Log($"{TAG}->_OnAdOpenedEvent");
            SharedCore.Instance.TrackingService.Track(AdShowSucceededParams.Rewarded(_placement.Name));
            SharedCoreEvents.RewardAd.InvokeRewardedAdShowedHandlers();
        }

        private void _OnAdShowFailedEvent(string message)
        {
            SharedLogger.Log($"{TAG}->_OnAdShowFailedEvent: {message}");
            _showAdLock.Unlock();
            ShowOperation.Fail($"_OnAdShowFailedEvent - {message}");
            SharedCore.Instance.TrackingService.Track(AdShowFailedParams.Rewarded(_placement.Name, string.Empty, message));
        }

        private void _OnAdClosedEvent()
        {
            SharedLogger.Log($"{TAG}->_OnAdClosedEvent");
            _showAdLock.Unlock();
            ShowOperation.Complete();
            SharedCore.Instance.TrackingService.Track(AdClosedParams.Rewarded(_placement.Name));
            SharedCoreEvents.RewardAd.InvokeRewardedAdClosedHandlers();
        }

        private void _OnAdRewardedEvent()
        {
            SharedLogger.Log($"{TAG}->_OnAdRewardedEvent");
            ShowOperation.Success();
            SharedCore.Instance.TrackingService.Track(AdRewardedParams.Rewarded(_placement.Name));
        }
    }
}
#endif
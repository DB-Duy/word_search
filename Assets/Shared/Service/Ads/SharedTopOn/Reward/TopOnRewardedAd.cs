#if TOPON
using System.Collections.Generic;
using System.Diagnostics;
using AnyThinkAds.Api;
using Newtonsoft.Json;
using Shared.Ads.Common;
using Shared.Ads.Common.RV;
using Shared.Tracking.Models;
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Models.Templates;
using Shared.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Shared.Ads.SharedTopOn.Reward
{
    [DisallowMultipleComponent]
    public class TopOnRewardedAd : MonoBehaviour, IRewardAd
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "TopOnRewardedAd";

        private IAdController _adController;
        private ITopOnAdConfig _adConfig;
        
        private bool _isSetupDone = false;
        private bool _autoLoadEnable = false;
        
        private IAdPlacement _placement;
        private IRewardedAdAsyncOperation ShowOperation { get; set; }
        public IRewardedAdShowFailMessageResolver FailMessageResolver { get; set; }
        
        private readonly FlagLock _loadAdLock = new(name: $"{TAG}._loadAdLock", isLock: false);
        private readonly FlagLock _showAdLock = new(name: $"{TAG}._showAdLock", isLock: false);

        public bool IsRewardedVideoAvailable => ATRewardedVideo.Instance.hasAdReady(_adConfig.TopOnRewarded);
        private bool InternetReachability => SharedCore.Instance.InternetReachabilityController.IsAvailable;
        /// ------------------------------------------------------------------------------------------------------------
        /// Setup Functions
        /// ------------------------------------------------------------------------------------------------------------
        public void Setup(IAdController adController)
        {
            if (!_isSetupDone)
            {
                _isSetupDone = true;
                _adController = adController;
                _adConfig = (ITopOnAdConfig)adController.AdConfig;
                _AddListeners();
            }
            else Debug.LogErrorFormat("{0}->Setup: _isSetupDone=true", TAG);
        }

        private void _AddListeners()
        {
            // Load
            ATRewardedVideo.Instance.client.onAdLoadEvent += _onAdLoadEvent;
            ATRewardedVideo.Instance.client.onAdLoadFailureEvent += _onAdLoadFailureEvent;
            
            // some platforms have these callbacks
            ATRewardedVideo.Instance.client.onAdVideoStartEvent += _onAdVideoStartEvent;
            ATRewardedVideo.Instance.client.onAdVideoFailureEvent += _onAdVideoFailureEvent;
            ATRewardedVideo.Instance.client.onAdVideoEndEvent += _onAdVideoEndEvent;
            
            // others
            ATRewardedVideo.Instance.client.onAdClickEvent += _onAdClickEvent;
            
            // reward
            ATRewardedVideo.Instance.client.onRewardEvent += _onRewardEvent;
            
            //close
            ATRewardedVideo.Instance.client.onAdVideoCloseEvent += _onAdVideoCloseEvent;
            
            // The ad source events
            ATRewardedVideo.Instance.client.onAdSourceAttemptEvent += _onAdSourceAttemptEvent;
            ATRewardedVideo.Instance.client.onAdSourceFilledEvent += _onAdSourceFilledEvent;
            ATRewardedVideo.Instance.client.onAdSourceLoadFailureEvent += _onAdSourceLoadFailureEvent;
            ATRewardedVideo.Instance.client.onAdSourceBiddingAttemptEvent += _onAdSourceBiddingAttemptEvent;
            ATRewardedVideo.Instance.client.onAdSourceBiddingFilledEvent += _onAdSourceBiddingFilledEvent;
            ATRewardedVideo.Instance.client.onAdSourceBiddingFailureEvent += _onAdSourceBiddingFailureEvent;
            
            // play again.
            ATRewardedVideo.Instance.client.onPlayAgainStart += _onPlayAgainStart;
            ATRewardedVideo.Instance.client.onPlayAgainFailure += _onPlayAgainFailure;
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Load Functions
        /// ------------------------------------------------------------------------------------------------------------
        public void StartLoadLoop()
        {
            _autoLoadEnable = true;
            _LoadInternal();
        }

        private void Update()
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
            else if(!InternetReachability) error = "!InternetReachability";
            else if (IsRewardedVideoAvailable) error = "IsRewardedVideoAvailable";
            if (!string.IsNullOrEmpty(error))
            {
                SharedLogger.Log($"{TAG}->_LoadInternal: IGNORE: {error}");
                return;
            }

            _loadAdLock.Lock();
            
            //If you need to distribute rewards through the developer's server (some advertising platforms support this server incentive), you need to pass the following two keys
            //ATConst.USERID_KEY must be transmitted, used to identify each user; ATConst.USER_EXTRA_DATA is an optional parameter, after being passed in, it will be transparently transmitted to the developer's server
            var config = new Dictionary<string, string>
            {
                { ATConst.USERID_KEY, SystemInfo.deviceUniqueIdentifier },
                { ATConst.USER_EXTRA_DATA, "" }
            };

            ATRewardedVideo.Instance.loadVideoAd(_adConfig.TopOnRewarded, config);
            _loadAdLock.Lock();
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
            else if (ShowOperation != null && !ShowOperation.IsComplete) errorMessage = $"ShowOperation != null && !ShowOperation.IsComplete && placement({placement}) != _placement({_placement})"; 
            else if (_showAdLock.IsLock) errorMessage = "_showAdLock.IsLock";
            else if (_adController == null) errorMessage = "_adController == null";
            else if (_adController.IsNotInitialized) errorMessage = "_adController.IsNotInitialized";
            else if(!InternetReachability) errorMessage = "!InternetReachability";
            else if (!IsRewardedVideoAvailable) errorMessage = "!IsRewardedVideoAvailable";
            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.LogError($"{TAG}->Show: Error: {placement.Name} {errorMessage}");
                var userMessage = FailMessageResolver.Resolve(InternetReachability ? RewardedAdShowFailReason.NoInternetConnection : RewardedAdShowFailReason.NotAvailable);
                return new RewardedAdAsyncOperation().Fail(errorMessage, userMessage);
            }

            _placement = placement;
            ShowOperation = new RewardedAdAsyncOperation();
            _showAdLock.Lock();
            _TrackAvailable(placement);
            ATRewardedVideo.Instance.showAd(_adConfig.TopOnRewarded);
            return ShowOperation;
        }

        public IRewardAd SetShowFailMessageResolver(IRewardedAdShowFailMessageResolver resolver)
        {
            FailMessageResolver = resolver;
            return this;
        }

        private void _TrackAvailable(IAdPlacement placement)
        {
            ITrackingEvent e = IsRewardedVideoAvailable? AdReadyParams.Rewarded(placement.Name) : AdNotReadyParams.Rewarded(placement.Name);
            SharedCore.Instance.TrackingService.Track(e);
        }
        // -------------------------------------------------------------------------------------------------------------
        // Load callbacks
        // -------------------------------------------------------------------------------------------------------------
        private void _onAdLoadEvent(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdLoadEvent", erg);
            _loadAdLock.Unlock();
            SharedCore.Instance.TrackingService.Track(AdLoadSucceededParams.Rewarded());
        }
        
        private void _onAdLoadFailureEvent(object sender, ATAdErrorEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdLoadFailureEvent", erg);
            _loadAdLock.Unlock();
            SharedCore.Instance.TrackingService.Track(AdLoadFailedParams.Rewarded(erg.errorCode, erg.errorMessage));
        }
        /// ------------------------------------------------------------------------------------------------------------
        /// Some platforms have these callbacks
        /// ------------------------------------------------------------------------------------------------------------
        private void _onAdVideoStartEvent(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdVideoStartEvent", erg);
            SharedCore.Instance.TrackingService.Track(AdShowSucceededParams.Rewarded(_placement.Name));
            SharedCoreEvents.RewardAd.InvokeRewardedAdShowedHandlers();
        }
        
        private void _onAdVideoFailureEvent(object sender, ATAdErrorEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdVideoFailureEvent", erg);
            _showAdLock.Unlock();
            ShowOperation.Fail($"_onAdVideoFailureEvent: {erg.errorCode}, {erg.errorMessage}");
            SharedCore.Instance.TrackingService.Track(AdShowFailedParams.Rewarded(_placement.Name, erg.errorCode, erg.errorMessage));
            SharedCoreEvents.RewardAd.InvokeRewardedAdShowFailedHandlers();
        }
        
        private void _onAdVideoEndEvent(object sender, ATAdEventArgs erg) => _DebugATAdEventArgs("_onAdVideoEndEvent", erg);
        /// ------------------------------------------------------------------------------------------------------------
        /// Others
        /// ------------------------------------------------------------------------------------------------------------
        private void _onAdClickEvent(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdClickEvent", erg);
            SharedCore.Instance.TrackingService.Track(AdClickedParams.Rewarded(_placement.Name));
        }
        /// ------------------------------------------------------------------------------------------------------------
        /// Reward
        /// ------------------------------------------------------------------------------------------------------------
        private void _onRewardEvent(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onRewardEvent", erg);
            ShowOperation.Success();
            SharedCore.Instance.TrackingService.Track(AdRewardedParams.Rewarded(_placement.Name));
        }
        /// ------------------------------------------------------------------------------------------------------------
        /// Close
        /// ------------------------------------------------------------------------------------------------------------
        // The ad is closed, where isReward only means that the onReward() method is called back when onRewardedVideoAdPlayClosed is called back.
        private void _onAdVideoCloseEvent(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdVideoCloseEvent", erg);
            _showAdLock.Unlock();
            ShowOperation.Complete();
            SharedCore.Instance.TrackingService.Track(AdClosedParams.Rewarded(_placement.Name));
            SharedCoreEvents.RewardAd.InvokeRewardedAdClosedHandlers();
        }
        /// ------------------------------------------------------------------------------------------------------------
        /// The ad source events
        /// ------------------------------------------------------------------------------------------------------------
        private void _onAdSourceAttemptEvent(object sender, ATAdEventArgs erg) => _DebugATAdEventArgs("_onAdSourceAttemptEvent", erg);
        private void _onAdSourceFilledEvent(object sender, ATAdEventArgs erg) => _DebugATAdEventArgs("_onAdSourceFilledEvent", erg);
        private void _onAdSourceLoadFailureEvent(object sender, ATAdErrorEventArgs erg) => _DebugATAdEventArgs("_onAdSourceLoadFailureEvent", erg);
        private void _onAdSourceBiddingAttemptEvent(object sender, ATAdEventArgs erg) => _DebugATAdEventArgs("_onAdSourceBiddingAttemptEvent", erg);
        private void _onAdSourceBiddingFilledEvent(object sender, ATAdEventArgs erg) => _DebugATAdEventArgs("_onAdSourceBiddingFilledEvent", erg);
        private void _onAdSourceBiddingFailureEvent(object sender, ATAdErrorEventArgs erg) => _DebugATAdEventArgs("_onAdSourceBiddingFailureEvent", erg);
        /// ------------------------------------------------------------------------------------------------------------
        /// Play Again
        /// ------------------------------------------------------------------------------------------------------------
        private void _onPlayAgainStart(object sender, ATAdEventArgs erg) => _DebugATAdEventArgs("_onPlayAgainStart", erg);
        private void _onPlayAgainFailure(object sender, ATAdErrorEventArgs erg) => _DebugATAdEventArgs("_onPlayAgainFailure", erg);
        // -------------------------------------------------------------------------------------------------------------
        // Utility functions
        // -------------------------------------------------------------------------------------------------------------
        [Conditional("LOG_INFO")]
        private void _DebugATAdEventArgs(string fromFunction, ATAdEventArgs erg)
        {
            var logDict = new Dictionary<string, object>()
            {
                {"placementId", erg.placementId},
                {"callbackInfo", erg.callbackInfo.toDictionary()},
                {"isTimeout", erg.isTimeout},
                {"isDeeplinkSucceed", erg.isDeeplinkSucceed}
            };
            if (erg is ATAdErrorEventArgs err)
            {
                logDict.Add("errorMessage", err.errorMessage);
                logDict.Add("errorCode", err.errorCode);
            }

            SharedLogger.Log($"{0}->{fromFunction}: {JsonConvert.SerializeObject(logDict)}");
        }
    }
}
#endif
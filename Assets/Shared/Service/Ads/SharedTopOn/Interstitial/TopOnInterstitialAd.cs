#if TOPON
using System;
using System.Collections;
using System.Collections.Generic;
using AnyThinkAds.Api;
using Shared.Ads.Common;
using Shared.Ads.Validator;
using Shared.Ads.Validator.Interstitial;
using Shared.Common;
using Shared.Tracking.Models;
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Models.Templates;
using Shared.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Shared.Ads.SharedTopOn.Interstitial
{
    [DisallowMultipleComponent]
    public class TopOnInterstitialAd : MonoBehaviour, ITopOnInterstitialAd
    {
        private const string Tag = "TopOnInterstitialAd";
        
        private IAdController _adController;
        private ITopOnAdConfig _adConfig;
        
        private bool _isSetupDone = false;
        private bool _autoLoadEnable = false;

        private readonly FlagLock _loadAdLock = new(name: Tag, isLock: false);

        private IAdPlacement _adPlacement;
        private IAsyncOperation _showOperation;

        public bool IsInterstitialReady => ATInterstitialAd.Instance.hasInterstitialAdReady(_adConfig.TopOnInterstitial);
        private bool InternetReachability => SharedCore.Instance.InternetReachabilityController.IsAvailable;

        public DateTime CloseDateTime { get; private set; }

        private readonly HashSet<IAdValidator> _validators = new();
        private readonly HashSet<IAdCloseEventValidator> _closeEventValidators = new();
        
        
        public void Setup(IAdController adController)
        {
            if (!_isSetupDone)
            {
                _isSetupDone = true;
                _adController = adController;
                _adConfig = (ITopOnAdConfig)adController.AdConfig;
                _AddListeners();
            }
            else Debug.LogErrorFormat("{0}->Setup: ERROR: Already setup.", Tag);
        }
        
        private void _AddListeners()
        {
            // load
            ATInterstitialAd.Instance.client.onAdLoadEvent += _onAdLoadEvent;
            ATInterstitialAd.Instance.client.onAdLoadFailureEvent += _onAdLoadFailureEvent;
            
            // Show
            ATInterstitialAd.Instance.client.onAdShowEvent += _onAdShowEvent;
            ATInterstitialAd.Instance.client.onAdShowFailureEvent += _onAdShowFailureEvent;
            
            // Close
            ATInterstitialAd.Instance.client.onAdCloseEvent += _onAdCloseEvent;
            
            // More 
            ATInterstitialAd.Instance.client.onAdClickEvent += _onAdClickEvent;
            ATInterstitialAd.Instance.client.onAdVideoStartEvent += _onAdVideoStartEvent;
            ATInterstitialAd.Instance.client.onAdVideoEndEvent += _onAdVideoEndEvent;
            ATInterstitialAd.Instance.client.onAdVideoFailureEvent += _onAdVideoFailureEvent;
            
            // The ad source events
            ATInterstitialAd.Instance.client.onAdSourceAttemptEvent += _onAdSourceBiddingFilledEvent;
            ATInterstitialAd.Instance.client.onAdSourceFilledEvent += _onAdSourceFilledEvent;
            ATInterstitialAd.Instance.client.onAdSourceLoadFailureEvent += _onAdSourceLoadFailureEvent;
            ATInterstitialAd.Instance.client.onAdSourceBiddingAttemptEvent += _onAdSourceBiddingFilledEvent;
            ATInterstitialAd.Instance.client.onAdSourceBiddingFilledEvent += _onAdSourceBiddingFilledEvent;
            ATInterstitialAd.Instance.client.onAdSourceBiddingFailureEvent += _onAdSourceBiddingFailureEvent;
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Validators
        /// ------------------------------------------------------------------------------------------------------------
        public IInterstitialAd AddValidators(params IAdValidator[] validators)
        {
            _validators.AddRange(validators);
            foreach (var validator in _validators)
            {
                if (validator is IAdCloseEventValidator ee) _closeEventValidators.Add(ee);
            }

            return this;
        }
        
        private bool _ValidateShow(IAdPlacement placement)
        {
            SharedLogger.Log($"{Tag}->_ValidateShow: {placement.Name}");
            var p = new InterstitialValidationParams(placement);
            foreach (var validator in _validators)
            {
                var r = validator.Validate(p);
                if (!r.IsSuccess)
                {
                    SharedLogger.Log($"{Tag}->_ValidateShow: FAILED: {r.ValidatorName} - {r.FailReason}");
                    return false;
                }

                if (!string.IsNullOrEmpty(r.FailReason))
                {
                    SharedLogger.Log($"{Tag}->_ValidateShow: IGNORE: {r.ValidatorName} - {r.FailReason}");
                }
            }

            return true;
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Load Functions
        /// ------------------------------------------------------------------------------------------------------------
        public void StartLoadLoop()
        {
            SharedLogger.Log($"{Tag}->StartLoadLoop");
            _autoLoadEnable = true;
            _LoadInternal();
        }
        
        void Update()
        {
            if (_autoLoadEnable && Time.frameCount % 180 == 0) _LoadInternal();
        }

        private void _LoadInternal()
        {
            var error = string.Empty;
            if (Application.isEditor) error = "Application.isEditor";
            else if (_adController == null) error = "_adController == null";
            else if (_adController.IsNotInitialized) error = "_adController.IsNotInitialized";
            else if (_loadAdLock.IsLock) error = "_loadAdLock.IsLock";
            else if (_showOperation != null && !_showOperation.IsComplete) error = "_showOperation != null && !_showOperation.IsComplete";
            else if(!InternetReachability) error = "!InternetReachability";
            else if (IsInterstitialReady) error = "IsInterstitialReady";
            if (!string.IsNullOrEmpty(error))
            {
                SharedLogger.Log($"{Tag}->_LoadInternal: IGNORE: {error}");
                return;
            }
            _loadAdLock.Lock();
            var config = new Dictionary<string, object> {
                { ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL, ATConst.USE_REWARDED_VIDEO_AS_INTERSTITIAL_NO } 
            };
            ATInterstitialAd.Instance.loadInterstitialAd(_adConfig.TopOnInterstitial, config);
            SharedCore.Instance.TrackingService.Track(AdLoadParams.InterstitialAdLoadParams());
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Show Functions
        /// ------------------------------------------------------------------------------------------------------------
        public IAsyncOperation Show(IAdPlacement adPlacement)
        {
            SharedLogger.Log($"{Tag}->Show: {adPlacement.Name}");
            var errorMessage = string.Empty;
            if (Application.isEditor) errorMessage = "APPLICATION_IS_EDITOR";
            else if (_showOperation != null && !_showOperation.IsComplete) errorMessage = "ShowOperation != null && !ShowOperation.IsComplete";
            else if (_adController == null) errorMessage = "_adController == null";
            else if (_adController.IsNotInitialized) errorMessage = "_adController.IsNotInitialized";
            else if (!IsInterstitialReady) errorMessage = "!IsInterstitialReady";
            else if (!_ValidateShow(adPlacement)) errorMessage = $"!_ValidateShow({adPlacement.Name})";
            
            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.Log($"{Tag}->Show: ERROR: {errorMessage}");
                return new SharedAsyncOperation().Fail(errorMessage);
            }

            _showOperation = new SharedAsyncOperation();
            StartCoroutine(_Show(adPlacement));
            return _showOperation;
        }

        private IEnumerator _Show(IAdPlacement adPlacement)
        {
            SharedLogger.Log($"{Tag}->_Show: {adPlacement.Name}");
            _adPlacement = adPlacement;
            _TrackInterstitialReady(adPlacement);
            
            SharedCoreEvents.InterstitialAd.OnInterstitialAdFakeLoadingStartedEvent.Invoke();
            yield return new WaitForSeconds(1.5f);
            ATInterstitialAd.Instance.showInterstitialAd(_adConfig.TopOnInterstitial);
            yield return new WaitForSeconds(0.3f);
            SharedCoreEvents.InterstitialAd.OnInterstitialAdFakeLoadingCompletedEvent.Invoke();
        }

        private void _TrackInterstitialReady(IAdPlacement adPlacement)
        {
            ITrackingEvent ob = IsInterstitialReady ? AdReadyParams.Interstitial(adPlacement.Name) : AdNotReadyParams.Interstitial(adPlacement.Name);
            SharedCore.Instance.TrackingService.Track(ob);
        }
        
        /// ------------------------------------------------------------------------------------------------------------
        /// Callbacks
        /// ------------------------------------------------------------------------------------------------------------
        // Load
        private void _onAdLoadEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdLoadEvent", erg);
            _loadAdLock.Unlock();
            SharedCore.Instance.TrackingService.Track(AdLoadSucceededParams.Interstitial());
        }
        
        private void _onAdLoadFailureEvent(object sender, ATAdErrorEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdLoadFailureEvent", erg);
            _loadAdLock.Unlock();
            SharedCore.Instance.TrackingService.Track(AdLoadFailedParams.Interstitial(erg.errorCode, erg.errorMessage));
        }
        
        // Show
        private void _onAdShowEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdShowEvent", erg);
            SharedCore.Instance.TrackingService.Track(AdShowSucceededParams.Interstitial(_adPlacement.Name));
            SharedCoreEvents.InterstitialAd.OnInterstitialAdShowedEvent.Invoke();
        }
        
        private void _onAdShowFailureEvent(object sender, ATAdErrorEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdShowFailureEvent", erg);
            _showOperation.Fail("OnAdShowFailedEvent");
            _showOperation = null;
            SharedCore.Instance.TrackingService.Track(AdShowFailedParams.Interstitial(_adPlacement.Name, erg.errorCode, erg.errorMessage));
        }
        
        // Close
        private void _onAdCloseEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdCloseEvent", erg);
            foreach (var validator in _closeEventValidators) validator.OnAdCloseEvent();
            CloseDateTime = DateTime.Now;
            _showOperation.Success();
            _showOperation = null;
            SharedCore.Instance.TrackingService.Track(AdClosedParams.Interstitial(_adPlacement.Name));
        }
        
        // More others
        private void _onAdClickEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdClickEvent", erg);
            SharedCore.Instance.TrackingService.Track(AdClickedParams.Interstitial(_adPlacement.Name));
        }
        
        private void _onAdVideoStartEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdVideoStartEvent", erg);
        }
        
        private void _onAdVideoEndEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdVideoStartEvent", erg);
        }
        
        private void _onAdVideoFailureEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdVideoFailureEvent", erg);
        }
        
        // AdSource
        private void _onAdSourceBiddingFilledEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdSourceBiddingFilledEvent", erg);
        }
        
        private void _onAdSourceFilledEvent(object sender, ATAdEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdSourceFilledEvent", erg);
        }
        
        private void _onAdSourceBiddingFailureEvent(object sender, ATAdErrorEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdSourceBiddingFailureEvent", erg);
        }
        
        private void _onAdSourceLoadFailureEvent(object sender, ATAdErrorEventArgs erg)
        {
            TopOnUtils.DebugATAdEventArgs($"{Tag}->_onAdSourceLoadFailureEvent", erg);
        }
    }
}
#endif
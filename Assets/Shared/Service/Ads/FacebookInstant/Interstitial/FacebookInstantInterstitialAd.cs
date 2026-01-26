#if FACEBOOK_INSTANT
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RemoteConfig;
using Shared.Ads.Common;
using Shared.Ads.FacebookInstant.Callbacks;
using Shared.Ads.FacebookInstant.Config;
using Shared.Ads.FacebookInstant.Interstitial.Validation;
using Shared.Ads.Validator;
using Shared.Common;
using Shared.PlayerPrefsRepository.RemoteConfig;
using Shared.Tracking.Models;
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Models.Templates;
using Shared.Utils;
using UnityEngine;

namespace Shared.Ads.FacebookInstant.Interstitial
{
    /// <summary>
    /// https://developers.facebook.com/docs/games/monetize/in-app-ads/instant-games/
    /// </summary>
    public class FacebookInstantInterstitialAd : MonoBehaviour, IInterstitialAd, IFacebookInstantInterstitialAd
    {
        [DllImport("__Internal")]
        private static extern void LoadFacebookInstantInterstitialAd(string interstitialID);
        [DllImport("__Internal")]
        private static extern string IsFacebookInstantInterstitialReady();
        [DllImport("__Internal")]
        private static extern void ShowFacebookInstantInterstitialAd();

        private const string Tag = "FacebookInstantInterstitialAd";
        private IAdController _adController;
        private bool _isSetupDone = false;
        private bool _autoLoadEnable = false;

        private readonly FlagLock _loadAdLock = new(name: Tag, isLock: false);
        private readonly FlagLock _showAdLock = new(name: Tag, isLock: false);

        private IAdPlacement _adPlacement;

        private IAsyncOperation ShowOperation { get; set; }

        public bool IsInterstitialReady => IsFacebookInstantInterstitialReady() == "true";
        private bool IsInternetReachability => SharedCore.Instance.InternetReachabilityController.IsAvailable;

        public DateTime CloseDateTime { get; private set; }

        public IRemoteConfigRepository<InterstitialConfig> ConfigRepository { get; set; }

        private readonly HashSet<IAdValidator> _validators = new();
        private readonly HashSet<IAdCloseEventValidator> _closeEventValidators = new();
        
        public void Setup(IAdController adController)
        {
            if (!_isSetupDone)
            {
                _isSetupDone = true;
                _adController = adController;
                _AddListeners();
            }
            else Debug.LogErrorFormat("{0}->Setup: ERROR: Already setup.", Tag);
        }
        
        private void _AddListeners()
        {
            // Load
            FacebookInstantEvents.Interstitial.onAdReadyEvent += _OnAdReadyEvent;
            FacebookInstantEvents.Interstitial.onAdLoadFailedEvent += _OnAdLoadFailedEvent;
            // Show
            FacebookInstantEvents.Interstitial.onAdShowSucceededEvent += _OnAdShowSucceededEvent;
            FacebookInstantEvents.Interstitial.onAdShowFailedEvent += _OnAdShowFailedEvent;
            // Close
            FacebookInstantEvents.Interstitial.onAdClosedEvent += _OnAdClosedEvent;
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
            var config = ConfigRepository.Get();
            var p = new FacebookInstantInterstitialValidationParams(placement, config);
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
            if (_autoLoadEnable && Time.frameCount % 90 == 0) _LoadInternal();
        }

        private void _LoadInternal()
        {
            var error = string.Empty;
            if (Application.isEditor) error = "Application.isEditor";
            else if (_adController == null) error = "_adController == null";
            else if (_adController.IsNotInitialized) error = "_adController.IsNotInitialized";
            else if (_loadAdLock.IsLock) error = "_loadAdLock.IsLock";
            else if (_showAdLock.IsLock) error = "_showAdLock.IsLock";
            else if(!IsInternetReachability) error = "No Network connection";
            else if (IsInterstitialReady) error = "IsInterstitialReady";
            if (!string.IsNullOrEmpty(error))
            {
                SharedLogger.Log($"{Tag}->_LoadInternal: IGNORE: {error}");
                return;
            }
            _loadAdLock.Lock();
            if (_adController.AdConfig is IFacebookInstantAdConfig facebookInstantAdConfig)
            {
                LoadFacebookInstantInterstitialAd(facebookInstantAdConfig.FacebookInterstitialId);
                SharedCore.Instance.TrackingService.Track(AdLoadParams.InterstitialAdLoadParams());
            }
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Show Functions
        /// ------------------------------------------------------------------------------------------------------------
        public IAsyncOperation Show(IAdPlacement adPlacement)
        {
            SharedLogger.Log($"{Tag}->Show: {adPlacement.Name}");
            var errorMessage = string.Empty;
            if (Application.isEditor) errorMessage = "APPLICATION_IS_EDITOR";
            else if (ConfigRepository == null) errorMessage = "ConfigRepository == null";
            else if (_showAdLock.IsLock) errorMessage = "_showAdLock.IsLock";
            else if (_adController == null) errorMessage = "_adController == null";
            else if (_adController.IsNotInitialized) errorMessage = "_adController.IsNotInitialized";
            else if (!IsInterstitialReady) errorMessage = "!IsInterstitialReady";
            else if (!_ValidateShow(adPlacement)) errorMessage = $"!_ValidateShow({adPlacement.Name})";
            
            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.Log($"{Tag}->Show: ERROR: {errorMessage}");
                return new SharedAsyncOperation().Fail(errorMessage);
            }

            ShowOperation = new SharedAsyncOperation();
            StartCoroutine(_Show(adPlacement));
            return ShowOperation;
        }

        private IEnumerator _Show(IAdPlacement adPlacement)
        {
            SharedLogger.Log($"{Tag}->_Show: {adPlacement.Name}");
            _adPlacement = adPlacement;
            _TrackInterstitialReady(adPlacement);
            
            _showAdLock.Lock();
            SharedCoreEvents.InterstitialAd.OnInterstitialAdFakeLoadingStartedEvent.Invoke();
            yield return new WaitForSeconds(1.5f);
            ShowFacebookInstantInterstitialAd();
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
        private void _OnAdReadyEvent()
        {
            SharedLogger.Log($"{Tag}->_OnAdReadyEvent");
            _loadAdLock.Unlock();
            SharedCore.Instance.TrackingService.Track(AdLoadSucceededParams.Interstitial());
        }

        private void _OnAdLoadFailedEvent(string message)
        {
            SharedLogger.Log($"{Tag}->_OnAdLoadFailedEvent {message}");
            _loadAdLock.Unlock();
            SharedCore.Instance.TrackingService.Track(AdLoadFailedParams.Interstitial(string.Empty, message));
        }
         
        private void _OnAdShowSucceededEvent()
        {
            SharedLogger.Log($"{Tag}->_OnAdShowSucceededEvent");
            SharedCore.Instance.TrackingService.Track(AdShowSucceededParams.Interstitial(_adPlacement.Name));
            SharedCoreEvents.InterstitialAd.OnInterstitialAdShowedEvent.Invoke();
        }
 
        private void _OnAdShowFailedEvent(string message)
        {
            SharedLogger.Log($"{Tag}->_OnAdShowFailedEvent: {message}");
            _showAdLock.Unlock();
            ShowOperation.Fail("OnAdShowFailedEvent");
            SharedCore.Instance.TrackingService.Track(AdShowFailedParams.Interstitial(_adPlacement.Name, string.Empty, message));
        }

        private void _OnAdClosedEvent()
        {
            SharedLogger.Log($"{Tag}->_OnAdClosedEvent");
            // Run simple logics
            CloseDateTime = DateTime.Now;
            ShowOperation.Success();
            _showAdLock.Unlock();
            
            SharedCore.Instance.TrackingService.Track(AdClosedParams.Interstitial(_adPlacement.Name));
            
            // Run complex logics
            foreach (var validator in _closeEventValidators) validator.OnAdCloseEvent();
            SharedCoreEvents.InterstitialAd.InvokeInterstitialAdClosedHandlers();
        }
    }
}

#endif
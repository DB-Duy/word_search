#if FACEBOOK_INSTANT
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Shared.Ads.FacebookInstant.Callbacks;
using Shared.Ads.FacebookInstant.Config;
using Shared.Ads.Validator;
using Shared.Tracking.Models;
using Shared.Tracking.Models.Ads;
using Shared.Utils;
using UnityEngine;

namespace Shared.Ads.FacebookInstant.Banner
{
    public class FacebookInstantBannerAd : MonoBehaviour, IBannerAd
    {
        [DllImport("__Internal")]
        private static extern void LoadFacebookInstantBannerAd(string bannerID);
        [DllImport("__Internal")]
        private static extern void HideFacebookInstantBannerAd();

        const string TAG = "FacebookInstantBannerAd";
        public bool IsSetupDone { get; private set; } = false;
        private IAdController _adController;
        private readonly HashSet<IAdValidator> _validators = new();
        
        public void Setup(IAdController adController)
        {
            if (!IsSetupDone)
            {
                IsSetupDone = true;
                _adController = adController;
                _AddListeners();
            }
            else Debug.LogErrorFormat("{0}->Setup: This object already setup!", TAG);
        }

        private void _AddListeners()
        {
            FacebookInstantEvents.Banner.onAdLoadedEvent += _onAdLoadedEvent;
            FacebookInstantEvents.Banner.onAdLoadFailedEvent += _onAdLoadFailedEvent;
        }

        public void LoadBanner()
        {
            SharedLogger.Log($"{TAG}->LoadBanner");
            var errorMessage = string.Empty;
            if (!_Validate()) errorMessage = "!_Validate()";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.Log($"{TAG}->LoadBanner: ERROR: {errorMessage}");
                return;
            }

            var adConfig = (IFacebookInstantAdConfig)_adController.AdConfig;
            LoadFacebookInstantBannerAd(adConfig.FacebookBannerId);
            SharedCore.Instance.TrackingService.Track(AdLoadParams.BannerAdLoadParams());
        }

        public void DestroyBanner()
        {
            SharedLogger.Log($"{TAG}->DestroyBanner");
            HideFacebookInstantBannerAd();
        }

        public IBannerAd AddValidators(params IAdValidator[] validators)
        {
            _validators.AddRange(validators);
            return this;
        }
        
        private bool _Validate()
        {
            var p = new FacebookInstantBannerValidationParams();
            foreach (var validator in _validators)
            {
                var r = validator.Validate(p);
                if (!r.IsSuccess)
                {
                    SharedLogger.Log($"{TAG}->_Validate: FAILED: {r.FailReason}");
                    return false;
                }
                if (!string.IsNullOrEmpty(r.FailReason))
                {
                    SharedLogger.Log($"{TAG}->_Validate: IGNORE: {r.FailReason}");
                }
            }

            return true;
        }
        
        // --------------------------------------------------------------------------
        // Callbacks
        // --------------------------------------------------------------------------
        private void _onAdLoadedEvent()
        {
            SharedLogger.Log($"{TAG}->_onAdLoadedEvent");
            SharedCore.Instance.TrackingService.Track(AdLoadSucceededParams.Banner());
        }

        private void _onAdLoadFailedEvent(string message)
        {
            SharedLogger.LogError($"{TAG}->_onAdLoadFailedEvent: {message}");
            SharedCore.Instance.TrackingService.Track(AdLoadFailedParams.Banner(message));
        }
    }
}
#endif
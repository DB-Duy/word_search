#if TOPON
using System.Collections.Generic;
using System.Diagnostics;
using AnyThinkAds.Api;
using Newtonsoft.Json;
using Shared.Ads.Validator;
using Shared.Common;
using Shared.Tracking.Models;
using Shared.Tracking.Models.Ads;
using Shared.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Shared.Ads.SharedTopOn.Banner
{
    [DisallowMultipleComponent]
    public class TopOnBannerAd : MonoBehaviour, IBannerAd
    {
        private const string TAG = "TopOnBannerAd";
        public bool IsSetupDone { get; private set; } = false;
        private IAdController _adController;
        private readonly HashSet<IAdValidator> _validators = new();
        private ITopOnAdConfig _adConfig;
        private IAsyncOperation _loadOperation;
        
        public void Setup(IAdController adController)
        {
            if (!IsSetupDone)
            {
                IsSetupDone = true;
                _adController = adController;
                _adConfig = (ITopOnAdConfig)adController.AdConfig;
                _AddListeners();
            }
            else Debug.LogErrorFormat("{0}->Setup: This object already setup!", TAG);
        }

        private void _AddListeners()
        {
            ATBannerAd.Instance.client.onAdAutoRefreshEvent += _onAdAutoRefresh;
            ATBannerAd.Instance.client.onAdAutoRefreshFailureEvent += _onAdAutoRefreshFail;
            ATBannerAd.Instance.client.onAdClickEvent += _onAdClick;
            ATBannerAd.Instance.client.onAdCloseEvent += _onAdClose;
            ATBannerAd.Instance.client.onAdCloseButtonTappedEvent += _onAdCloseButtonTapped;
            ATBannerAd.Instance.client.onAdImpressEvent += _onAdImpress;
            ATBannerAd.Instance.client.onAdLoadEvent += _onAdLoad;
            ATBannerAd.Instance.client.onAdLoadFailureEvent += _onAdLoadFail;
            ATBannerAd.Instance.client.onAdSourceAttemptEvent += _startLoadingADSource;
            ATBannerAd.Instance.client.onAdSourceFilledEvent += _finishLoadingADSource;
            ATBannerAd.Instance.client.onAdSourceLoadFailureEvent += _failToLoadADSource;
            ATBannerAd.Instance.client.onAdSourceBiddingAttemptEvent += _startBiddingADSource;
            ATBannerAd.Instance.client.onAdSourceBiddingFilledEvent += _finishBiddingADSource;
            ATBannerAd.Instance.client.onAdSourceBiddingFailureEvent += _failBiddingADSource;
        }

        public void LoadBanner()
        {
            if (_loadOperation != null) return;
            SharedLogger.Log($"{TAG}->LoadBanner");

            var internetReachability = SharedCore.Instance.InternetReachabilityController.IsAvailable; 
            var errorMessage = string.Empty;
            if (!internetReachability) errorMessage = "!internetReachability";
            else if (string.IsNullOrEmpty(_adConfig.TopOnBanner)) errorMessage = $"string.IsNullOrEmpty({_adConfig.TopOnBanner})"; 
            else if (!_Validate()) errorMessage = "!_Validate()";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.Log($"{TAG}->LoadBanner: ERROR: {errorMessage}");
                return;
            }
            _loadOperation = new SharedAsyncOperation();
            // Configure the width and height of the banner to be displayed, whether to use pixel as the unit(Only valid for iOS, Android uses pixel as the unit)
            // Note that banner ads on different platforms have certain restrictions. For example, the configured CSJ(TT) banner AD is 640*100. In order to fill
            // the screen width, the height H = (screen width *100)/640 is calculated. Then the extra size of the load is (screen width: H).
            var bannerSize = new ATSize(Screen.width, ((Screen.width * 100) / 640), true);
            var pairs = new Dictionary<string, object>
            {
                { ATBannerAdLoadingExtra.kATBannerAdLoadingExtraBannerAdSizeStruct, bannerSize },
                { ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveWidth, bannerSize.width },
                { ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveOrientation, ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveOrientationCurrent }
            };
            
            ATBannerAd.Instance.loadBannerAd(_adConfig.TopOnBanner, pairs);
            SharedCore.Instance.TrackingService.Track(AdLoadParams.BannerAdLoadParams());
        }

        public void DestroyBanner()
        {
            SharedLogger.Log($"{TAG}->DestroyBanner");
            if (_loadOperation != null && !_loadOperation.IsComplete) _loadOperation.Fail("DestroyBanner");
            _loadOperation = null;
            ATBannerAd.Instance.cleanBannerAd(_adConfig.TopOnBanner);
        }

        public IBannerAd AddValidators(params IAdValidator[] validators)
        {
            _validators.AddRange(validators);
            return this;
        }
        
        private bool _Validate()
        {
            var p = new TopOnBannerValidationParams();
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
        // Load until success.
        // --------------------------------------------------------------------------
        private void Update()
        {
            if (Time.frameCount % 180 == 0)
            {
                if (_loadOperation != null && _loadOperation.IsComplete && !_loadOperation.IsSuccess)
                {
                    _loadOperation = null;
                    LoadBanner();    
                }
            }
        }
        // --------------------------------------------------------------------------
        // Callbacks
        // --------------------------------------------------------------------------
        private void _onAdLoad(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdLoad", erg);
            if (_loadOperation != null)
            {
                _loadOperation.Success();

                ATBannerAd.Instance.showBannerAd(_adConfig.TopOnBanner, ATBannerAdLoadingExtra.kATBannerAdShowingPisitionBottom);
                SharedCore.Instance.TrackingService.Track(AdLoadSucceededParams.Banner());
            }
            else
            {
                // {"placementId":"b1f7vpdm1ut2co","callbackInfo":{"network_firm_id":0,"adsource_id":"","adsource_index":-1,"adsource_price":0.0,"adsource_isheaderbidding":0,"id":"","publisher_revenue":0.0,"currency":"","country":"","adunit_id":"","adunit_format":"","precision":"","network_type":"","network_placement_id":"","ecpm_level":0,"segment_id":0,"scenario_id":"","user_load_extra_data":null,"scenario_reward_name":"","scenario_reward_number":0,"abtest_id":0,"sub_channel":"","channel":"","custom_rule":null,"ext_info":null,"reward_custom_data":""},"isTimeout":false,"isDeeplinkSucceed":false}
                Debug.LogError($"{TAG}->_onAdLoad: _loadOperation = null");
            }
        }

        private void _onAdLoadFail(object sender, ATAdErrorEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdLoadFail", erg);
            _loadOperation.Fail("_onAdLoadFail");
            SharedCore.Instance.TrackingService.Track(AdLoadFailedParams.Banner(erg.errorCode, erg.errorMessage));
        }
        
        private void _onAdAutoRefresh(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdAutoRefresh", erg);
        }

        private void _onAdAutoRefreshFail(object sender, ATAdErrorEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdAutoRefreshFail", erg);
        }

        private void _onAdClick(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdClick", erg);
            SharedCore.Instance.TrackingService.Track(AdClickedParams.Banner("bottom_banner"));
        }

        private void _onAdClose(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdClose", erg);
        }

        private void _onAdCloseButtonTapped(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdCloseButtonTapped", erg);
        }

        private void _onAdImpress(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_onAdImpress", erg);
            // ((TopOnAdsManager)_adManager).OnImpressionSuccessEvent(erg.callbackInfo);
        }

        private void _startLoadingADSource(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_startLoadingADSource", erg);
        }

        private void _finishLoadingADSource(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_finishLoadingADSource", erg);
        }

        private void _failToLoadADSource(object sender, ATAdErrorEventArgs erg)
        {
            _DebugATAdEventArgs("_failToLoadADSource", erg);
        }

        private void _startBiddingADSource(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_startBiddingADSource", erg);
        }

        private void _finishBiddingADSource(object sender, ATAdEventArgs erg)
        {
            _DebugATAdEventArgs("_finishBiddingADSource", erg);
        }

        private void _failBiddingADSource(object sender, ATAdErrorEventArgs erg)
        {
            _DebugATAdEventArgs("_failBiddingADSource", erg);
        }
        
        // --------------------------------------------------------------------------
        // Utility functions
        // --------------------------------------------------------------------------
        [Conditional("LOG_INFO")]
        private void _DebugATAdEventArgs(string fromFunction, ATAdEventArgs erg)
        {
            if (erg == null)
            {
                SharedLogger.Log($"{TAG}->{fromFunction}._DebugATAdEventArgs: erg is NULL.");
                return;
            }

            var logDict = new Dictionary<string, object>()
            {
                {"placementId", erg.placementId},
                {"callbackInfo", erg.callbackInfo == null ? "null" : erg.callbackInfo.toDictionary()},
                {"isTimeout", erg.isTimeout},
                {"isDeeplinkSucceed", erg.isDeeplinkSucceed}
            };
            if (erg is ATAdErrorEventArgs err)
            {
                logDict.Add("errorMessage", err.errorMessage);
                logDict.Add("errorCode", err.errorCode);
            }

            SharedLogger.Log($"{TAG}->{fromFunction}._DebugATAdEventArgs: {JsonConvert.SerializeObject(logDict)}");
        }
    }
}
#endif
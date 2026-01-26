#if LEVEL_PLAY
using System.Collections;
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Entity.Ads;
using Shared.Repository.Ads;
using Shared.Service.Ads.Common;
using Shared.Service.NotificationCenter;
using Shared.Service.SharedCoroutine;
using Shared.Service.Tracking;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace Shared.Service.Ads.SharedLevelPlay.FS
{
    public class LevelPlayInterstitialAdWrapper : ISharedUtility, ISharedLogTag
    {
        public string LogTag => SharedLogTag.AdNLevelPlayNInterstitial_;
        public string Id { get; private set; }
        public LifeCycleState LifeCycleState { get; private set; }

        public string UnitId { get; set; }
        public string PlacementName { get; set; }
        public IAsyncHandler<ApsInterstitialConfig> ApsFetcher { get; set; }
        public ApsInterstitialConfig ApsInterstitialConfig { get; set; }

        private LevelPlayInterstitialAd _ad;
        private IAsyncOperation _showOperation;
        public LevelPlayAdInfo CachedAdInfo { get; private set; }
        public bool IsZeroRevenueAd => CachedAdInfo != null && CachedAdInfo.FixedRevenue() <= 0; 

        private IAdPlacement _adPlacement;

        public bool RequireCreate => LifeCycleState is LifeCycleState.Destroyed or LifeCycleState.ShowSuccess or LifeCycleState.ShowFail or LifeCycleState.LoadFail; 
        public bool RequireLoad => LifeCycleState == LifeCycleState.Created;
        public bool CouldCreate => Time.time >= _nextRetryTimestamp;

        private float _nextRetryTimestamp;
        private int _retryCount;
        private const float RetryPowerValue = 2;
        private const float MaxRetryCountValue = 6;
        private float _fakeLoadingTime;
        
        public AdsBreakConfigRepository AdsBreakConfigRepository { get; set; }
        
        public LevelPlayInterstitialAdWrapper(string id, float fakeLoadingTime)
        {
            Id = id;
            _fakeLoadingTime = fakeLoadingTime;
        }

        public void Create()
        {
            if (_ad != null) DestroyAd();
            _ad = new LevelPlayInterstitialAd(UnitId);
            // Register to interstitial events
            _ad.OnAdLoaded += _OnAdLoaded;
            _ad.OnAdLoadFailed += _OnAdLoadFailed;
            
            _ad.OnAdDisplayed += _OnAdDisplayed;
            _ad.OnAdDisplayFailed += _OnAdDisplayFailed;
            
            _ad.OnAdClicked += _OnAdClicked;
            _ad.OnAdClosed += _OnAdClosed;
            _ad.OnAdInfoChanged += _OnAdInfoChanged;

            LifeCycleState = LifeCycleState.Created;
        }

        public void LoadAd()
        {
            if (LifeCycleState != LifeCycleState.Created)
            {
                this.LogError(nameof(Id), Id, nameof(LifeCycleState), LifeCycleState.ToString());
                return;
            }

            LifeCycleState = LifeCycleState.LoadStart;
            this.LogInfo(nameof(Id), Id, nameof(UnitId), UnitId, nameof(LifeCycleState), LifeCycleState);
            CachedAdInfo = null;
            this.StartSharedCoroutine(_LoadAdCoroutine());
        }

        private IEnumerator _LoadAdCoroutine()
        {
            this.Track(AdLoadParams.InterstitialAdLoadParams());
            if (ApsFetcher != null && ApsInterstitialConfig != null)
            {
                this.LogInfo("f", nameof(_LoadAdCoroutine), nameof(Id), Id, nameof(ApsInterstitialConfig), ApsInterstitialConfig);
                var timeOut = Time.realtimeSinceStartup + 5f;
                var o = ApsFetcher.Handle(ApsInterstitialConfig);
                while (!o.IsComplete  && timeOut < Time.realtimeSinceStartup) yield return null;
            }
            _ad.LoadAd();
        }

        public IAsyncOperation ShowAd(IAdPlacement placement)
        {
            if (LifeCycleState != LifeCycleState.LoadSuccess)
            {
                this.LogError(nameof(Id), Id, nameof(LifeCycleState), LifeCycleState.ToString());
                return new SharedAsyncOperation().Fail($"LifeCycleState({LifeCycleState.ToString()}) != LifeCycleState.LoadSuccess");
            }
            
            if (!string.IsNullOrEmpty(PlacementName) && LevelPlayInterstitialAd.IsPlacementCapped(PlacementName))
            {
                this.LogError(nameof(Id), Id, "fail_reason", $"IsPlacementCapped({PlacementName})");
                return new SharedAsyncOperation().Fail($"IsPlacementCapped({PlacementName})");
            }

            if (_showOperation != null) return _showOperation;
            _showOperation = new SharedAsyncOperation();
            _adPlacement = placement;
            LifeCycleState = LifeCycleState.ShowStart;
            this.StartSharedCoroutine(_ShowAd());
            return _showOperation;
        }
        
        private IEnumerator _ShowAd()
        {
            this.LogInfo("f", nameof(_ShowAd), nameof(Id), Id, nameof(PlacementName), PlacementName);
            
            AdEvents.OnInterstitialAdFakeLoadingStartedEvent?.Invoke();
            var config = this.AdsBreakConfigRepository.Get();
            if (config == null || !config.Unlock)
            {
                yield return new WaitForSecondsRealtime(_fakeLoadingTime); // Default 1.5f
            }
            else
            {
                yield return new WaitForSecondsRealtime(config.TimeDisplay);
            }
            
            AdEvents.OnInterstitialShowStartedEvent?.Invoke();
            SharedNotificationCenter.Notify(NotificationId.InterstitialStarted);
            _ad.ShowAd(PlacementName);
            yield return new WaitForSecondsRealtime(0.3f);
            AdEvents.OnInterstitialAdFakeLoadingCompletedEvent?.Invoke();
        }
        
        public bool IsAdReady() => _ad != null && _ad.IsAdReady();

        public void DestroyAd()
        {
            this.LogInfo(nameof(Id), Id);
            _ad?.DestroyAd();
            _showOperation = null;
            CachedAdInfo = null;
            LifeCycleState = LifeCycleState.Destroyed;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Implement the events
        // -------------------------------------------------------------------------------------------------------------
        private void _OnAdLoaded(LevelPlayAdInfo adInfo)
        {
            if (adInfo == null)
            {
                this.LogError(nameof(Id), Id, nameof(adInfo), "null");
                LifeCycleState = LifeCycleState.LoadFail;
                CachedAdInfo = null;
                return;
            }

            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo?.DebugLessField());
            LifeCycleState = LifeCycleState.LoadSuccess;
            CachedAdInfo = adInfo;
            _retryCount = 0;
            this.Track(AdLoadSucceededParams.Interstitial(adInfo.AdUnitName, adInfo.AdUnitId));
        }

        private void _OnAdLoadFailed(LevelPlayAdError error)
        {
            if (error == null) this.LogError(nameof(Id), Id, nameof(error), "null");
            else this.LogError(nameof(Id), Id, nameof(error), error);
            var adUnitName = "null";
            var adUnitId = error?.AdUnitId ?? "null";
            LifeCycleState = LifeCycleState.LoadFail;
            CachedAdInfo = null;
            _retryCount++;
            _nextRetryTimestamp = Time.time + Mathf.Pow(RetryPowerValue, Mathf.Min(_retryCount, MaxRetryCountValue));
            this.Track(AdLoadFailedParams.Interstitial(error?.ErrorCode.ToString() ?? "1989", error?.ErrorMessage ?? "error is null.", adUnitName, adUnitId));
        }

        private void _OnAdDisplayed(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            this.Track(AdShowSucceededParams.Interstitial(_adPlacement.Name, adInfo.AdUnitName, adInfo.AdUnitId));
        }

        private void _OnAdDisplayFailed(LevelPlayAdDisplayInfoError infoError)
        {
            var errorCode = infoError?.LevelPlayError == null ? "Unknown" : infoError.LevelPlayError.ErrorCode.ToString();
            var errorMessage = infoError?.LevelPlayError == null ? "infoError?.LevelPlayError == null" : infoError.LevelPlayError.ErrorMessage;
            var adUnitName = infoError?.DisplayLevelPlayAdInfo?.AdUnitName ?? "null";
            var adUnitId = infoError?.DisplayLevelPlayAdInfo?.AdUnitId ?? "null";
            this.LogError(nameof(Id), Id, nameof(infoError), infoError, nameof(errorMessage), errorMessage);
            if (_showOperation == null) Debug.LogError("LevelPlayInterstitialAdWrapper->_OnAdDisplayFailed: _showOperation == null");
            LifeCycleState = LifeCycleState.ShowFail;
            _showOperation?.Fail(errorMessage);
            _showOperation = null;
            AdEvents.OnInterstitialShowCompletedEvent?.Invoke();
            this.Track(AdShowFailedParams.Interstitial(_adPlacement.Name, errorCode, errorMessage, adUnitName, adUnitId));
        }

        private void _OnAdClicked(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo?.DebugLessField());
            this.Track(AdClickedParams.Interstitial(_adPlacement.Name, adInfo.AdUnitName, adInfo.AdUnitId));
        }

        /// <summary>
        /// {"adUnitId":"3i0z2zd6lxxllzd2","adUnitName":"Interstitial","adSize":null,"adFormat":"interstitial","placementName":"","auctionId":"e3353030-ff13-11ef-a4b6-63cac602dace_1087940059","country":"US","ab":"N/A","segmentName":"","adNetwork":"mintegral","instanceName":"Bidding","instanceId":"2462988","revenue":0.0025312,"precision":"BID","encryptedCPM":"","AdId":"25e936b1-c56c-4213-9ed3-eb1ad4aa0df4","AdUnitId":"3i0z2zd6lxxllzd2","AdUnitName":"Interstitial","AdSize":null,"AdFormat":"interstitial","PlacementName":"","AuctionId":"e3353030-ff13-11ef-a4b6-63cac602dace_1087940059","CreativeId":null,"Country":"US","Ab":"N/A","SegmentName":"","AdNetwork":"mintegral","InstanceName":"Bidding","InstanceId":"2462988","Revenue":0.0025312,"Precision":"BID","EncryptedCPM":""}
        ///
        /// • Fix AdInfo callbacks alignment with ILR postbacks for banners in Multiple Ad Units APIs
        /// • Fix duplicate callbacks for rewarded ads in Multiple Ad Units APIs
        /// ==>>>>>> Fix these stupid bugs via this.Track(AdClosedParams.Interstitial(_adPlacement == null ?  "null" : _adPlacement.Name));
        /// </summary>
        private void _OnAdClosed(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(_showOperation), _showOperation == null ? null : "OK", nameof(adInfo), adInfo);
            if (_showOperation == null) return;
            
            LifeCycleState = LifeCycleState.ShowSuccess;
            _showOperation.Success();
            _showOperation = null;
            if (_resumedCoroutine != null)
            {
                this.StopSharedCoroutine(_resumedCoroutine);
                _resumedCoroutine = null;
            }
            AdEvents.OnInterstitialShowCompletedEvent?.Invoke();
            this.Track(AdClosedParams.Interstitial(_adPlacement?.Name == null ?  "null" : _adPlacement.Name, adInfo.AdUnitName, adInfo.AdUnitId));
        }

        private void _OnAdInfoChanged(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            if (CachedAdInfo != null && CachedAdInfo.FixedRevenue() > adInfo.FixedRevenue())
                this.LogError(nameof(Id), Id, nameof(CachedAdInfo.Revenue), CachedAdInfo.FixedRevenue(), nameof(adInfo.Revenue), adInfo.FixedRevenue());
            CachedAdInfo = adInfo;
        }

        private Coroutine _resumedCoroutine;
        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) return;
            if (_showOperation == null) return;
            if (_resumedCoroutine != null) this.StopSharedCoroutine(_resumedCoroutine);
            _resumedCoroutine = this.StartSharedCoroutine(_HandleOnApplicationResumed());
        }

        private readonly WaitForSecondsRealtime _wait = new(0.5f);
        private IEnumerator _HandleOnApplicationResumed()
        {
            _wait.Reset();
            yield return _wait;
            if (_showOperation == null) yield break;
            LifeCycleState = LifeCycleState.ShowFail;
            _showOperation.Fail("_HandleOnApplicationResumed");
            _showOperation = null;
            AdEvents.OnInterstitialShowCompletedEvent?.Invoke();
        }
    }
}
#endif
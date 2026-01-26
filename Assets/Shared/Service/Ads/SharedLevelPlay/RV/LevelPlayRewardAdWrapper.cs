#if LEVEL_PLAY
using System.Collections;
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Core.Resolver;
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
using TimeOut = Shared.Utils.TimeOut;

namespace Shared.Service.Ads.SharedLevelPlay.RV
{
    public class LevelPlayRewardAdWrapper : ISharedUtility, ISharedLogTag
    {
        private static readonly IAsyncOperation<TimeOut> AlwaysSuccessLoadOperation = new SharedAsyncOperation<TimeOut>(new TimeOut(30f)).Success();
        
        public string Id { get; private set; }
        public string AdUnitId { get; set; }
        public string PlacementName { get; set; }

        public IResolver<string, RewardedAdShowFailReason> FailMessageResolver { get; set; }
        public IAsyncHandler<string> ApsRewardedFetcher { get; set; }
        public string ApsSlotId { get; set; }

        private LevelPlayRewardedAd _ad;
        public LevelPlayAdInfo CachedAdInfo { get; private set; }
        private IAdPlacement _placement;
        
        private IAsyncOperation<RewardedAdOperation> _showOperation;
        public LifeCycleState LifeCycleState { get; set; } = LifeCycleState.Initialized;

        public bool RequireCreate => LifeCycleState is LifeCycleState.Destroyed or LifeCycleState.LoadFailed or LifeCycleState.ShowFailed or LifeCycleState.ShowSucceeded or LifeCycleState.Initialized;
        public bool RequireLoad => LifeCycleState is LifeCycleState.Created;
        public bool CouldCreate => Time.time >= _nextRetryTimestamp;

        private float _nextRetryTimestamp;
        private int _retryCount;
        private const float RetryPowerValue = 2;
        private const float MaxRetryCountValue = 6;

        public LevelPlayRewardAdWrapper(string id)
        {
            Id = id;
        }

        public void Create()
        {
            if (_ad != null) DestroyAd();
            this.LogInfo(nameof(Id), Id, nameof(AdUnitId), AdUnitId);
            _ad = new LevelPlayRewardedAd(AdUnitId);
            _ad.OnAdLoaded += _OnAdLoaded;
            _ad.OnAdLoadFailed += _OnAdLoadFailed;
            _ad.OnAdDisplayed += _OnAdDisplayed;
            _ad.OnAdDisplayFailed += _OnAdDisplayFailed;
            _ad.OnAdRewarded += _OnAdRewarded;
            _ad.OnAdClicked += _OnAdClicked;
            _ad.OnAdClosed += _OnAdClosed;
            _ad.OnAdInfoChanged += _OnAdInfoChanged;
            LifeCycleState = LifeCycleState.Created;
        }

        public void LoadAd()
        {
            if (LifeCycleState != LifeCycleState.Created)
            {
                this.LogError(nameof(Id), Id, "reason", $"LifeCycleState({LifeCycleState.ToString()}) != LifeCycleState.Created");
                return;
            }

            this.LogInfo(nameof(Id), Id);
            LifeCycleState = LifeCycleState.LoadStarted;
            CachedAdInfo = null;
            this.StartSharedCoroutine(_LoadInternal());
        }

        private IEnumerator _LoadInternal()
        {
            if (ApsRewardedFetcher != null && !string.IsNullOrEmpty(ApsSlotId))
            {
                this.LogInfo(nameof(Id), Id, nameof(ApsSlotId), ApsSlotId);
                var o = ApsRewardedFetcher.Handle(apsSlotId: ApsSlotId);
                while (!o.IsComplete) yield return null;
            }
            _ad?.LoadAd();
            this.Track(AdLoadParams.RewardedAdLoadParams());
        }

        public IAsyncOperation<RewardedAdOperation> ShowAd(IAdPlacement placement)
        {
            if (LifeCycleState != LifeCycleState.LoadSucceeded)
            {
                this.LogError(nameof(Id), Id, nameof(LifeCycleState), LifeCycleState.ToString());
                return new SharedAsyncOperation<RewardedAdOperation>(new RewardedAdOperation(placement.Name)).Fail($"LifeCycleState({LifeCycleState.ToString()}) != LifeCycleState.LoadSucceeded");
            }
            
            if (_showOperation != null) return _showOperation;
            
            if (!IsAdReady())
            {
                this.Track(AdNotReadyParams.Rewarded(placement.Name));
                return new SharedAsyncOperation<RewardedAdOperation>(new RewardedAdOperation(placement.Name)).Fail("!IsAdReady()");
            }

            if (!string.IsNullOrEmpty(PlacementName) && LevelPlayRewardedAd.IsPlacementCapped(PlacementName))
            {
                this.LogError(nameof(Id), Id, "fail_reason", $"IsPlacementCapped({PlacementName})");
                return new SharedAsyncOperation<RewardedAdOperation>(new RewardedAdOperation(placement.Name)).Fail($"IsPlacementCapped({PlacementName})");
            }

            this.LogInfo(nameof(Id), Id, nameof(PlacementName), PlacementName);
            _showOperation = new SharedAsyncOperation<RewardedAdOperation>(new RewardedAdOperation(placement.Name));
            _placement = placement;
            CachedAdInfo = null;
            LifeCycleState = LifeCycleState.ShowStarted;
            this.Track(AdReadyParams.Rewarded(placement.Name));
            SharedNotificationCenter.Notify(NotificationId.RewardedStarted);
            _ad?.ShowAd(PlacementName);
            return _showOperation;
        }

        public bool IsAdReady() => _ad.IsAdReady();
        public bool ContainsZeroRevenue() => CachedAdInfo != null && CachedAdInfo.FixedRevenue() == 0;
        
        public void DestroyAd()
        {
            this.LogInfo(nameof(Id), Id);
            _ad?.DestroyAd();
            _ad = null;
            CachedAdInfo = null;
            LifeCycleState = LifeCycleState.Destroyed;
        }

        private void _OnAdLoaded(LevelPlayAdInfo adInfo)
        {
            if (adInfo == null)
            {
                this.LogError(nameof(Id), Id, nameof(adInfo), "null");
                LifeCycleState = LifeCycleState.LoadFailed;
                CachedAdInfo = null;
                return;
            }
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            CachedAdInfo = adInfo;
            LifeCycleState = LifeCycleState.LoadSucceeded;
            _retryCount = 0;
            this.Track(AdLoadSucceededParams.Rewarded(adInfo.AdUnitName, adInfo.AdUnitId));
        }

        private void _OnAdLoadFailed(LevelPlayAdError adError)
        {
            if (adError == null) this.LogError(nameof(Id), Id, nameof(adError), "null");
            this.LogInfo(nameof(Id), Id, nameof(adError), adError);
            var adUnitName = "null";
            var adUnitId = adError?.AdUnitId ?? "null";
            CachedAdInfo = null;
            LifeCycleState = LifeCycleState.LoadFailed;
            _retryCount++;
            _nextRetryTimestamp = Time.time + Mathf.Pow(RetryPowerValue, Mathf.Min(_retryCount, MaxRetryCountValue));
            this.Track(AdLoadFailedParams.Rewarded(adError?.ErrorCode.ToString() ?? "1989", adError?.ErrorMessage ?? "adError is null.", adUnitName, adUnitId));
        }

        private void _OnAdDisplayed(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            this.Track(AdShowSucceededParams.Rewarded(_placement.Name, adInfo.AdUnitName, adInfo.AdUnitId));
            AdEvents.OnRewardedAdShowStartedEvent.Invoke();
        }

        private void _OnAdDisplayFailed(LevelPlayAdDisplayInfoError adInfoError)
        {
            var errorCode = adInfoError?.LevelPlayError == null ? "Unknown" : adInfoError.LevelPlayError.ErrorCode.ToString();
            var errorMessage = adInfoError?.LevelPlayError == null ? "adInfoError?.LevelPlayError == null" : adInfoError.LevelPlayError.ErrorMessage;
            var adUnitName = adInfoError?.DisplayLevelPlayAdInfo?.AdUnitName ?? "null";
            var adUnitId = adInfoError?.DisplayLevelPlayAdInfo?.AdUnitId ?? "null";
            this.LogError(nameof(Id), Id, nameof(adInfoError), adInfoError, nameof(errorCode), errorCode, nameof(errorMessage), errorMessage);
            LifeCycleState = LifeCycleState.ShowFailed;
            if (_showOperation != null)
            {
                _showOperation.Data.LocalizedErrorMessage = FailMessageResolver?.ResolveUserMessage();
                _showOperation.Fail(errorCode);
                _showOperation = null;
            }
            this.Track(AdShowFailedParams.Rewarded(_placement?.Name ?? "Unknown", errorCode, errorMessage, adUnitName, adUnitId));
            AdEvents.OnRewardedAdShowCompletedEvent?.Invoke();
        }

        private void _OnAdRewarded(LevelPlayAdInfo adInfo, LevelPlayReward adReward)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo?.DebugLessField(), nameof(_placement), _placement);
            if (_showOperation != null)
                _showOperation.Data.IsRewarded = true;
            else 
                this.LogError(nameof(Id), Id, nameof(adInfo), adInfo?.DebugLessField(), nameof(_showOperation), "null");
            this.Track(AdRewardedParams.Rewarded(_placement == null ?  "null" : _placement.Name));
        }

        private void _OnAdClicked(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo?.DebugLessField());
            this.Track(AdClickedParams.Rewarded(_placement.Name, adInfo?.AdUnitName, adInfo?.AdUnitId));
        }

        private void _OnAdClosed(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo?.DebugLessField(), nameof(_showOperation), _showOperation == null ? "null" : "notnull");
            _CleanResumedCoroutine();
            this.StartSharedCoroutine(_DelayAndHandleOnAdClosed());
            this.Track(AdClosedParams.Rewarded(_placement == null ?  "null" : _placement.Name, adInfo.AdUnitName, adInfo.AdUnitId));
        }

        private void _OnAdInfoChanged(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            if (CachedAdInfo != null && adInfo.FixedRevenue() < CachedAdInfo.FixedRevenue())
                this.LogError(nameof(Id), Id, nameof(CachedAdInfo.Revenue), CachedAdInfo.FixedRevenue(), nameof(adInfo.Revenue), adInfo.FixedRevenue());
            CachedAdInfo = adInfo;
        }

        private IEnumerator _DelayAndHandleOnAdClosed()
        {
            yield return null;
            yield return null;
            this.LogInfo(nameof(Id), Id);
            if (_showOperation == null) yield break;

            if (_showOperation.Data.IsRewarded)
            {
                LifeCycleState = LifeCycleState.ShowSucceeded;
                _showOperation.Success();
            }
            else
            {
                LifeCycleState = LifeCycleState.ShowFailed;
                _showOperation.Fail("!_showOperation.Data.IsRewarded");
            }

            _showOperation.Data.IsClosed = true;
            _showOperation = null;
            
            AdEvents.OnRewardedAdShowCompletedEvent.Invoke();
            _CleanResumedCoroutine();
        }

        public string LogTag => SharedLogTag.AdNLevelPlayNReward_;
        
        private Coroutine _resumedCoroutine;
        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) return;
            if (_showOperation == null) return;
            this.LogInfo("f", nameof(OnApplicationPause), nameof(Id), Id);
            _CleanResumedCoroutine();
            _resumedCoroutine = this.StartSharedCoroutine(_HandleOnApplicationResumed());
        }

        private readonly WaitForSecondsRealtime _wait = new(1f);
        private IEnumerator _HandleOnApplicationResumed()
        {
            _wait.Reset();
            yield return _wait;
            if (_showOperation == null) yield break;
            
            this.LogInfo("f", nameof(_HandleOnApplicationResumed), nameof(Id), Id);

            if (_showOperation.Data.IsRewarded)
            {
                LifeCycleState = LifeCycleState.ShowSucceeded;
                _showOperation.Data.IsClosed = true;
                _showOperation.Success();
            }
            else
            {
                LifeCycleState = LifeCycleState.ShowFailed;
                _showOperation.Fail("Ad closed by _HandleOnApplicationResumed");
                _showOperation.Data.IsClosed = true;
            }
            _showOperation = null;
            AdEvents.OnRewardedAdShowCompletedEvent.Invoke();
            _resumedCoroutine = null;
        }
        
        private void _CleanResumedCoroutine()
        {
            if (_resumedCoroutine == null) return;
            this.StopSharedCoroutine(_resumedCoroutine);
            _resumedCoroutine = null;
        }
    }
}
#endif
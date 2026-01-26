#if LEVEL_PLAY
using System.Collections;
using Shared.Entity.Ads;
using Shared.Service.SharedCoroutine;
using Shared.Service.Tracking;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace Shared.Service.Ads.SharedLevelPlay.Banner
{
    public class LevelPlayBannerWrapper : ISharedUtility, ISharedLogTag
    {
        public string LogTag => SharedLogTag.AdNLevelPlayNBanner_;
        
        public string Id { get; }
        public string AdUnitId { get; set; }
        public com.unity3d.mediation.LevelPlayAdSize AdSize { get; set; } 
        public com.unity3d.mediation.LevelPlayBannerPosition Position { get; set; }
        public string PlacementName { get; set; } = null;
        public bool DisplayOnLoad { get; set; }
        
        public IApsBannerFetcher ApsBannerFetcher { get; set; }
        public ApsBannerConfig ApsBannerConfig { get; set; }
        
        private ILevelPlayBannerAd _ad;
        public LevelPlayAdInfo CachedAdInfo { get; private set; }
        private bool IsAutoRefreshing { get; set; } = true;
        private LifeCycleState LifecycleLifeCycleState { get; set; } = LifeCycleState.None;
        public bool IsDisplaying => LifecycleLifeCycleState is LifeCycleState.DisplaySuccess or LifeCycleState.DisplayStart;
        public bool RequireCreate => LifecycleLifeCycleState is LifeCycleState.LoadFailed or LifeCycleState.DisplayFailed or LifeCycleState.DisplaySuccess;
        public bool RequireLoad => LifecycleLifeCycleState is LifeCycleState.Created;
        public bool IsZeroRevenue => CachedAdInfo != null && CachedAdInfo.FixedRevenue() == 0;

        public LevelPlayBannerWrapper(int id)
        {
            Id = id.ToString();
        }

        public void CreateBannerAd()
        {
            if (_ad != null) DestroyBannerAd();
            LifecycleLifeCycleState = LifeCycleState.Created;
            _ad = new LevelPlayBannerAd(AdUnitId, AdSize, Position, PlacementName, DisplayOnLoad, respectSafeArea: false);
            _ad.OnAdLoaded += OnADAdLoaded;
            _ad.OnAdLoadFailed += OnADAdLoadFailed;
            _ad.OnAdClicked += OnADAdClicked;
            _ad.OnAdDisplayed += OnADAdDisplayed;
            _ad.OnAdDisplayFailed += OnADAdDisplayFailed;
            _ad.OnAdExpanded += OnADAdExpanded;
            _ad.OnAdCollapsed += OnADAdCollapsed;
            _ad.OnAdLeftApplication += OnADAdLeftApplication;
            this.LogInfo(nameof(Id), Id, nameof(AdUnitId), AdUnitId, nameof(AdSize), AdSize?.Description, nameof(Position), Position.ToString(), nameof(PlacementName), PlacementName, nameof(DisplayOnLoad), DisplayOnLoad);
        }

        public void LoadBannerAd()
        {
            this.LogInfo(nameof(Id), Id);
            this.Track(AdLoadParams.BannerAdLoadParams());
            CachedAdInfo = null;
            LifecycleLifeCycleState = LifeCycleState.LoadStart;
            this.StartSharedCoroutine(_LoadBannerAd());
        }

        private IEnumerator _LoadBannerAd()
        {
            if (ApsBannerFetcher != null && ApsBannerConfig != null)
            {
                // Cho timeout là 5s, vì có khả năng APS sẽ không trả về callback.
                var timeOut = Time.realtimeSinceStartup + 5f;
                this.LogInfo("f", nameof(_LoadBannerAd), nameof(Id), Id, nameof(ApsBannerConfig), ApsBannerConfig);
                var o = ApsBannerFetcher.Handle(ApsBannerConfig);
                while (!o.IsComplete && timeOut < Time.realtimeSinceStartup) yield return null;
            }
            this.LogInfo("f", nameof(_LoadBannerAd), nameof(Id), Id, "call", "_ad?.LoadAd();");
            _ad?.LoadAd();
        }

        public void DestroyBannerAd()
        {
            this.LogInfo(nameof(Id), Id);
            _ad?.DestroyAd();
            _ad = null;
            CachedAdInfo = null;
            LifecycleLifeCycleState = LifeCycleState.None;
        }

        public void HideBannerAd()
        {
            this.LogInfo(nameof(Id), Id);
            _ad?.HideAd();
        }

        public void ShowBannerAd()
        {
            this.LogInfo(nameof(Id), Id);
            CachedAdInfo = null;
            LifecycleLifeCycleState = LifeCycleState.DisplayStart;
            _ad?.ShowAd();
        }

        public void PauseAutoRefresh()
        {
            this.LogInfo(nameof(Id), Id);
            IsAutoRefreshing = false;
            _ad?.PauseAutoRefresh();
        }

        public void ResumeAutoRefresh()
        {
            this.LogInfo(nameof(Id), Id);
            IsAutoRefreshing = true;
            _ad?.ResumeAutoRefresh();
        }

        // Multiple Ad Units callbacks
        private void OnADAdLoaded(LevelPlayAdInfo adInfo)
        {
            if (adInfo == null)
            {
                this.LogError(nameof(Id), Id, nameof(adInfo), "null");
                LifecycleLifeCycleState = LifeCycleState.LoadFailed;
                CachedAdInfo = null;
                return;
            }
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            this.Track(AdLoadSucceededParams.Banner(adInfo.AdUnitName, adInfo.AdUnitId));
            CachedAdInfo = adInfo;
            LifecycleLifeCycleState = LifeCycleState.LoadSuccess;
        }
        
        private void OnADAdLoadFailed(LevelPlayAdError error)
        {
            this.LogInfo(nameof(Id), Id, nameof(error), error);
            var errorCode = error?.ErrorCode.ToString() ?? "1989";
            var errorMessage = error?.ErrorMessage ?? "error is null";
            var adUnitName = "null";
            var adUnitId = error?.AdUnitId ?? "null";
            CachedAdInfo = null;
            LifecycleLifeCycleState = LifeCycleState.LoadFailed;
            this.Track(AdLoadFailedParams.Banner(errorCode, errorMessage, adUnitName, adUnitId));
        }
        
        private void OnADAdDisplayFailed(LevelPlayAdDisplayInfoError error)
        {
            this.LogInfo(nameof(Id), Id, nameof(error), error);
            LifecycleLifeCycleState = LifeCycleState.DisplayFailed;
        }

        private void OnADAdDisplayed(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            LifecycleLifeCycleState = LifeCycleState.DisplaySuccess;
        }

        private void OnADAdLeftApplication(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
        }

        private void OnADAdCollapsed(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
        }

        private void OnADAdExpanded(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
        }

        private void OnADAdClicked(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            this.Track(AdClickedParams.Banner("bottom_banner", adInfo.AdUnitName, adInfo.AdUnitId));
        }

        public void OnBannerImpressionDataReadyEvent(IronSourceImpressionData data)
        {
            if (!IsAutoRefreshing) return;
            this.LogInfo(nameof(Id), Id, nameof(data), data);
            if(ApsBannerFetcher != null && ApsBannerConfig != null) ApsBannerFetcher.Handle(ApsBannerConfig);
        }
    }
}
#endif
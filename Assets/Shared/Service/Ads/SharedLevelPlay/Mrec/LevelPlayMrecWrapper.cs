#if LEVEL_PLAY
using System.Collections;
using Shared.Service.SharedCoroutine;
using Shared.Service.Tracking;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace Shared.Service.Ads.SharedLevelPlay.Mrec
{
    public class LevelPlayMrecWrapper : ISharedUtility, ISharedLogTag
    {
        public string LogTag => SharedLogTag.AdNLevelPlayNMrec;
        
        private string Id { get; }
        private Vector2 Position { get; set; }
        
        private string AdUnitId { get; set; } 
        private string PlacementName { get; set; } = null;
        
        public IApsMrecFetcher ApsFetcher { get; set; }
        public string ApsSlotId { get; set; }
        
        private ILevelPlayBannerAd _ad;
        private MrecLifeCycleState _state = MrecLifeCycleState.None;

        public LevelPlayMrecWrapper(string id, Vector2 position, string adUnitId, string placementName)
        {
            Id = id;
            Position = position;
            AdUnitId = adUnitId;
            PlacementName = placementName;
        }

        public void CreateAd()
        {
            if (_ad != null) DestroyAd();
            _state = MrecLifeCycleState.Created;
            _ad = new LevelPlayBannerAd(AdUnitId, com.unity3d.mediation.LevelPlayAdSize.MEDIUM_RECTANGLE, new com.unity3d.mediation.LevelPlayBannerPosition(Position), PlacementName, false);
            _ad.OnAdLoaded += _OnAdLoaded;
            _ad.OnAdLoadFailed += _OnAdLoadFailed;
            _ad.OnAdClicked += _OnAdClicked;
            _ad.OnAdDisplayed += _OnAdDisplayed;
            _ad.OnAdDisplayFailed += _OnAdDisplayFailed;
            _ad.OnAdExpanded += _OnAdExpanded;
            _ad.OnAdCollapsed += _OnAdCollapsed;
            _ad.OnAdLeftApplication += _OnAdLeftApplication;
            this.LogInfo(nameof(Id), Id, nameof(AdUnitId), AdUnitId, nameof(Position), Position.ToString(), nameof(PlacementName), PlacementName);
        }
        
        public bool CreateIfPositionChanged(Vector2 position)
        {
            var xr = Mathf.Approximately(position.x, Position.x);
            var yr = Mathf.Approximately(position.y, Position.y);
            if (xr && yr) return false;
            Position = position;
            CreateAd();
            return true;
        }

        public void LoadAd()
        {
            this.LogInfo(nameof(Id), Id);
            this.Track(AdLoadParams.MrecAdLoadParams());
            _state = MrecLifeCycleState.LoadStart;
            this.StartSharedCoroutine(_LoadAd());
        }

        private IEnumerator _LoadAd()
        {
            if (ApsFetcher != null)
            {
                this.LogInfo(nameof(Id), Id, nameof(ApsSlotId), ApsSlotId);
                var o = ApsFetcher.Handle(ApsSlotId);
                while (!o.IsComplete) yield return null;
            }
            _ad?.LoadAd();
        }

        public void DestroyAd()
        {
            this.LogInfo(nameof(Id), Id);
            _ad?.DestroyAd();
            _ad = null;
            _state = MrecLifeCycleState.None;
        }

        public void HideAd()
        {
            this.LogInfo(nameof(Id), Id);
            _ad?.HideAd();
            _state = MrecLifeCycleState.Hidden;
        }

        public void ShowAd()
        {
            this.LogInfo(nameof(Id), Id);
            _state = MrecLifeCycleState.DisplayStart;
            _ad?.ShowAd();
        }

        // Multiple Ad Units callbacks
        private void _OnAdLoaded(LevelPlayAdInfo adInfo)
        {
            if (adInfo == null)
            {
                this.LogError(nameof(Id), Id, nameof(adInfo), "null");
                _state = MrecLifeCycleState.LoadFailed;
                return;
            }
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            this.Track(AdLoadSucceededParams.Banner(adInfo.AdUnitName, adInfo.AdUnitId));
            _state = MrecLifeCycleState.LoadSuccess;
        }
        
        private void _OnAdLoadFailed(LevelPlayAdError error)
        {
            this.LogInfo(nameof(Id), Id, nameof(error), error);
            var adUnitName = "null";
            var adUnitId = error?.AdUnitId ?? "null";
            _state = MrecLifeCycleState.LoadFailed;
            this.Track(AdLoadFailedParams.Banner(error?.ErrorCode.ToString() ?? "1989", error?.ErrorMessage ?? "error is null", adUnitName, adUnitId));
        }
        
        private void _OnAdDisplayFailed(LevelPlayAdDisplayInfoError error)
        {
            this.LogInfo(nameof(Id), Id, nameof(error), error);
            _state = MrecLifeCycleState.DisplayFailed;
        }

        private void _OnAdDisplayed(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            _state = MrecLifeCycleState.DisplaySuccess;
        }

        private void _OnAdLeftApplication(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
        }

        private void _OnAdCollapsed(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
        }

        private void _OnAdExpanded(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
        }

        private void _OnAdClicked(LevelPlayAdInfo adInfo)
        {
            this.LogInfo(nameof(Id), Id, nameof(adInfo), adInfo.DebugLessField());
            this.Track(AdClickedParams.Banner(PlacementName, adInfo.AdUnitName, adInfo.AdUnitId));
        }

        public void OnBannerImpressionDataReadyEvent(IronSourceImpressionData data)
        {
            this.LogInfo(nameof(Id), Id, nameof(data), data);
            if(ApsFetcher != null && !string.IsNullOrEmpty(ApsSlotId)) ApsFetcher.Handle(ApsSlotId);
        }
    }
}
#endif
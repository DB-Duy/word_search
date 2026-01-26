#if GADSME
using System.Collections;
using Gadsme;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Service.Gadsme.Internal;
using Shared.Service.InPlayAds;
using Shared.Service.SharedCoroutine;
using Shared.Service.Tracking;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using Shared.View.InPlayAds;
using UnityEngine;

namespace Shared.Service.Gadsme
{
    /// <summary>
    //2023/09/29 10:43:14.640 9712 9876 Info Unity AuAd GadsmeAdsManager->OnLoadAudioAd: load audio ad - Gadsme.GadsmeAudioAdInfo
    //2023/09/29 10:43:18.647 9712 9876 Info Unity AuAd GadsmeAdsManager->OnAudioAdReadyToPlay: audio ad ready to play - Gadsme.GadsmeAudioAdInfo
    //2023/09/29 10:43:33.929 9712 9876 Info Unity AuAd GadsmeAdsManager->Show: gameplay
    //2023/09/29 10:43:34.062 9712 9876 Info Unity AuAd GadsmeAdsManager->OnPlayAudioAd: play audio ad - Gadsme.GadsmeAudioAdInfo
    //2023/09/29 10:43:35.980 9712 9876 Info Unity AuAd GadsmeAdsManager->OnImpressionSuccessEvent::placementId: - gameId:cllbtzwfo0029zv0i1ba44gup - countryCode:VN - currency:USD - netRevenue:0 - lineItemType:sandbox - platform:android
    //2023/09/29 10:43:56.452 9712 9876 Info Unity AuAd GadsmeAdsManager->OnFinishAudioAd::finish audio ad - Gadsme.GadsmeAudioAdInfo(complete: True)
    /// </summary>
    [Service]
    public class GadsmeService : IGadsmeService, ISharedUtility, ISharedLogTag
    {
        private const string Tag = "GadsmeService";

        private IAsyncOperation _initOperation;
        public bool IsInitialized { get; private set; }

        // Audio
        private string _audioPlacement;
        public bool IsAudioAdAvailable { get; private set; }
        private IAsyncOperation _playAudioOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;

            this.LogInfo(nameof(GadsmeSDK.Version), GadsmeSDK.Version);
            
            // GadsmeEvents.ReadyEvent += (args) =>
            // {
            //     this.LogInfo(nameof(args), args.ToDict());
            // };

            // Placement events
            // GadsmeEvents.AdRequestEvent is removed out of SDK 
            //GadsmeEvents.AdRequestEvent += _OnAdRequestEvent;
            GadsmeEvents.PlacementLoadedEvent += _OnPlacementLoadedEvent;
            GadsmeEvents.PlacementVisibleEvent += _OnPlacementVisibleEvent;
            GadsmeEvents.PlacementViewableEvent += _OnPlacementViewableEvent;
            GadsmeEvents.PlacementClickedEvent += _OnPlacementClickedEvent;
            GadsmeEvents.PlacementFailedEvent += _OnPlacementFailedEvent;
            GadsmeEvents.PlacementNoAdEvent += _OnPlacementNoAdEvent;

            // Ad content events
            GadsmeEvents.AdContentLoadedEvent += _OnAdContentLoadedEvent;
            GadsmeEvents.AdContentVisibleEvent += _OnAdContentVisibleEvent;
            GadsmeEvents.AdContentViewableEvent += _OnAdContentViewableEvent;
            GadsmeEvents.AdContentClickedEvent += _OnAdContentClickedEvent;
            GadsmeEvents.AdContentFailedEvent += _OnAdContentFailedEvent;
            GadsmeEvents.AdContentNoAdEvent += _OnAdContentNoAdEvent;

            // Audio ad events
            GadsmeEvents.LoadAudioAdEvent += _OnLoadAudioAdEvent;
            GadsmeEvents.CancelAudioAdEvent += _OnCancelAudioAdEvent;
            GadsmeEvents.AudioAdReadyToPlayEvent += _OnAudioAdReadyToPlayEvent;
            GadsmeEvents.PlayAudioAdEvent += _OnPlayAudioAdEvent;
            GadsmeEvents.AudioAdIncompletePlaythroughEvent += _OnAudioAdIncompletePlayThroughEvent;
            GadsmeEvents.FinishAudioAdEvent += _OnFinishAudioAdEvent;

            // Impression Event
            GadsmeEvents.ImpressionEvent += _OnImpressionEvent;

            GadsmeSDK.SetMainCamera(Camera.main);
            GadsmeSDK.SetAllowConsentDialog(true);
            GadsmeSDK.SetVideoAdVolume(0f);
            GadsmeSDK.Init();

            return _initOperation;
        }
        
        public IAsyncOperation PlayAudioAd(string placement)
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(PlayAudioAd), nameof(placement), placement);
            _audioPlacement = placement;
            if (_playAudioOperation != null) return _playAudioOperation;
            _playAudioOperation = new SharedAsyncOperation();
#if GADSME
            this.StartSharedCoroutine(_LoadAndPlay());
#endif
            return _playAudioOperation;
        }

        public void Stop()
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(Stop));
            GadsmeSDK.StopAudioAd();
        }

        private IEnumerator _LoadAndPlay()
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(_LoadAndPlay), nameof(IsAudioAdAvailable), IsAudioAdAvailable);
            if (IsAudioAdAvailable) GadsmeSDK.StartAudioAd();
            else
            {
                GadsmeSDK.PreloadAudioAd();
                var startTime = Time.realtimeSinceStartup;
                yield return new WaitUntil(() => Time.realtimeSinceStartup - startTime >= 10f || IsAudioAdAvailable);
                if (IsAudioAdAvailable)
                    GadsmeSDK.StartAudioAd();
                else
                {
                    _playAudioOperation?.Fail("!IsAudioAdAvailable");
                    _playAudioOperation = null;
                }
            }
        }

        /// ---------------------------------------------------------------------------------------
        /// Placement events 
        /// ---------------------------------------------------------------------------------------

        // private void _OnAdRequestEvent(GadsmeAdRequestData data)
        // {
        //     this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnAdRequestEvent), data?.adFormat?.ToDict());
        // }
        
        // Fired when no ad is available for the placement.
        private void _OnPlacementNoAdEvent(GadsmePlacement placement)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementLoadedEvent), nameof(placement), placement.ToDict());
        }
        
        //Fired when an ad is loaded, for every placement using that ad.
        private void _OnPlacementLoadedEvent(GadsmePlacement placement)
        {
            
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementLoadedEvent), nameof(placement), placement.ToDict());
        }

        //Fired when an ad request fails.
        private void _OnPlacementFailedEvent(GadsmePlacement placement)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementFailedEvent), nameof(placement), placement.ToDict());
        }

        //Fired when an ad is loaded and become visible for the first time, for the placement being visible.
        private void _OnPlacementVisibleEvent(GadsmePlacement placement)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementVisibleEvent), nameof(placement), placement.ToDict());
        }

        //Fired when an ad is viewable, for the placement being viewable.
        private void _OnPlacementViewableEvent(GadsmePlacement placement)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementViewableEvent), nameof(placement), placement.ToDict());
        }

        //Fired when an ad is clicked, for the placement where the click happened.
        private void _OnPlacementClickedEvent(GadsmePlacement placement)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementClickedEvent), nameof(placement), placement.ToDict());
            var p = placement.GetComponent<AbstractInPlayAd>();
            var placementName = p == null ? "Unknown" : p.ForPlacementName;
            this.Track(AdClickedParams.Inplay(placementName));
        }

        /// ---------------------------------------------------------------------------------------
        /// Ad content events 
        /// ---------------------------------------------------------------------------------------
        //Fired when an ad content is loaded.
        private void _OnAdContentLoadedEvent(GadsmeAdContentInfo info)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnAdContentLoadedEvent), nameof(info), info.ToDict());
        }

        //Fired when an ad content is loaded and become visible for the first time.
        private void _OnAdContentVisibleEvent(GadsmeAdContentInfo info)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnAdContentVisibleEvent), nameof(info), info.ToDict());
        }

        //Fired when an ad content is viewable.
        private void _OnAdContentViewableEvent(GadsmeAdContentInfo info)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnAdContentViewableEvent), nameof(info), info.ToDict());
        }

        //Fired when an ad content is clicked.
        private void _OnAdContentClickedEvent(GadsmeAdContentInfo info)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnAdContentClickedEvent), nameof(info), info.ToDict());
        }

        //Fired when an ad request fails.
        private void _OnAdContentFailedEvent(GadsmeAdContentInfo info)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnAdContentFailedEvent), nameof(info), info.ToDict());
        }
        
        private void _OnAdContentNoAdEvent(GadsmeAdContentInfo info)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnAdContentFailedEvent), nameof(info), info.ToDict());
        }

        /// ---------------------------------------------------------------------------------------
        /// Audio ad events 
        /// ---------------------------------------------------------------------------------------
        private void _OnLoadAudioAdEvent(GadsmeAudioAdInfo info)
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(_OnLoadAudioAdEvent), nameof(info), info.ToDict());
            this.Track(AdLoadParams.AudioAdLoadParams());
        }

        private void _OnCancelAudioAdEvent(GadsmeAudioAdInfo info, GadsmeCancelAudioAdReason reason)
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(_OnCancelAudioAdEvent), nameof(info), info.ToDict(), nameof(reason), reason);
            this.Track(AdLoadFailedParams.Audio(reason.ToString(), reason.ToString()));
            IsAudioAdAvailable = false;

            if (_playAudioOperation == null) return;
            _playAudioOperation.Fail($"_OnCancelAudioAdEvent: {reason}");
            _playAudioOperation = null;
        }

        private void _OnAudioAdReadyToPlayEvent(GadsmeAudioAdInfo info)
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(_OnAudioAdReadyToPlayEvent), nameof(info), info.ToDict());
            IsAudioAdAvailable = true;
            this.Track(AdLoadSucceededParams.Audio());
        }

        private void _OnPlayAudioAdEvent(GadsmeAudioAdInfo info)
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(_OnPlayAudioAdEvent), nameof(info), info.ToDict());
            IsAudioAdAvailable = false;
            this.Track(AdShowSucceededParams.Audio(_audioPlacement));
        }

        private void _OnAudioAdIncompletePlayThroughEvent(GadsmeAudioAdInfo info)
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(_OnAudioAdIncompletePlayThroughEvent), nameof(info), info.ToDict());
        }

        private void _OnFinishAudioAdEvent(GadsmeAudioAdInfo info, bool complete)
        {
            this.LogInfo(SharedLogTag.AudioAdsNGadsme, "f", nameof(_OnFinishAudioAdEvent), nameof(info), info.ToDict(), nameof(complete), complete);
            this.Track(AdClosedParams.Audio(_audioPlacement));
            IsAudioAdAvailable = false;
            _playAudioOperation?.Success();
            _playAudioOperation = null;
        }

        /// ---------------------------------------------------------------------------------------
        /// Impression events 
        /// ---------------------------------------------------------------------------------------
        private void _OnImpressionEvent(GadsmeImpressionData impressionData)
        {
            this.LogInfo(impressionData.adFormat.IsBanner() || impressionData.adFormat.IsVideo() ? SharedLogTag.InPlayAdsNGadsme : SharedLogTag.AudioAdsNGadsme, "keyword", "impression Gadsme", nameof(impressionData), impressionData.ToDict());
            var e = impressionData.ToTrackingImpression();
            this.Track(e);
            InPlayAdRegistry.RegisterPotentialProvider(InPlayAdProvider.Gadsme);
        }

        public string LogTag => SharedLogTag.Gadsme;
    }
}
#endif
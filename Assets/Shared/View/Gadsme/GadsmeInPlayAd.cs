#if GADSME
using Gadsme;
using Shared.Service.Gadsme.Internal;
#endif
using Shared.Utils;
using Shared.View.InPlayAds;
using UnityEngine;

namespace Shared.View.Gadsme
{
    [DisallowMultipleComponent]
    public abstract class GadsmeInPlayAd : AbstractInPlayAd, ISharedUtility
    {
        public override bool IsReady { get; protected set; }
        
#if GADSME
        private void Start()
        {
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "Name", GetType().Name);
            
            GadsmeEvents.PlacementLoadedEvent += _OnPlacementLoadedEvent;
            GadsmeEvents.PlacementVisibleEvent += _OnPlacementVisibleEvent;
            GadsmeEvents.PlacementViewableEvent += _OnPlacementViewableEvent;
            GadsmeEvents.PlacementClickedEvent += _OnPlacementClickedEvent;
            GadsmeEvents.PlacementFailedEvent += _OnPlacementFailedEvent;
            
            // GadsmeEvents.AdContentLoadedEvent += _OnAdContentLoadedEvent;
            // GadsmeEvents.AdContentVisibleEvent += _OnAdContentVisibleEvent;
            // GadsmeEvents.AdContentViewableEvent += _OnAdContentViewableEvent;
            // GadsmeEvents.AdContentClickedEvent += _OnAdContentClickedEvent;
            // GadsmeEvents.AdContentFailedEvent += _OnAdContentFailedEvent;
            
#if LOG_INFO
            var p = GetComponent<GadsmePlacement>();
            var isClickable = p != null && p.clickInteraction;
            var gadsmeRawImage = gameObject.GetComponent<GadsmeRawImage>();
            var raycastTarget = gadsmeRawImage != null && gadsmeRawImage.raycastTarget;
            if (gadsmeRawImage == null)
                this.LogError(SharedLogTag.InPlayAdsNGadsme, nameof(gadsmeRawImage), "null");
            this.LogInfo("clazz", "GadsmeInPlayAd", nameof(isClickable), isClickable, nameof(raycastTarget), raycastTarget);
#endif
        }

        /// ---------------------------------------------------------------------------------------
        /// Placement events 
        /// ---------------------------------------------------------------------------------------
        //Fired when an ad is loaded, for every placement using that ad.
        private void _OnPlacementLoadedEvent(GadsmePlacement placement)
        {
            if (gameObject != placement.gameObject) return;
            // var gadsmeRawImage = gameObject.GetComponent<GadsmeRawImage>();
            IsReady = true;// gadsmeRawImage.enabled && gadsmeRawImage.texture != null;
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementLoadedEvent), nameof(ForPlacementName), ForPlacementName, "Name", GetType().Name, "class", nameof(GadsmeInPlayAd), nameof(IsReady), IsReady);   
        }

        //Fired when an ad request fails.
        private void _OnPlacementFailedEvent(GadsmePlacement placement)
        {
            if (gameObject != placement.gameObject) return;
            IsReady = false;
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementFailedEvent), nameof(ForPlacementName), ForPlacementName, "Name", GetType().Name, "class", nameof(GadsmeInPlayAd), nameof(IsReady), IsReady);
            
        }

        //Fired when an ad is loaded and become visible for the first time, for the placement being visible.
        private void _OnPlacementVisibleEvent(GadsmePlacement placement)
        {
            if (gameObject != placement.gameObject) return;
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementVisibleEvent), nameof(ForPlacementName), ForPlacementName, "Name", GetType().Name, "class", nameof(GadsmeInPlayAd));
        }

        //Fired when an ad is viewable, for the placement being viewable.
        private void _OnPlacementViewableEvent(GadsmePlacement placement)
        {
            if (gameObject != placement.gameObject) return;
            // var gadsmeRawImage = gameObject.GetComponent<GadsmeRawImage>();
            // IsReady = gadsmeRawImage.enabled && gadsmeRawImage.texture != null;
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementViewableEvent), nameof(ForPlacementName), ForPlacementName, "Name", GetType().Name, "class", nameof(GadsmeInPlayAd), nameof(IsReady), IsReady);
        }

        //Fired when an ad is clicked, for the placement where the click happened.
        private void _OnPlacementClickedEvent(GadsmePlacement placement)
        {
            if (gameObject != placement.gameObject) return;
            this.LogInfo(SharedLogTag.InPlayAdsNGadsme, "f", nameof(_OnPlacementClickedEvent), nameof(ForPlacementName), ForPlacementName, "Name", GetType().Name, "class", nameof(GadsmeInPlayAd));
        }
#endif
    }
}
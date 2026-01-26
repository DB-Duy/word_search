#if ADVERTY_5
using Adverty5.AdPlacements;
#endif
using System;
using Shared.Utilities;
using Shared.Utils;
using Shared.View.InPlayAds;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.View.Adverty5
{
    /// <summary>
    /// https://adverty.com/sdk-5-upgrade-guide/
    /// </summary>
    public abstract class Adverty5InPlayAd : AbstractInPlayAd, ISharedUtility, ISharedLogTag
    {

        public override bool IsReady { get; protected set; }
        private RawImage _rawImage;
#if ADVERTY_5
        private AdPlacement _placement;
        private AdPlacement Placement => _placement ??= GetComponent<AdPlacement>();
#endif

        private void Start()
        {
            _rawImage = GetComponent<RawImage>();
#if ADVERTY_5
            Placement.AdPlacementRegisteredEvent += _OnAdPlacementRegisteredEvent;
            Placement.AdPlacementFailedToRegisterEvent += _OnAdPlacementFailedToRegisterEvent;
            Placement.AdPlacementActivatedEvent += _OnAdPlacementActivatedEvent;
            
#endif
        }

#if ADVERTY_5
        private void Update()
        {
            if (Time.frameCount % 5 != 0) return;
            IsReady = Placement.IsActive && _rawImage.enabled;
        }
#endif
        
#if ADVERTY_5
        private void _OnAdPlacementActivatedEvent(AdPlacement placement)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, nameof(ForPlacementName), ForPlacementName, "class", nameof(Adverty5InPlayAd), nameof(placement), placement.ToInfoString());
        }

        private void _OnAdPlacementFailedToRegisterEvent(AdPlacement placement)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, nameof(ForPlacementName), ForPlacementName, "class", nameof(Adverty5InPlayAd), nameof(placement), placement.ToInfoString());
        }

        private void _OnAdPlacementRegisteredEvent(AdPlacement placement)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, nameof(ForPlacementName), ForPlacementName, "class", nameof(Adverty5InPlayAd), nameof(placement), placement.ToInfoString());
        }
#endif

        public string LogTag => SharedLogTag.InPlayAdsNAdverty;
    }
}
#if BYTE_BREW
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Templates;
using UnityEngine;

namespace Shared.Tracking.Internal.Handler.ByteBrewHandlers
{
    /// <summary>
    /// https://docs.bytebrew.io/sdk/unity#AdTrackingEvent
    /// </summary>
    public class ByteBrewAdRevenueTrackingHandler : BaseTrackingHandler
    {
        public override void Handle(ITrackingEvent e)
        {
            if (e is AdRevenueEvent adEvent && !Application.isEditor)
            {
                var adType = TrackingUtils.ConvertToByteBrewAdType(adEvent.AdFormat);
                ByteBrewSDK.ByteBrew.TrackAdEvent(adType, adEvent.AdSource, adEvent.AdUnitName, "in-game", adEvent.Value);    
            }
        }
    }
}
#endif
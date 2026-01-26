#if ADJUST
using Shared.Core.IoC;
using Shared.Service.SharedAdjust;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.Adjust
{
    [Component]
    public class AdjustAdRevenueTrackingHandler : ITrackingHandler, ISharedUtility
    {
        public void Handle(ITrackingEvent e)
        {
            if (e is not AdImpressionEvent ee) return;
            var adjustAdRevenue = ee.ToAdjustAdRevenue();
            this.LogInfo(SharedLogTag.TrackingNAdjustNAds_, nameof(adjustAdRevenue), adjustAdRevenue);
            AdjustSdk.Adjust.TrackAdRevenue(adjustAdRevenue);
        }
    }
}
#endif
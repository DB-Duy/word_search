#if ADJUST && GOOGLE_PLAY && IAP
using Shared.Core.IoC;
using Shared.Service.SharedAdjust;
using Shared.Service.Tracking.TrackingEvent.IAP;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.Adjust
{
    [Component]
    public class AdjustGooglePlayInAppTrackingHandler : ITrackingHandler, ISharedUtility
    {
        /// <summary>
        /// https://github.com/adjust/unity_sdk?tab=readme-ov-file#et-revenue
        /// </summary>
        public void Handle(ITrackingEvent e)
        {
            if (e is not GooglePlayInAppParams ee) return;
            var adjustEvent = ee.ToAdjustEvent();
            this.LogInfo(SharedLogTag.TrackingNAdjustNIap_, nameof(adjustEvent), adjustEvent);
            AdjustSdk.Adjust.TrackEvent(adjustEvent);
        }
    }
}
#endif
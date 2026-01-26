#if ADJUST && GOOGLE_PLAY && IAP
using Shared.Core.IoC;
using Shared.Service.SharedAdjust;
using Shared.Service.Tracking.TrackingEvent.IAP;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.Adjust
{
    /// <summary>
    /// https://github.com/adjust/unity_sdk?tab=readme-ov-file#ad-subscriptions
    /// 
    /// </summary>
    [Component]
    public class AdjustGooglePlaySubscriptionTrackingHandler : ITrackingHandler, ISharedUtility
    {
        public void Handle(ITrackingEvent e)
        {
            if (e is not GooglePlaySubscriptionParams ee) return;
            this.LogInfo(SharedLogTag.TrackingNAdjustNIap_, nameof(e.EventName), e.EventName);
            AdjustSdk.Adjust.TrackPlayStoreSubscription(ee.ToAdjustPlayStoreSubscription());
        }
    }
}
#endif
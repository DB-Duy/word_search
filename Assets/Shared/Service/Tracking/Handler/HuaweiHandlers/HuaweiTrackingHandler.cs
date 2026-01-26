#if HUAWEI
using HmsPlugin;
using Shared.Tracking.Models.Templates;
using UnityEngine;

namespace Shared.Tracking.Internal.Handler
{
    public class HuaweiTrackingHandler : ITrackingHandler
    {
        public void Handle(ITrackingEvent e)
        {
            if (e is IHuaweiCustomEvent ee && !Application.isEditor)
            {
                HMSAnalyticsKitManager.Instance.SendEventWithBundle(e.EventName, ee.ToHuaweiParams());
            }
        }
    }
}
#endif
#if APPSTORE && DEV_TO_DEV
using DevToDev.Subscriptions;
using Shared.Tracking.Models.Templates;

namespace Shared.Tracking.Internal.Handler
{
    public class AppStoreSubscriptionD2DHandler : ITrackingHandler
    {
        public void Handle(ITrackingEvent e)
        {
            if (e is IDtdAppStoreSubscriptionEvent ee)
            {
                DTDSubscriptions.Payment(ee.ToDtdAppStoreSubscription());
            }
        }
    }
}
#endif
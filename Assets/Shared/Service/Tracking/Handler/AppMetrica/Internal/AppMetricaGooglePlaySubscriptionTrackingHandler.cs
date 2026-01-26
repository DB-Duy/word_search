#if APPMETRICA && GOOGLE_PLAY

using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Service.AppMetrica;
using Shared.Service.Tracking.TrackingEvent.IAP;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.AppMetrica.Internal
{
    [Component]
    public class AppMetricaGooglePlaySubscriptionTrackingHandler : IHandler<ITrackingEvent>, IAppMetricaTrackingHandler, ISharedUtility
    {
        public void Handle(ITrackingEvent e)
        {
            if (e is not GooglePlaySubscriptionParams ee) return;
            var revenue = ee.ToRevenue();
            this.LogInfo(SharedLogTag.TrackingNAppMetrica, nameof(revenue), revenue);
            Io.AppMetrica.AppMetrica.ReportRevenue(revenue);
        }
    }
}

#endif
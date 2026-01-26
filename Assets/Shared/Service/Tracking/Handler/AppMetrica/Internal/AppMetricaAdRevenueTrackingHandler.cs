#if APPMETRICA

using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Service.AppMetrica;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.AppMetrica.Internal
{
    [Component]
    public class AppMetricaAdRevenueTrackingHandler : IHandler<ITrackingEvent>, IAppMetricaTrackingHandler, ISharedUtility
    {
        public void Handle(ITrackingEvent e)
        {
            if (e is not AdImpressionEvent ee) return;
            var adRevenue = ee.ToAdRevenue();
            this.LogInfo(SharedLogTag.TrackingNAppMetrica, nameof(adRevenue), adRevenue);
            Io.AppMetrica.AppMetrica.ReportAdRevenue(adRevenue);
        }
    }
}
#endif
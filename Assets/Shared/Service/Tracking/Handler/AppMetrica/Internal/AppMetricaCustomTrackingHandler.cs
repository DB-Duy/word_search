#if APPMETRICA
using Newtonsoft.Json;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.AppMetrica.Internal
{
    [Component]
    public class AppMetricaCustomTrackingHandler : IHandler<ITrackingEvent>, IAppMetricaTrackingHandler, ISharedUtility
    {
        public void Handle(ITrackingEvent e)
        {
            if (e is not IConvertableEvent ee) return;
            this.LogInfo(SharedLogTag.TrackingNAppMetrica, nameof(e.EventName), e.EventName);
            var p = JsonConvert.SerializeObject(ee.ToConvertableEvent());
            Io.AppMetrica.AppMetrica.ReportEvent(((ITrackingEvent)ee).EventName, p);
        }
    }
}

#endif
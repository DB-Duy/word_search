# if ADJUST && !APPMETRICA
// using System.Collections;
// using AdjustSdk;
// using Shared.Core.IoC;
// using Shared.Service.Tracking;
// using Shared.Utils;
//
// namespace Shared.Service.SharedAdjust.Handler
// {
//     [Component]
//     public class AdjustAttributionHandler : IAdjustAttributionHandler
//     {
//         private const string KEY = "AdjustTrackerToken";
//         public void Handle(AdjustAttribution t)
//         {
//             this.LogInfo(nameof(t), t);
//             this.SetUserProperty(KEY, t.TrackerToken);
//         }
//     }
// }
#endif

# if ADJUST && APPMETRICA
using System.Collections;
using AdjustSdk;
using Shared.Core.IoC;
using Shared.Service.AppMetrica;
using Shared.Service.SharedCoroutine;
using Shared.Service.Tracking;
using Shared.Utils;
using Zenject;

namespace Shared.Service.SharedAdjust.Handler
{
    [Component]
    public class AdjustAttributionHandler : IAdjustAttributionHandler, ISharedUtility
    {
        // private const string KEY = "AdjustTrackerToken";
        [Inject] private IAppMetricaService _appMetricaService;
        public void Handle(AdjustAttribution t)
        {
            this.LogInfo(SharedLogTag.AdjustNAppMetrica, nameof(t), t);
            this.StartSharedCoroutine(_WaitAppMetricaInitAndHandle(t));
        }

        private IEnumerator _WaitAppMetricaInitAndHandle(AdjustAttribution t)
        {
            while (!_appMetricaService.IsInitialized) yield return null;
            this.LogInfo(SharedLogTag.AdjustNAppMetrica, nameof(t), t);
            // this.SetUserProperty(KEY, t.TrackerToken);
            this.LogInfo(SharedLogTag.AdjustNAppMetrica, "call", "Io.AppMetrica.AppMetrica.ReportExternalAttribution(Io.AppMetrica.ExternalAttributions.Adjust(t));");
            Io.AppMetrica.AppMetrica.ReportExternalAttribution(Io.AppMetrica.ExternalAttributions.Adjust(t));
        }
    }
}
#endif
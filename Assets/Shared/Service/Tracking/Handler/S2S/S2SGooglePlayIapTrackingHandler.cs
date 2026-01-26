#if GOOGLE_PLAY && S2S
using Shared.Core.IoC;
using Shared.Service.S2S;
using Shared.Service.S2S.Request;
using Shared.Service.S2S.Response;
using Shared.Tracking.Models.IAP;
using Shared.Tracking.Templates;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Tracking.Handler.S2S
{
    [Component]
    public class S2SGooglePlayIapTrackingHandler : ITrackingHandler, IIoC, ISharedUtility
    {
        [Inject] private IS2SService _s2SService;

        public void Handle(ITrackingEvent e)
        {
            if (e is not IGooglePlayPurchasingParams ee) return;
            
            this.LogInfo(SharedLogTag.TrackingNS2S, nameof(e.EventName), e.EventName);
            var request = new IapVerificationRequest
            {
                Receipt = ee.Receipt,
                UserData = _s2SService.GetUserData()
            };
            _s2SService.ExecutePost<IapVerificationResponse>(request);
        }
    }
}
#endif
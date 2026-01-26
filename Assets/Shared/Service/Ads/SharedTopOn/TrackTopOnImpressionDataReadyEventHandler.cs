#if TOPON
using AnyThinkAds.Api;
using Shared.Tracking.Models.Ads;
using Shared.Utils;

namespace Shared.Ads.SharedTopOn
{
    public class TrackTopOnImpressionDataReadyEventHandler : ITopOnImpressionDataReadyEventHandler
    {
        private const string TAG = "TrackTopOnImpressionDataReadyEventHandler";
        
        public void Handle(ATAdEventArgs entity)
        {
            SharedLogger.Log($"{TAG}->Handle");
            var atCallbackInfo = entity.callbackInfo;
            var adImpressionParams = new AdRevenueEvent(
                adPlatform: "TopOn",
                adSource: atCallbackInfo.network_firm_id.ToString(),
                adUnitName: atCallbackInfo.adsource_id,
                adFormat: atCallbackInfo.adunit_format,
                currency: "USD",
                value: atCallbackInfo.publisher_revenue);
            SharedCore.Instance.TrackingService.Track(adImpressionParams);
        }
    }
}
#endif
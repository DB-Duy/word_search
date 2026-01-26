#if APPMETRICA

using System.Collections.Generic;
using Io.AppMetrica;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Service.Tracking.TrackingEvent.IAP;
using Shared.Tracking.Models.Ads;
using Shared.Utils;

namespace Shared.Service.AppMetrica
{
    public static class AppMetricaExtensions
    {
        public static AdRevenue ToAdRevenue(this AdImpressionEvent ee)
        {
            return new AdRevenue(ee.Value, ee.Currency)
            {
                AdNetwork = ee.AdSource,
                AdUnitName = ee.AdUnitName,
                AdPlacementName = ee.AdFormat,
                AdType = ee.AdFormat.ToAppMetricaAdType(),
                Payload = new Dictionary<string, string>(ee.ExParams.ToDictStringString())
                {
                    { "ad_format", ee.AdFormat }
                } 
            };
        }
#if GOOGLE_PLAY
        public static Revenue ToRevenue(this GooglePlaySubscriptionParams ee)
        {
            return new Revenue(ee.PriceAmountMicros, ee.PriceCurrencyCode)
            {
                ProductID = ee.ProductId,
                ReceiptValue = new Revenue.Receipt
                {
                    Data = ee.Json,
                    Signature = ee.Signature,
                }
            };
        }
#endif
        
        private static readonly Dictionary<string, AdType> AppMetricaAdTypeMap = new()
        {
            {"banner", AdType.Banner},
            {"interstitial", AdType.Interstitial},
            {"rewarded_video", AdType.Rewarded}
        };
        
        public static AdType ToAppMetricaAdType(this string adFormat) => AppMetricaAdTypeMap.GetValueOrDefault(adFormat, AdType.Other);
        
        // Io.AppMetrica.AppMetrica.ReportExternalAttribution(Io.AppMetrica.ExternalAttributions.Adjust(SharedCore.Instance.AdjustService.AdjustAttribution))
    }
}
#endif
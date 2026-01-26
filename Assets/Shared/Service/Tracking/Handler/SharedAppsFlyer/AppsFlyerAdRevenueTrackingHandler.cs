#if APPS_FLYER
using System.Collections.Generic;
using AppsFlyerSDK;
using Shared.Core.IoC;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.SharedAppsFlyer
{
    /// <summary>
    /// https://dev.appsflyer.com/hc/docs/ad-revenue-unity
    /// </summary>
    [Component]
    public class AppsFlyerAdRevenueTrackingHandler : ITrackingHandler, ISharedUtility
    {
        public void Handle(ITrackingEvent e)
        {
            if (e is not AdImpressionEvent ee) return;
            var additionalParams = new Dictionary<string, string>
            {
                { AdRevenueScheme.COUNTRY, ee.Country },

                { AdRevenueScheme.AD_UNIT, ee.AdUnitId },
                { AdRevenueScheme.AD_TYPE, ee.AdFormat },
                { AdRevenueScheme.PLACEMENT, ee.Placement },

                { "auction_id", ee.AuctionId },
                { "creative_id", ee.CreativeId },
                { "mediation_ad_unit_name", ee.MediationAdUnitName },
                { "ab", ee.Ab },
                { "segment_name", ee.SegmentName },
                { "instance_name", ee.InstanceName },
                { "instance_id", ee.InstanceId },
                { "precision", ee.Precision },
                { "encrypted_cpm", ee.EncryptedCpm },
                { "conversion_value", ee.ConversionValue + "" }
            };
            var logRevenue = new AFAdRevenueData(ee.AdSource, MediationNetwork.IronSource, ee.Currency, ee.Value);
            this.LogInfo(SharedLogTag.TrackingNAppsFlyerNAds_, nameof(logRevenue), logRevenue, nameof(additionalParams), additionalParams);
            AppsFlyer.logAdRevenue(logRevenue, additionalParams);
        }
    }
}
#endif
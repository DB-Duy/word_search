#if ADJUST

using AdjustSdk;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Service.Tracking.TrackingEvent.IAP;
using Shared.Tracking.Models.Ads;
using Shared.Tracking.Models.IAP;
using Shared.Utils;

namespace Shared.Service.SharedAdjust
{
    public static class AdjustExtensions
    {
        private const string ClassName = "AdjustExtensions";

        public static void AddGranularOptionWithLog(this AdjustThirdPartySharing sharing, string partnerName, string key, string value)
        {
            SharedLogger.LogInfoCustom(SharedLogTag.UmpNAdjust_, ClassName, "AddGranularOptionWithLog", nameof(partnerName), partnerName, nameof(key), key, nameof(value), value);
            sharing.AddGranularOption(partnerName, key, value);
        }

#if IAP
        public static AdjustPlayStoreSubscription ToAdjustPlayStoreSubscription(this GooglePlaySubscriptionParams ee)
        {
            return new AdjustPlayStoreSubscription(
                price: ee.PriceAmountMicros.ToString(),
                currency: ee.PriceCurrencyCode,
                productId: ee.ProductId,
                orderId: ee.OrderId,
                signature: ee.Signature,
                purchaseToken: ee.PurchaseToken)
            {
                PurchaseTime = ee.PurchaseTime
            };
        }

        public static AdjustEvent ToAdjustEvent(this GooglePlayInAppParams ee)
        {
            var config = IoCExtensions.Resolve<IConfig>();
            var adjustEvent = new AdjustEvent(config.AdjustRevenueEventToken)
            {
                PurchaseToken = ee.TransactionId
            };
            adjustEvent.SetRevenue(ee.Amount, ee.PriceCurrencyCode);
            return adjustEvent;
        }
        
        public static AdjustAppStoreSubscription ToAdjustAppStoreSubscription(this AppStoreSubscriptionParams ee)
        {
            return new AdjustAppStoreSubscription(
                price: $"{ee.LocalizedPrice}",
                currency: ee.IsoCurrencyCode,
                transactionId: ee.TransactionID);
        }
        
        public static AdjustEvent ToAdjustEvent(this AppStoreInAppParams ee)
        {
            var config = IoCExtensions.Resolve<IConfig>();
            var adjustEvent = new AdjustEvent(config.AdjustRevenueEventToken)
            {
                PurchaseToken = ee.TransactionID
            };
            adjustEvent.SetRevenue((double) ee.LocalizedPrice, ee.IsoCurrencyCode);
            return adjustEvent;
        }
#endif

        /// <summary>
        /// public const string AdjustAdRevenueSourceAppLovinMAX = "applovin_max_sdk";
        /// public const string AdjustAdRevenueSourceMopub = "mopub";
        /// public const string AdjustAdRevenueSourceAdMob = "admob_sdk";
        /// public const string AdjustAdRevenueSourceIronSource = "ironsource_sdk";
        /// public const string AdjustAdRevenueSourceAdmost = "admost_sdk";
        /// public const string AdjustAdRevenueSourceUnity = "unity_sdk";
        /// public const string AdjustAdRevenueSourceHeliumChartboost = "helium_chartboost_sdk";
        /// public const string AdjustAdRevenueSourcePublisher = "publisher_sdk";
        /// public const string AdjustAdRevenueSourceTopOn = "topon_sdk";
        /// public const string AdjustAdRevenueSourceAdx = "adx_sdk";
        /// public const string AdjustAdRevenueTradPlus = "tradplus_sdk";
        /// 
        /// </summary>
        public static AdjustAdRevenue ToAdjustAdRevenue(this AdImpressionEvent ee)
        {
            var adjustAdRevenue = new AdjustAdRevenue("ironsource_sdk")
            {
                AdRevenueNetwork = ee.AdSource,
                AdRevenueUnit = ee.AdFormat,
                AdRevenuePlacement = ee.AdUnitName
            };
            adjustAdRevenue.SetRevenue(ee.Value, ee.Currency);
            return adjustAdRevenue;
        }
    }
}
#endif
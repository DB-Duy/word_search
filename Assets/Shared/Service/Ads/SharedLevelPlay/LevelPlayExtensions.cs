#if LEVEL_PLAY
using System.Collections.Generic;
using Shared.Entity.Ump;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;
using Shared.Utils;

namespace Shared.Service.Ads.SharedLevelPlay
{
    public static class LevelPlayExtensions
    {
        private const string Tag = "LevelPlayExtensions";
        
        public static AdImpressionEvent ToAdRevenueEvent(this IronSourceImpressionData data)
        {
            if (data == null) return null;
            double revenue = 0;
            if (data.revenue != null) 
            {
                try
                {
                    revenue = (double) data.revenue;
                    // Loại bỏ culture bằng cách cast trực tiếp, không dùng parseString.
                    // double.Parse(entity.revenue.ToString(), CultureInfo.InvariantCulture);
                }
                catch (System.Exception e)
                {
                    SharedLogger.LogJsonError(SharedLogTag.AdNLevelPlay, $"{Tag}->ToAdRevenueEvent", nameof(data.revenue), data.revenue);
                    revenue = 0;
                }
            }
            // data.adNetwork
            var are = new AdImpressionEvent(
                adPlatform: "ironSource",
                adSource: data.adNetwork,
                adUnitName: data.instanceName,
                adFormat: data.adUnit,
                currency: "USD",
                value: revenue)
            {
                Country = data.country,
                AdUnitId = data.mediationAdUnitId,
                Placement = data.placement,
                
                AuctionId = data.auctionId,
                CreativeId = data.CreativeId,
                MediationAdUnitName = data.mediationAdUnitName,
                MediationAdUnitId = data.mediationAdUnitId,
                Ab = data.ab,
                SegmentName = data.segmentName,
                InstanceName = data.instanceName,
                InstanceId = data.instanceId,
                Precision = data.precision,
                EncryptedCpm = data.encryptedCPM,
                ConversionValue = data.conversionValue + ""
            };

            return are;
        }
        
        public static void setMetaDataWithLog(this IronSource agent, string key, string value)
        {
            agent.setMetaData(key, value);
            SharedLogger.LogJson(SharedLogTag.Ump, $"{Tag}->setMetaDataWithLog", nameof(key), key, nameof(value), value);
        }
        
        private static readonly Dictionary<string, string> UsPrivacyStringDoNotSellMap = new()
        {
            {UsPrivacyValue.Const1YynOn, "true"},
            {UsPrivacyValue.Const1YnnOff, "false"},
        };

        public static string ToDoNotSellString(this string usPrivacyString)
        {
            return !UsPrivacyStringDoNotSellMap.ContainsKey(usPrivacyString) ? "false" : UsPrivacyStringDoNotSellMap[usPrivacyString];
        }
    }
}
#endif
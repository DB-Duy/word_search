#if GADSME
using System.Collections.Generic;
using Gadsme;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;

namespace Shared.Service.Gadsme.Internal
{
    public static class GadsmeExtensions
    {
        public static Dictionary<string, object> ToDict(this GadsmeAudioAdInfo info, string extraLog = null)
        {
            return new Dictionary<string, object>
            {
                { "info", info.ToString() },
                { "info.IsRewardedAd", info.IsRewardedAd },
                { "info.CompanionSize", info.CompanionSize },
                { "extraLog", extraLog }
            };
        }

        public static Dictionary<string, object> ToDict(this GadsmeAdContentInfo info)
        {
            return new Dictionary<string, object>
            {
                { "adFormat", info.adFormat.GetName() },
                { "adChannelNumber", info.adChannelNumber },
                { "lineItemType", info.lineItemType },
            };
        }

        // public static Dictionary<string, object> ToDict(this GadsmePlacement placement)
        // {
        //     return new()
        //     {
        //         { "placementId", placement.placementId },
        //         { "adFormat", placement.adFormat.ToString() },
        //     };
        // }

        public static Dictionary<string, object> ToDict(this GadsmePlacement placement)
        {
            return new Dictionary<string, object>()
            {
                { "placementName", placement.gameObject.name },
                { "placementId", placement.placementId },
                { "adFormat", placement.adFormat.ToString() }
            };
        }

        public static Dictionary<string, object> ToDict(this GadsmeImpressionData impressionData)
        {
            return new Dictionary<string, object>
            {
                { "placementId", impressionData.placementId },
                { "countryCode", impressionData.countryCode },
                { "gameId", impressionData.gameId },
                { "adFormatId", impressionData.adFormat.GetId() },
                { "adFormatName", impressionData.adFormat.GetName() },
                { "lineItemType", impressionData.lineItemType },
                { "netRevenue", impressionData.netRevenue },
                { "platform", impressionData.platform },
                { "currency", impressionData.currency }
            };
        }

        public static AdImpressionEvent ToTrackingImpression(this GadsmeImpressionData impressionData)
        {
            if (impressionData.adFormat.IsAudio()) return impressionData.ToAudioImpression();
            if (impressionData.adFormat.IsBanner()) return impressionData.ToBannerImpression();
            if (impressionData.adFormat.IsVideo()) return impressionData.ToVideoImpression();
            return null;
        }

        public static AdImpressionEvent ToAudioImpression(this GadsmeImpressionData impressionData)
            => CreateAdRevenueEvent("Audio", "audio", impressionData);

        public static AdImpressionEvent ToBannerImpression(this GadsmeImpressionData impressionData)
            => CreateAdRevenueEvent("Banner", "in_play", impressionData);

        public static AdImpressionEvent ToVideoImpression(this GadsmeImpressionData impressionData)
            => CreateAdRevenueEvent("Video", "in_play", impressionData);

        private static AdImpressionEvent CreateAdRevenueEvent(string adUnit, string adFormat, GadsmeImpressionData impressionData)
        {
            return new AdImpressionEvent(
                adPlatform: "gadsme",
                adSource: "gadsme",
                adUnitName: adUnit,
                adFormat: adFormat,
                currency: "USD",
                value: impressionData.netRevenue);
        }

        // public static Dictionary<string, object> ToDict(this GadsmeReadyEventArgs me)
        // {
        //     return new Dictionary<string, object>()
        //     {
        //         { "ip", me.Ip },
        //         { "countryCode", me.CountryCode },
        //         { "gdprApplies", me.GdprApplies }
        //     };
        // }

        public static Dictionary<string, object> ToDict(this GadsmeAdFormat me)
        {
            if (me == null)
                return new Dictionary<string, object>()
                {
                    { "me", "null" }
                };

            return new Dictionary<string, object>()
            {
                { "id", me.GetId() },
                { "name", me.GetName() },
                { "ratio", me.GetRatio() },
                { "isAudio", me.IsAudio() },
                { "isBanner", me.IsBanner() },
                { "isVideo", me.IsVideo() },
                { "width", me.GetWidth() },
                { "height", me.GetHeight() },
            };
        }
    }
}
#endif
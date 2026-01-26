using Shared.Entity.Ads;
using Shared.Entity.Config;

namespace Shared.Service.Aps
{
    public static class ApsExtensions
    {
        /// public static ApsBannerConfig NewPhoneInstance(string id) =&gt; new(320, 50, id);
        /// public static ApsBannerConfig NewTabletInstance(string id) =&gt; new(728, 90, id);
        /// var apsBannerConfig = isTablet ? ApsBannerConfig.NewTabletInstance(config.ApsBanner_728x90) : ApsBannerConfig.NewPhoneInstance(config.ApsBanner_320x50);
        public static ApsBannerConfig ToApsBannerConfig(this IConfig config)
            => ToApsBannerConfigOrNull(config.ApsBanner_728x90, config.ApsBanner_320x50);

        public static ApsBannerConfig ToApsBannerConfig(this MultipleBannerAdsConfig.AdUnit adUnit)
            => ToApsBannerConfigOrNull(adUnit.ApsIdTablet, adUnit.ApsIdPhone);

        private static ApsBannerConfig ToApsBannerConfigOrNull(string tablet, string phone)
        {
            if (SharedUtilities.IsTabletDevice)
                return string.IsNullOrEmpty(tablet) ? null : ApsBannerConfig.NewTabletInstance(tablet);
            return string.IsNullOrEmpty(phone) ? null : ApsBannerConfig.NewPhoneInstance(phone);
        }

        public static ApsInterstitialConfig ToApsInterstitialConfig(this IConfig config)
            => string.IsNullOrEmpty(config.ApsInterstitialImage) && string.IsNullOrEmpty(config.ApsInterstitialVideo) 
                ? null : new ApsInterstitialConfig(config.ApsInterstitialImage, config.ApsInterstitialVideo);

        public static ApsInterstitialConfig ToApsInterstitialConfig(this MultipleInterstitialAdsConfig.AdUnit adUnit)
            => string.IsNullOrEmpty(adUnit.ApsIdStatic) && string.IsNullOrEmpty(adUnit.ApsIdVideo) 
                ? null : new ApsInterstitialConfig(adUnit.ApsIdStatic, adUnit.ApsIdVideo);
#if APS
        public static string ToInfoString(this AmazonAds.AdResponse adResponse)
        {
            return StringUtils.ToJsonString("pricePoint", adResponse.GetPricePoint(), "width", adResponse.GetWidth(), "height", adResponse.GetHeight(), "bidInfo", adResponse.GetBidInfo());
        }
#endif
    }
}

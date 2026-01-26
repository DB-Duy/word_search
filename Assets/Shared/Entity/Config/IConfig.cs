namespace Shared.Entity.Config
{
    public interface IConfig
    {
        string BundleId { get; }
        string PrivacyUrl { get; }
        string TermOfUseUrl { get; }
        string StoreUrl { get; }

        string AppStoreId { get; }

        string IronSourceAppKey { get; }
        string IronSourceBannerId { get; }
        string IronSourceInterstitialId { get; }
        string IronSourceRewardedId { get; }
        string IronSourceMrecId { get; }
        string AdmobAppId { get; }
        float InterstitialFakeLoadingTime { get; }
        
        string TopOnAppKey { get; }
        string TopOnAppId { get; }
        string TopOnRewardedId { get; }
        string TopOnInterstitialId { get; }
        string TopOnBannerId { get; }

        string ApsAppId { get; }
        string ApsBanner_728x90 { get; }
        string ApsBanner_320x50 { get; }
        string ApsInterstitialVideo { get; }
        string ApsInterstitialImage { get; }
        string ApsRewardedVideo { get; }
        string ApsMrec { get; }

        string AdvertyApiKey { get; }
        string Adverty5ApiKey { get; }
        string DevtodevAppKey { get; }
        string AdjustAppToken { get; }
        string AdjustRevenueEventToken { get; }
        string IapBase64RsaPublicKey { get; }
        string OdeeoApiKey { get; }
        string OdeeoAppKey { get; }
        string OdeeoIconId { get; }
        string OdeeoRewardedIconId { get; }
        string AudiomobId { get; }
        string GadsmeId { get; }
        string FacebookId { get; }
        string FacebookClientToken { get; }
        string FacebookRewardedId { get; }
        string FacebookInterstitialId { get; }
        string FacebookBannerId { get; }
        string AppmetricaId { get; }

        bool S2SIapVerifyEnable { get; }
        string S2SIapVerifyEnableRaw { get; }
        string S2SIapVerifyUrl { get; }
        string S2SIapVerifyKey { get; }

        int LoadingTaskTimeout { get; }
        int TargetFrameRate { get; }

        string TrackingEventStaticPropertyNames { get; }

        string AppsFlyerDevKey { get; }
        int FixedDpi { get; }
    }
}
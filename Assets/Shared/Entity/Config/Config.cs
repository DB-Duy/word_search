using Newtonsoft.Json;

namespace Shared.Entity.Config
{
    [System.Serializable]
    public class Config : IConfig
    {
        [JsonProperty("store_url")] public string StoreUrl { get; private set; }
        [JsonProperty("appstore_id")] public string AppStoreId  { get; private set; }
        [JsonProperty("bundle_id")] public string BundleId { get; private set; }
        [JsonProperty("iap_base64_rsa_public_key")] public string IapBase64RsaPublicKey { get; private set; }

        [JsonProperty("privacy_url")] public string PrivacyUrl { get; private set; }
        [JsonProperty("tos_url")] public string TermOfUseUrl { get; private set; }

        [JsonProperty("topon_app_key")] public string TopOnAppKey { get; private set; }
        [JsonProperty("topon_app_id")] public string TopOnAppId { get; private set; }
        [JsonProperty("topon_rewarded_id")] public string TopOnRewardedId { get; private set; }
        [JsonProperty("topon_interstitial_id")] public string TopOnInterstitialId { get; private set; }
        [JsonProperty("topon_banner_id")] public string TopOnBannerId { get; private set; }

        [JsonProperty("is_app_key")] public string IronSourceAppKey  { get; private set; }
        [JsonProperty("is_banner_id")] public string IronSourceBannerId { get; private set; }
        [JsonProperty("is_interstitial_id")] public string IronSourceInterstitialId { get; private set; }
        [JsonProperty("is_rewarded_id")] public string IronSourceRewardedId { get; private set; }
        [JsonProperty("is_mrec_id")] public string IronSourceMrecId { get; private set; }
        [JsonProperty("admob_app_id")] public string AdmobAppId { get; private set; }
        [JsonProperty("interstitial_fake_loading_time")] private string _interstitialFakeLoadingTime;
        [JsonIgnore] private float? _interstitialFakeLoadingTimeFloatValue;
        [JsonIgnore] public float InterstitialFakeLoadingTime => _interstitialFakeLoadingTimeFloatValue ??= string.IsNullOrEmpty(_interstitialFakeLoadingTime) ? 1.5f : float.Parse(_interstitialFakeLoadingTime, System.Globalization.CultureInfo.InvariantCulture);
        
        [JsonProperty("aps_app_id")] public string ApsAppId { get; private set; }
        [JsonProperty("aps_banner_728x90")] public string ApsBanner_728x90 { get; private set; }
        [JsonProperty("aps_banner_320x50")] public string ApsBanner_320x50 { get; private set; }
        [JsonProperty("aps_interstitial_video")] public string ApsInterstitialVideo { get; private set; }
        [JsonProperty("aps_interstitial_image")] public string ApsInterstitialImage { get; private set; }
        [JsonProperty("aps_rewarded_video")] public string ApsRewardedVideo { get; private set; }
        [JsonProperty("aps_mrec_300x250")] public string ApsMrec { get; private set; }

        [JsonProperty("devtodev_app_key")] public string DevtodevAppKey { get; private set; }
        [JsonProperty("appmetrica_id")] public string AppmetricaId { get; private set; }

        [JsonProperty("adjust_app_token")] public string AdjustAppToken { get; private set; }
        [JsonProperty("adjust_revenue_event_token")] public string AdjustRevenueEventToken { get; private set; }
        
        [JsonProperty("adverty_api_key")] public string AdvertyApiKey { get; private set; }
        [JsonProperty("adverty5_api_key")] public string Adverty5ApiKey { get; private set; }
        [JsonProperty("odeeo_api_key")] public string OdeeoApiKey { get; private set; }
        [JsonProperty("odeeo_app_key")] public string OdeeoAppKey { get; private set; }
        [JsonProperty("odeeo_icon_id")] public string OdeeoIconId { get; private set; }
        [JsonProperty("odeeo_rewarded_icon_id")] public string OdeeoRewardedIconId { get; private set; }
        
        [JsonProperty("audiomob_id")] public string AudiomobId { get; private set; }
        [JsonProperty("gadsme_id")] public string GadsmeId { get; private set; }

        [JsonProperty("facebook_id")] public string FacebookId { get; private set; }
        [JsonProperty("facebook_client_token")] public string FacebookClientToken { get; private set; }
        [JsonProperty("facebook_rewarded_id")] public string FacebookRewardedId { get; private set; }
        [JsonProperty("facebook_interstitial_id")] public string FacebookInterstitialId { get; private set; }
        [JsonProperty("facebook_banner_id")] public string FacebookBannerId { get; private set; }

        [JsonProperty("s2s_iap_verify_enable")] public string S2SIapVerifyEnableRaw { get; private set; }
        public bool S2SIapVerifyEnable => "true".Equals(S2SIapVerifyEnableRaw);
        
        [JsonProperty("s2s_iap_verify_url")] public string S2SIapVerifyUrl { get; private set; }
        [JsonProperty("s2s_iap_verify_key")] public string S2SIapVerifyKey { get; private set; }
        public int LoadingTaskTimeout => 5;
        public int TargetFrameRate => 60;
        [JsonProperty("tracking_event_static_property_names")] public string TrackingEventStaticPropertyNames { get; private set; }
        [JsonProperty("appsflyer_dev_key")] public string AppsFlyerDevKey { get; private set; }
        [JsonProperty("fixed_dpi")] private string _fixedDpi; 
        public int FixedDpi => string.IsNullOrEmpty(_fixedDpi) ? 0 : int.Parse(_fixedDpi);

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
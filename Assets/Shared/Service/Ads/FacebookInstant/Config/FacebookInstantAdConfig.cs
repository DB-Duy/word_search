#if FACEBOOK_INSTANT

using Newtonsoft.Json;

namespace Shared.Ads.FacebookInstant.Config
{
    [System.Serializable]
    public class FacebookInstantAdConfig : IAdConfig, IFacebookInstantAdConfig
    {
        [JsonProperty("IsTablet")] public bool IsTablet { get; }
        [JsonProperty("FacebookId")] public string FacebookId { get; }
        [JsonProperty("FacebookRewardedId")] public string FacebookRewardedId { get; }
        [JsonProperty("FacebookInterstitialId")] public string FacebookInterstitialId { get; }
        [JsonProperty("FacebookBannerId")] public string FacebookBannerId { get; }

        public FacebookInstantAdConfig(bool isTablet, string facebookId, string facebookRewardedId, string facebookInterstitialId, string facebookBannerId)
        {
            IsTablet = isTablet;
            FacebookId = facebookId;
            FacebookRewardedId = facebookRewardedId;
            FacebookInterstitialId = facebookInterstitialId;
            FacebookBannerId = facebookBannerId;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
#endif
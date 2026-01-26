#if TOPON
using Newtonsoft.Json;

namespace Shared.Ads.SharedTopOn
{
    public class TopOnAdConfig : ITopOnAdConfig
    {
        [JsonProperty("isTablet")] public bool IsTablet { get; private set; }
        [JsonProperty("topOnAppId")] public string TopOnAppId { get; }
        [JsonProperty("topOnAppKey")] public string TopOnAppKey { get; }
        [JsonProperty("topOnRewarded")] public string TopOnRewarded { get; }
        [JsonProperty("topOnInterstitial")] public string TopOnInterstitial { get; }
        [JsonProperty("topOnBanner")] public string TopOnBanner { get; }

        public TopOnAdConfig(bool isTablet, string topOnAppId, string topOnAppKey, string topOnRewarded, string topOnInterstitial, string topOnBanner)
        {
            IsTablet = isTablet;
            TopOnAppId = topOnAppId;
            TopOnAppKey = topOnAppKey;
            TopOnRewarded = topOnRewarded;
            TopOnInterstitial = topOnInterstitial;
            TopOnBanner = topOnBanner;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
#endif
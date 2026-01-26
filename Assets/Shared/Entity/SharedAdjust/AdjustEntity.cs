#if ADJUST
using AdjustSdk;
using Newtonsoft.Json;

namespace Shared.Entity.SharedAdjust
{
    [System.Serializable]
    public class AdjustEntity
    {
        [JsonProperty("sdk_version")] public string SdkVersion { get; set; }
        [JsonProperty("adjust_id")] public string AdjustId { get; set; }
        [JsonProperty("google_ad_id")] public string GoogleAdId { get; set; }
        [JsonProperty("adjust_attribution")] public AdjustAttribution AdjustAttribution { get; set; }
        [JsonProperty("android_id")] public string AndroidId { get; set; }
    }
}
#endif
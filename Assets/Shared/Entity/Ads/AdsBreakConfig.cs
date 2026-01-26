using Newtonsoft.Json;

namespace Shared.Entity.Ads
{
    public class AdsBreakConfig
    {
        [JsonProperty("unlock")] public bool Unlock { get; private set; }
        [JsonProperty("time_display")] public int TimeDisplay { get; private set; }
    }
}
using Newtonsoft.Json;

namespace Shared.Entity.Ads
{
    [System.Serializable]
    public class InterstitialConfig
    {
        [JsonProperty("is_first_show_level")] private int _enableAfterLevel;
        [JsonProperty("is_show_after_level")] private int _levelSpace;
        [JsonProperty("time_interval")] private int _interval;

        [JsonIgnore] public int EnableAfterLevel => _enableAfterLevel;
        [JsonIgnore] public int LevelSpace => _levelSpace;
        [JsonIgnore] public int IntervalSeconds => _interval;

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
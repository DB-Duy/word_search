using Newtonsoft.Json;

namespace Shared.Repository.RemoteConfig.Models
{
    [System.Serializable]
    public class HuaweiFakeRatingPopup
    {
        [JsonProperty("after_session")] public int AfterSession { get; private set; }
        [JsonProperty("show_after_level")] public int NumberOfPlayed { get; private set; }
        [JsonProperty("interval_session")] public int IntervalSession { get; private set; }
        [JsonProperty("show_popup")] public int MaxShownCount { get; private set; }
        public override string ToString() => JsonConvert.SerializeObject(this);
        
        public static HuaweiFakeRatingPopup DefaultInstance() => new() { AfterSession = 10, NumberOfPlayed = 1, IntervalSession = 5, MaxShownCount = 1 };
    }
}
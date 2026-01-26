using Newtonsoft.Json;

namespace Shared.Entity.Premium
{
    [System.Serializable]
    public class PremiumConfig
    {
        [JsonProperty("Unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("session_to_show")] public int StartSession { get; private set; }
        [JsonProperty("daily_frequency")] public int DailyFrequency { get; private set; }
    }
}
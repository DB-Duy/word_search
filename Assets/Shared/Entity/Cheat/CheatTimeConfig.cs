#if CHEAT_TIME_DETECTOR
using Newtonsoft.Json;

namespace Shared.Entity.Cheat
{
    [System.Serializable]
    public class CheatTimeConfig
    {
        [JsonProperty("unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("url")] public string Url { get; private set; }
        [JsonProperty("allowedOffset")] public int AllowedOffset { get; private set; }
    }
}
#endif
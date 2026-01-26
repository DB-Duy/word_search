using Newtonsoft.Json;

namespace Shared.Entity.StoreRating
{
    [System.Serializable]
    public class StoreRatingConfig
    {
        [JsonProperty("show_popup")] public int MaxShownCount { get; private set; }
        [JsonProperty("show_after_level")] public int AfterLevel { get; private set; }
    }
}
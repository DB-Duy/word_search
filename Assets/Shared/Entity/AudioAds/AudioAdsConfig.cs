using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Entity.AudioAds
{
    /// <summary>
    /// {"Unlocked":true,"first_show_level":4,"interval_by_level":4,"priority":[{"adType":"audiomob","id":2},{"adType":"odeeo","id":1}]}
    /// </summary>
    [System.Serializable]
    public class AudioAdsConfig
    {
        [System.Serializable]
        public class Priority
        {
            [JsonProperty("adType")] public string AdType { get; private set; }

             public Priority(string adType)
            {
                AdType = adType;
            }

            public Priority() { }

            public override string ToString() => JsonConvert.SerializeObject(this);
        }


        [JsonProperty("Unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("retry")] public int Retry { get; }
        [JsonProperty("first_show_level")] public int EnableLevel { get; private set; }
        [JsonProperty("interval_by_level")] public int IntervalInLevel { get; private set; }
        [JsonProperty("priority")] public List<Priority> Priorities { get; private set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}

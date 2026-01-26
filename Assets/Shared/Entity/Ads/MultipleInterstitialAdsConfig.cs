using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Shared.Entity.Ads
{
    [System.Serializable]
    public class MultipleInterstitialAdsConfig
    {
        [System.Serializable]
        public class AdUnit
        {
            [System.Serializable]
            public class ConfigObject
            {
                [JsonProperty("ceiling")] public double? Ceiling { get; private set; }
                [JsonProperty("floor")] public double? Floor { get; private set; }
            }
            
            [JsonProperty("id")] public string Id { get; private set; }
            [JsonProperty("aps_id_static")] public string ApsIdStatic { get; private set; }
            [JsonProperty("aps_id_video")] public string ApsIdVideo { get; private set; }
            [JsonProperty("placement_name")] public string PlacementName { get; private set; }
            [JsonProperty("config")] private ConfigObject Config { get; set; }
        }
        
        [JsonProperty("unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("refresh_rate")] public int RefreshRateInSeconds { get; private set; }
        [JsonProperty("capping")] public int? Capping { get; private set; }
        [JsonProperty("ad_units")] public List<AdUnit> AdUnits { get; private set; }
    }
}
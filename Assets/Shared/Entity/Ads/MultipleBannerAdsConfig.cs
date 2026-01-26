using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Shared.Entity.Ads
{
    [System.Serializable]
    public class MultipleBannerAdsConfig
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
            [JsonProperty("aps_id_phone")] public string ApsIdPhone { get; private set; }
            [JsonProperty("aps_id_tablet")] public string ApsIdTablet { get; private set; }
            [JsonProperty("placement_name")] public string PlacementName { get; private set; }
            [JsonProperty("config")] private ConfigObject Config { get; set; }
        }
        
        [JsonProperty("unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("refresh_rate")] public int RefreshRateInSeconds { get; private set; }
        [JsonProperty("ad_units")] public List<AdUnit> AdUnits { get; private set; }
    }
}
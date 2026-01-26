using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Entity.Ads
{
    [Serializable]
    public class InterstitialAdEntity
    {
        [JsonProperty("close_time")] public DateTime CloseTime { get; set; }
        
        [JsonProperty("capping_map")] public Dictionary<string, int> CappingMap { get; set; }
    }
}
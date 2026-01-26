using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Entity.InPlayAds
{
    /// <summary>
    /// {"Unlocked":true,"placements":{"priority_ingame":["GadsmeCanvasVideo4D3","GadsmeCanvasVideo16D9","AdvertyWorldBox"],"priority_endgame":[]}}
    /// </summary>
    [Serializable]
    public class InplayAdsConfig
    {
        [JsonProperty("unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("placements")] public Dictionary<string, List<string>> Placements { get; private set; }
    }
}
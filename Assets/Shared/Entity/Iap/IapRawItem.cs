using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.Entity.Iap
{
    [Serializable]
    public class IapRawItem
    {
        [JsonProperty("id")] public string Id { get; private set; }
            
        [JsonProperty("appstore")] public string AppStoreProductId { get; private set; }
        [JsonProperty("googleplay")] public string GooglePlayProductId { get; private set; }
            
        [JsonProperty("type")] public string Type { get; private set; }
            
        [JsonProperty("default_usd_price")] public string DefaultUsdPrice { get; private set; }
        [JsonProperty("reward")] public string RewardJsonString { get; private set; }
        [JsonProperty("en")] public string LocalizationTitle { get; private set; }
    }
}
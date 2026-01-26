using Newtonsoft.Json;

namespace Shared.Entity.Iap
{
    [System.Serializable]
    public class SharedProductMetadata
    {
        [JsonProperty("localizedPriceString")] public string LocalizedPriceString { get; set; }
        [JsonProperty("localizedTitle")] public string LocalizedTitle { get; set; }
        [JsonProperty("localizedDescription")] public string LocalizedDescription { get; set; }
        [JsonProperty("isoCurrencyCode")] public string IsoCurrencyCode { get; set; }
        [JsonProperty("localizedPrice")] public decimal LocalizedPrice { get; set; }
    }
}
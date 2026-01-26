using System;
using Newtonsoft.Json;
using Shared.Utils;

namespace Shared.Entity.Iap
{
    [System.Serializable]
    public class IapItem
    {
        [JsonProperty("id")] public string Id { get; set; }
            
        [JsonProperty("productId")] public string ProductId { get; set; }
            
        [JsonProperty("type")] public string Type { get; set; }
            
        [JsonProperty("default_usd_price")] public float DefaultUsdPrice { get; set; }
        [JsonProperty("reward")] public string RewardJsonString { get; set; }
        [JsonProperty("en")] public string LocalizationTitle { get; set; }
        
        public static IapItem Of(IapRawItem rawItem)
        {
#if GOOGLE_PLAY
            var productId = rawItem.GooglePlayProductId;
#elif APPSTORE
            var productId = rawItem.AppStoreProductId;
#else 
            var productId = rawItem.Id;
#endif
#if LOG_INFO
            if (string.IsNullOrEmpty(productId))
                throw new Exception($"ProductId is empty for {rawItem.Id}");
#endif
#if DEBUG_IAP && LOG_INFO
            productId = $"{productId}.debug";
#endif            
            return new IapItem
            {
                Id = rawItem.Id,
                ProductId = productId,
                Type = rawItem.Type,
                DefaultUsdPrice = rawItem.DefaultUsdPrice.CastToFloat(),
                RewardJsonString = rawItem.RewardJsonString,
                LocalizationTitle = rawItem.LocalizationTitle
            };
        }
    }
}
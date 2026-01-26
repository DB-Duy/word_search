using System;
using Newtonsoft.Json;

namespace Shared.Entity.Iap
{
    [Serializable]
    public class SharedSubscriptionInfo
    {
        [JsonProperty("product_id")] public string ProductId { get; set; }
        
        [JsonProperty("is_subscribed")] public bool IsSubscribed { get; set; }
        [JsonProperty("is_expired")] public bool IsExpired { get; set; }
        [JsonProperty("is_cancelled")] public bool IsCancelled { get; set; }
        [JsonProperty("is_free_trial")] public bool IsFreeTrial { get; set; }

        // [JsonProperty("is_auto_renewing")]  is_auto_renewing;
        // [JsonProperty("is_introductory_price_period")]  is_introductory_price_period;
        [JsonProperty("purchase_date")] public DateTime PurchaseDate { get; set; }
        [JsonProperty("subscription_expire_date")] public DateTime ExpireDate { get; set; }
        [JsonProperty("subscription_cancel_date")] public DateTime CancelDate { get; set; }
        [JsonProperty("remained_time")] public TimeSpan RemainedTime { get; set; }

        public bool IsValidSubscription
        {
            get
            {
                if (IsSubscribed) return true;
                if (!IsExpired) return true;
                if (IsFreeTrial) return true;
                return false;
            }
        }
    }
}
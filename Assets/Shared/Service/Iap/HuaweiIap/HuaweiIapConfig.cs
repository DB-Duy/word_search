#if HUAWEI
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.IAP.Config;

namespace Shared.IAP.HuaweiIap
{
    public class HuaweiIapConfig : IIapConfig
    {
        [JsonProperty("consumable_products")] public List<string> ConsumableProducts { get; }
        [JsonProperty("non-consumable_products")] public List<string> NonConsumableProducts { get; }
        [JsonProperty("subscriptions_products")] public List<string> SubscriptionsProducts { get; }
        
        public IIapConfig AddConsumableProducts(params string[] products)
        { 
            ConsumableProducts.AddRange(products);
            return this;
        }

        public IIapConfig AddNonConsumableProducts(params string[] products)
        {
            NonConsumableProducts.AddRange(products);
            return this;
        }

        public IIapConfig AddSubscriptionsProducts(params string[] products)
        {
            SubscriptionsProducts.AddRange(products);
            return this;
        }

        public bool ValidateConsumableProduct(string productId) => ConsumableProducts.Contains(productId);

        public bool ValidateNonConsumableProduct(string productId) => NonConsumableProducts.Contains(productId);

        public bool ValidateSubscriptionsProduct(string productId) => SubscriptionsProducts.Contains(productId);

        public HuaweiIapConfig(List<string> consumableProducts = null, List<string> nonConsumableProducts = null, List<string> subscriptionsProducts = null)
        {
            ConsumableProducts = consumableProducts ?? new List<string>();
            NonConsumableProducts = nonConsumableProducts ?? new List<string>();
            SubscriptionsProducts = subscriptionsProducts ?? new List<string>();
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
#endif
using Newtonsoft.Json;

namespace Shared.Service.Iap.Internal
{
    [System.Serializable]
    public class Purchasing
    {
        [JsonProperty("product_id")] public string ProductId { get; set; }

        public Purchasing(string productId)
        {
            ProductId = productId;
        }
    }
}
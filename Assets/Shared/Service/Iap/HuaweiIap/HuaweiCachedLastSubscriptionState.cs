using Newtonsoft.Json;

namespace Shared.IAP.HuaweiIap
{
    [System.Serializable]
    public class HuaweiCachedLastSubscriptionState
    {
        [JsonProperty("productId")] public string ProductId { get; private set; }
        [JsonProperty("inValid")] public bool IsValid { get; private set; }

        public HuaweiCachedLastSubscriptionState(string productId, bool isValid)
        {
            ProductId = productId;
            IsValid = isValid;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
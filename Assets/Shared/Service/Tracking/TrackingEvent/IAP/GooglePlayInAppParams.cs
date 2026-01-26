using Newtonsoft.Json;
using Shared.Entity.Iap;
using Shared.Tracking.Models.IAP;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.IAP
{
    public class GooglePlayInAppParams : BaseTrackingEvent, IGooglePlayPurchasingParams
    {
        [JsonProperty("event_name")] public override string EventName => "GoogleSubscriptionParams";

        // 1. Product Define
        public string ProductId { get; set; }
        public float DefaultUsdPriceValue { get; set; }

        // 2. Transaction
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string PurchaseTime { get; set; }

        public long PriceAmountMicros { get; set; }
        public double Amount { get; set; }
        public string PriceCurrencyCode { get; set; }

        public string Json { get; set; }
        public string Signature { get; set; }
        public string PurchaseToken { get; set; }
        public string Receipt { get; set; }


        public override string ToString() => JsonConvert.SerializeObject(this);

#if IAP
        public static GooglePlayInAppParams Of(UnityEngine.Purchasing.Product purchasedProduct, GooglePlayInAppReceipt receipt, float defaultUsdPrice)
        {
            var priceAmountMicros = long.Parse(receipt.Payload.SkuDetails[0].PriceAmountMicros);
            var amount = priceAmountMicros / 1000000d;
            return new GooglePlayInAppParams
            {
                // 1. Product Define
                ProductId = purchasedProduct.definition.id,
                DefaultUsdPriceValue = defaultUsdPrice,
                // 2. Transaction
                OrderId = receipt.Payload.Json.OrderId,
                TransactionId = receipt.TransactionID,
                PurchaseTime = receipt.Payload.Json.PurchaseTime.ToString(),

                PriceAmountMicros = priceAmountMicros,
                Amount = amount,
                PriceCurrencyCode = receipt.Payload.SkuDetails[0].PriceCurrencyCode,

                Json = receipt.Payload.JsonString,
                Signature = receipt.Payload.Signature,
                PurchaseToken = receipt.Payload.Json.PurchaseToken,
                Receipt = purchasedProduct.receipt,
            };
        }
#endif
    }
}
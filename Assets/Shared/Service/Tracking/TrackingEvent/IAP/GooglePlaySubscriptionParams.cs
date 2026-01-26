using Shared.Entity.Iap;
using Shared.Tracking.Models.IAP;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.IAP
{
    /// <summary>
    /// Google Product
    /// 
    /// freeTrialPeriod
    /// {
    ///     "definition.id": "com.indiez.nonogram.premiumpack",
    ///     "metadata.localizedTitle": "Premium Pack (Pixel Art: Logic Nonogram)",
    ///     "metadata.localizedPriceString": "$1.99",
    ///     "introductoryPrice": null,
    ///     "introductoryPricePeriod": null,
    ///     "introductoryPriceCycles": 0,
    ///     "freeTrialPeriod": "P3D",
    ///     "originalJson": "{\"productId\":\"com.indiez.nonogram.premiumpack\",\"type\":\"subs\",\"title\":\"Premium Pack (Pixel Art: Logic Nonogram)\",\"name\":\"Premium Pack\",\"description\":\"\",\"price\":\"$1.99\",\"price_amount_micros\":\"1990000\",\"price_currency_code\":\"USD\",\"subscriptionPeriod\":\"P1W\",\"freeTrialPeriod\":\"P3D\"}"
    /// }
    ///
    /// </summary>
    public class GooglePlaySubscriptionParams : ITrackingEvent, IGooglePlayPurchasingParams
    {
        public string EventName => "GoogleSubscriptionParams";
        
        // 1. Product Define
        public string ProductId { get; set; }
        public float DefaultUsdPriceValue { get; set; }
        // 2. Transaction
        public string OrderId { get; set; }
        public string PurchaseTime { get; set; }
        
        /// <summary>
        /// "price": "₫49,000",
        /// "price_amount_micros": "49000000000",
        /// "price_currency_code": "VND",
        /// </summary>
        public long PriceAmountMicros { get; set; }
        public double PriceAmountMicrosDivide1M { get; set; }
        public string PriceCurrencyCode { get; set; }
        
        public string Json { get; set; }
        public string Signature { get; set; }
        public string PurchaseToken { get; set; }
        public string Receipt { get; set; }
        
#if IAP
        public static GooglePlaySubscriptionParams Of(UnityEngine.Purchasing.Product purchaseProduct, GooglePlaySubscriptionReceipt receipt, float defaultUsdPrice)
        {
            var priceAmountMicros = long.Parse(receipt.Payload.SkuDetails[0].PriceAmountMicros);
            var amount = priceAmountMicros / 1000000d;
            return new GooglePlaySubscriptionParams
            {
                // 1. Product Define
                ProductId = purchaseProduct.definition.id,
                DefaultUsdPriceValue = defaultUsdPrice,
                // 2. Transaction
                OrderId = receipt.Payload.Json.OrderId,
                PurchaseTime = receipt.Payload.Json.PurchaseTime.ToString(),
                
                PriceAmountMicros = priceAmountMicros,
                PriceAmountMicrosDivide1M = amount,
                PriceCurrencyCode = receipt.Payload.SkuDetails[0].PriceCurrencyCode,
                
                Json = receipt.Payload.JsonString,
                Signature = receipt.Payload.Signature,
                PurchaseToken = receipt.Payload.Json.PurchaseToken,
                Receipt = purchaseProduct.receipt,
                
            };
        }
#endif
    }
}
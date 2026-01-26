#if IAP
using System;
using Newtonsoft.Json;
using UnityEngine.Purchasing.Security;

namespace Shared.Entity.Iap
{
    [Serializable]
    public class AppleIapReceiptEntity
    {
        /// <summary>
        /// The number of items purchased.
        /// </summary>
        [JsonProperty("quantity")] public int Quantity { get; internal set; }

        /// <summary>
        /// The product ID
        /// </summary>
        [JsonProperty("productID")] public string ProductID { get; internal set; }

        /// <summary>
        /// The ID of the transaction.
        /// </summary>
        [JsonProperty("transactionID")] public string TransactionID { get; internal set; }

        /// <summary>
        /// For a transaction that restores a previous transaction, the transaction ID of the original transaction. Otherwise, identical to the transactionID.
        /// </summary>
        [JsonProperty("originalTransactionIdentifier")] public string OriginalTransactionIdentifier { get; internal set; }

        /// <summary>
        /// The date of purchase.
        /// </summary>
        [JsonProperty("purchaseDate")] public DateTime PurchaseDate { get; internal set; }

        /// <summary>
        /// For a transaction that restores a previous transaction, the date of the original transaction.
        /// </summary>
        [JsonProperty("originalPurchaseDate")] public DateTime OriginalPurchaseDate { get; internal set; }

        /// <summary>
        /// The expiration date for the subscription, expressed as the number of milliseconds since January 1, 1970, 00:00:00 GMT.
        /// </summary>
        [JsonProperty("subscriptionExpirationDate")] public DateTime SubscriptionExpirationDate { get; internal set; }

        /// <summary>
        /// For a transaction that was canceled by Apple customer support, the time and date of the cancellation.
        /// For an auto-renewable subscription plan that was upgraded, the time and date of the upgrade transaction.
        /// </summary>
        [JsonProperty("cancellationDate")] public DateTime CancellationDate { get; internal set; }

        /// <summary>
        /// For a subscription, whether or not it is in the free trial period.
        /// </summary>
        [JsonProperty("isFreeTrial")] public int IsFreeTrial { get; internal set; }

        /// <summary>
        /// The type of product.
        /// </summary>
        [JsonProperty("productType")] public int ProductType { get; internal set; }

        /// <summary>
        /// For an auto-renewable subscription, whether or not it is in the introductory price period.
        /// </summary>
        [JsonProperty("isIntroductoryPricePeriod")] public int IsIntroductoryPricePeriod { get; internal set; }

        public override string ToString() => JsonConvert.SerializeObject(this);

        public static AppleIapReceiptEntity NewInstance(AppleInAppPurchaseReceipt receipt)
        {
            return new AppleIapReceiptEntity
            {
                Quantity = receipt.quantity,
                ProductID = receipt.productID,
                TransactionID = receipt.transactionID,
                OriginalTransactionIdentifier = receipt.originalTransactionIdentifier,
                PurchaseDate = receipt.purchaseDate,
                OriginalPurchaseDate = receipt.originalPurchaseDate,
                SubscriptionExpirationDate = receipt.subscriptionExpirationDate,
                CancellationDate = receipt.cancellationDate,
                IsFreeTrial = receipt.isFreeTrial,
                ProductType  = receipt.productType,
                IsIntroductoryPricePeriod = receipt.isIntroductoryPricePeriod
            };
        }
    }
}
#endif
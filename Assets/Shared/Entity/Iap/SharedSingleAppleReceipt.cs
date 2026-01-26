#if IAP
using System;
using Newtonsoft.Json;
using Shared.Entity.Iap;
using Shared.Utils;
using UnityEngine.Serialization;

namespace Shared.IAP.AppStore.Validator
{
    /// <summary>
    /// An Apple receipt as defined here:
    /// https://developer.apple.com/library/ios/releasenotes/General/ValidateAppStoreReceipt/Chapters/ReceiptFields.html#//apple_ref/doc/uid/TP40010573-CH106-SW1
    /// </summary>
    [Serializable]
    public class SharedSingleAppleReceipt
    {
        /// <summary>
        /// The app bundle ID
        /// </summary>
        [JsonProperty("bundleID")] public string BundleID { get; internal set; }

        /// <summary>
        /// The app version number
        /// </summary>
        [JsonProperty("appVersion")] public string AppVersion { get; internal set; }

        /// <summary>
        /// The expiration date of the receipt
        /// </summary>
        [JsonProperty("expirationDate")] public DateTime ExpirationDate { get; internal set; }

        /// <summary>
        /// An opaque value used, with other entity, to compute the SHA-1 hash during validation.
        /// </summary>
        [JsonProperty("opaque")] public byte[] Opaque { get; internal set; }

        /// <summary>
        /// A SHA-1 hash, used to validate the receipt.
        /// </summary>
        [JsonProperty("hash")] public byte[] Hash { get; internal set; }

        /// <summary>
        /// The version of the app that was originally purchased.
        /// </summary>
        [JsonProperty("originalApplicationVersion")] public string OriginalApplicationVersion { get; internal set; }

        /// <summary>
        /// The date the receipt was created
        /// </summary>
        [JsonProperty("receiptCreationDate")] public DateTime ReceiptCreationDate { get; internal set; }

        /// <summary>
        /// The receipts of the In-App purchases.
        /// </summary>
        [FormerlySerializedAs("InAppPurchaseReceipt")] [JsonProperty("inAppPurchaseReceipt")] public AppleIapReceiptEntity iapReceiptEntity;

        public override string ToString() => this.ToJsonString();

        public static SharedSingleAppleReceipt NewInstance(SharedAppleReceipt receipt, String transactionId)
        {
            return new SharedSingleAppleReceipt
            {
                BundleID = receipt.BundleID,
                AppVersion = receipt.AppVersion,
                ExpirationDate = receipt.ExpirationDate,
                Opaque = receipt.Opaque,
                Hash = receipt.Hash,
                OriginalApplicationVersion = receipt.OriginalApplicationVersion,
                ReceiptCreationDate = receipt.ReceiptCreationDate,
                iapReceiptEntity = receipt.GetByTransactionId(transactionId)
            };
        }
    }
}
#endif
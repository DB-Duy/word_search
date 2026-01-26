#if IAP
using System;
using System.Linq;
using Newtonsoft.Json;
using Shared.Entity.Iap;
using Shared.Utils;
using UnityEngine.Purchasing.Security;

namespace Shared.IAP.AppStore.Validator
{
    /// <summary>
    /// An Apple receipt as defined here:
    /// https://developer.apple.com/library/ios/releasenotes/General/ValidateAppStoreReceipt/Chapters/ReceiptFields.html#//apple_ref/doc/uid/TP40010573-CH106-SW1
    /// </summary>
    [Serializable]
    public class SharedAppleReceipt
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
        [JsonProperty("inAppPurchaseReceipts")] public AppleIapReceiptEntity[] InAppPurchaseReceipts;

        public AppleIapReceiptEntity GetByTransactionId(string transactionId) => InAppPurchaseReceipts.FirstOrDefault(r => r.TransactionID == transactionId);

        public override string ToString() => this.ToJsonString();

        private static AppleIapReceiptEntity[] NewInstances(AppleInAppPurchaseReceipt[] receipts)
        {
            var sharedReceipts = new AppleIapReceiptEntity[receipts.Length];
            for (var i = 0; i < receipts.Length; i++)
            {
                sharedReceipts[i] = AppleIapReceiptEntity.NewInstance(receipts[i]);
            }
            return sharedReceipts;
        }

        public static SharedAppleReceipt NewInstance(AppleReceipt receipt)
        {
            return new SharedAppleReceipt
            {
                BundleID = receipt.bundleID,
                AppVersion = receipt.appVersion,
                ExpirationDate = receipt.expirationDate,
                Opaque = receipt.opaque,
                Hash = receipt.hash,
                OriginalApplicationVersion = receipt.originalApplicationVersion,
                ReceiptCreationDate = receipt.receiptCreationDate,
                InAppPurchaseReceipts = NewInstances(receipt.inAppPurchaseReceipts)
            };
        }
    }
}
#endif
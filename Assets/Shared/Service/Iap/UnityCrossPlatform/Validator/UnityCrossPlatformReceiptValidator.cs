// #if IAP && (APPSTORE || GOOGLE_PLAY)
// using System.Diagnostics;
// using Shared.Core.Async;
// using Shared.Core.IoC;
// using Shared.Entity.Iap;
// using Shared.Utils;
// using UnityEngine;
// using UnityEngine.Purchasing;
// using UnityEngine.Purchasing.Security;
//
// namespace Shared.Service.Iap.UnityCrossPlatform.Validator
// {
//     /// <summary>
//     /// https://docs.unity3d.com/Packages/com.unity.purchasing@4.5/manual/UnityIAPValidatingReceipts.html
//     /// </summary>
//     [Component]
//     public class UnityCrossPlatformReceiptValidator : IUnityIapReceiptValidator, ISharedUtility
//     {
//         public IAsyncOperation Validate(Product purchasedProduct)
//         {
//             var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
//             try
//             {
//                 var receipts = validator.Validate(purchasedProduct.receipt);
//                 _LogAppleReceipts(receipts);
//                 return new SharedAsyncOperation().Success();
//             }
//             catch (IAPSecurityException exception)
//             {
//                 this.LogError(SharedLogTag.Iap, nameof(exception.Message), exception.Message);
//                 return new SharedAsyncOperation().Fail(exception.Message);
//             }
//         }
//
//         [Conditional("APPSTORE")]
//         private void _LogAppleReceipts(IPurchaseReceipt[] receipts)
//         {
//             foreach (var productReceipt in receipts) {
//                 if (productReceipt is not AppleInAppPurchaseReceipt apple) continue;
//                 var appleReceipt = AppleIapReceiptEntity.NewInstance(apple);
//                 this.LogInfo(SharedLogTag.Iap, nameof(appleReceipt), appleReceipt);
//                 break;
//             }
//         }
//
//     }
// }
// #endif
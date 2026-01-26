// #if APPSTORE
// using com.adjust.sdk;
// using Shared.Tracking.Models.Templates;
// using Shared.Tracking.Templates;
// using UnityEngine.Purchasing;
//
// namespace Shared.Tracking.Models.IAP
// {
//     public class AdjustDtdAppStoreSubscriptionParams : ITrackingEvent, IDtdAppStoreSubscriptionEvent, IAdjustAppStoreSubscriptionEvent
//     {
//         private readonly PurchaseEventArgs _purchaseEvent;
//
//         public AdjustDtdAppStoreSubscriptionParams(PurchaseEventArgs purchaseEvent)
//         {
//             _purchaseEvent = purchaseEvent;
//         }
//
//         public string EventName => "AppStoreSubscriptionParams";
//
//         public AdjustAppStoreSubscription ToAdjustAppStoreSubscription()
//         {
//             var appStoreSubscription = new AdjustAppStoreSubscription(
//                 price: _purchaseEvent.purchasedProduct.metadata.localizedPrice.ToString(),
//                 currency: _purchaseEvent.purchasedProduct.metadata.isoCurrencyCode,
//                 transactionId: _purchaseEvent.purchasedProduct.transactionID,
//                 receipt: _purchaseEvent.purchasedProduct.receipt);
//             return appStoreSubscription;
//         }
//
//         public Product ToDtdAppStoreSubscription() => _purchaseEvent.purchasedProduct;
//     }
// }
// #endif
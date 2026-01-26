using Shared.Tracking.Models.IAP;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.IAP
{
    public class AppStoreSubscriptionParams : ITrackingEvent, IAppStorePurchasingParams
    {
        public string EventName => "AppStoreSubscriptionParams";

        // 1. Product Define
        public string ProductID { get; }
        public decimal LocalizedPrice { get; }
        public string IsoCurrencyCode { get; }
        public float DefaultUsdPriceValue { get; }
        // 2. Transaction
        public string TransactionID { get; }
        public string Receipt { get; }
        
#if IAP
        public AppStoreSubscriptionParams(UnityEngine.Purchasing.Product purchasedProduct, float defaultUsdPrice)
        {      
            // 1. Product Define
            ProductID = purchasedProduct.definition.id;
            LocalizedPrice = purchasedProduct.metadata.localizedPrice;
            IsoCurrencyCode = purchasedProduct.metadata.isoCurrencyCode;
            DefaultUsdPriceValue = defaultUsdPrice;
            // 2. Transaction
            TransactionID = purchasedProduct.transactionID;
            Receipt = purchasedProduct.receipt;
        }
#endif
    }
}

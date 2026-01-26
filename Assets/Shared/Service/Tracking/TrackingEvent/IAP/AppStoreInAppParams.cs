using Shared.Tracking.Models.IAP;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.IAP
{
    public class AppStoreInAppParams : ITrackingEvent, IAppStorePurchasingParams
    {
        public string EventName => "AppStoreInAppParams";
        // 1. Product Define
        public string ProductID { get; }
        public decimal LocalizedPrice { get; }
        public string IsoCurrencyCode { get; }
        public float DefaultUsdPriceValue { get; }
        // 2. Transaction
        public string TransactionID { get; }
        public string Receipt { get; }
        
#if IAP
        public AppStoreInAppParams(UnityEngine.Purchasing.Product purchasedProduct, float defaultUsdPriceValue)
        {
            ProductID = purchasedProduct.definition.id;
            LocalizedPrice = purchasedProduct.metadata.localizedPrice;
            IsoCurrencyCode = purchasedProduct.metadata.isoCurrencyCode;
            DefaultUsdPriceValue = defaultUsdPriceValue;
            
            // 2. Transaction
            TransactionID = purchasedProduct.transactionID;
            Receipt = purchasedProduct.receipt;
        }
#endif
    }
}
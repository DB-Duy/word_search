using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.Handler.Corou.Initializable;
using Shared.Entity.Iap;
using Shared.Service.Iap.Internal;
using SharedPurchasing = Shared.Service.Iap.Internal.Purchasing;

namespace Shared.Service.Iap
{
    public interface IIapService : IInitializable
    {
        IAsyncOperation<SharedPurchasing> Purchase(string productId);
        
        // Consumables
        IAsyncOperation<SharedPurchasing> ProcessPendingConsumableByProductId(string productId);
        List<string> GetAllPendingConsumableProductId();
        
        // Subscriptions
        SharedSubscriptionInfo GetSubscriptionInfo(string productId);
        bool ValidateSubscription(string productId);
        
        // Non-Consumables
        bool IsNonConsumablePackageOwned(string productId);

        // Product Define
        SharedProductMetadata GetProduct(string productId);
        T GetReward<T>(string productId);
        
        // APIs
        void RestorePurchases();
        void RedirectingToSubscriptionManagementScreen(string productId = null);
    }
}
#if IAP && (UNITY_ANDROID || UNITY_IOS)
using Shared.Entity.Iap;
using UnityEngine.Purchasing;

namespace Shared.Service.Iap.Internal
{
    public static class SubscriptionInfoExtensions
    {
        public static SharedSubscriptionInfo ToSharedSubscriptionInfo(this SubscriptionInfo i)
        {
            return new SharedSubscriptionInfo
            {
                ProductId = i.getProductId(),
                
                IsSubscribed = i.isSubscribed() == Result.True,
                IsExpired = i.isExpired() == Result.True,
                IsCancelled = i.isCancelled() == Result.True,
                IsFreeTrial = i.isFreeTrial() == Result.True,
                
                PurchaseDate = i.getPurchaseDate(),
                ExpireDate = i.getExpireDate(),
                CancelDate = i.getCancelDate(),
                RemainedTime = i.getRemainingTime()
            };
        }
    }
}
#endif